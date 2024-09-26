//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using Newtonsoft.Json;
//using Cysharp.Threading.Tasks;
//using UnityEngine.AddressableAssets;
//using UnityEngine.Assertions;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using static RuntimeData;
//using System.Linq;


//// �R���e���g�̎��
//public enum DownloadContentType
//{
//    Prefab,
//    Scene,
//    Motion,
//}



//// ��荞��catalog�ƁAcalatlog�����ɍ�����J�^���O����DL�����R���e���g�̃f�[�^�x�[�X��Json�Ƃ��ĊǗ�����
//public class ContentsDatabase : Savable
//{
//    #region ====== Savable �̎d���� ================================================
//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
//    public override string SaveFolderPath { get; set; } = "DownloadContents";�@// DB��json��ۑ�����p�X
//    #endregion =======================================


//    #region ====== �f�[�^ ================================================
//    // CatalogModel�FCatalog �t�@�C���ƁA���̒��̃R���e���c�A�h���X������ۗL����

//    // �C���|�[�g�ς݂�CatalogModel�ꗗ�Bkey�F���[�U�[�����������R���e���g��
//    public Dictionary<string, ContentModel> importedContents { get; set; } = new Dictionary<string, ContentModel>();
    
//    // �J�^���O�ɕ\������R���e���c�ꗗ�Bkey�F���[�U�[�����������J�^���O��
//    public Dictionary<string, CatalogModel> catalogContents{ get; set; } = new Dictionary<string, CatalogModel>();
//    #endregion =======================================


//    // �R���e���c��ނ��Ƃ�DB�C���X�^���X���Ǘ����鎫��
//    public static Dictionary<DownloadContentType, ContentsDatabase> Databases { get; set; } = new Dictionary<DownloadContentType, ContentsDatabase>()
//    {
//        { DownloadContentType.Prefab, new ContentsDatabase($"{Application.dataPath}/DownloadContents", "Prefabs") },
//        { DownloadContentType.Scene, new ContentsDatabase($"{Application.dataPath}/DownloadContents", "Scenes") },
//        { DownloadContentType.Motion, new ContentsDatabase($"{Application.dataPath}/DownloadContents", "Motions") },
//    };


//    // �R���X�g���N�^ 
//    // �����FDB��json��ۑ�����p�X�ADB��json�t�@�C����
//    public ContentsDatabase(string saveFolderPath = "", string fileName = "")
//    {
//        SaveFolderPath = saveFolderPath;
//        FileName = fileName;
//    }


//    // DB�̎���catalogContents�ɃJ�^���O���Z�[�u
//    // �����F�C�ӂ̃J�^���O���A Catalog�t�@�C���A�J�^���O�ɕ\������R���e���c�̃A�h���X�B
//    public void SaveCatalog(string catalogName, string catalogFile, List<string> addresses)
//    {
//        if (catalogContents.ContainsKey(catalogName))
//        {
//            return;
//        }

//        catalogContents.Add(catalogName, new CatalogModel(catalogFile, addresses));
//        Save();
//    }


//    // DB�̎���catalogContents�ɃJ�^���O�����[�h
//    // �����F���[�h�������J�^���O�̖��O�i���[�U�[�������j
//    public async UniTask<CatalogModel> LoadCatalog(string catalogName)
//    {
//        Load();
//        CatalogModel catalogModel = catalogContents[catalogName];
//        if (!catalogContents.ContainsKey(catalogName))
//        {
//            Debug.LogWarning($"{catalogName} �Ƃ����J�^���O�͂���܂���B�C���|�[�g���Ă��������B");
//            return null;
//        }
//        await RegisterCatalog(catalogModel.CatalogFile);
//        return catalogModel;
//    }


//    // �J�^���O���烆�[�U�[��DL�����R���e���g���Z�[�u�iimportedContents�ɒǉ�����Save�j
//    // �����F�R���e���g�̖��O�i���[�U�[�������j�A�R���e���g����������catalog�t�@�C���A�R���e���g��Adress
//    public void SaveContent(string contentName, string catalogFile, string address)
//    {
//        if (importedContents.ContainsKey(contentName))
//        {
//            Debug.LogWarning($"���L�̃R���e���g�͊��ɑ��݂���̂ŃZ�[�u�����Ȃ��ǂ��܂�\n{contentName}");
//            return;
//        }
//        importedContents.Add(contentName, new ContentModel(catalogFile, address));
//        Save();
//    }


//    // DL�ς݂̃v���n�u�R���e���g�����[�h
//    // �����F�R���e���g�̖��O�i���[�U�[�������j
//    public async UniTask<GameObject> LoadConetent_Prefab(string contentName)
//    {
//        Load();
//        ContentModel contentModel = importedContents[contentName];
//        if (!importedContents.ContainsKey(contentName))
//        {
//            Debug.LogWarning($"{contentName} �Ƃ����R���e���c�͂���܂���B�C���|�[�g���Ă��������B");
//            return null;
//        }
//        await RegisterCatalog(contentModel.CatalogFile);
//        GameObject content = await Addressables.LoadAssetAsync<GameObject>(contentModel.Address);

//        return content;
//    }


//    // DL�ς݂̑S�R���e���c�����[�h�B���[�U�[���Ǘ�����p�̃e�[�u���𐶐�����ۂ̂��
//    public async UniTask<List<GameObject>> LoadAllConetents_Prefab()
//    {
//        Load();
//        List<GameObject> contents = new List<GameObject>();
//        foreach (var a in importedContents.Values)
//        {
//            await RegisterCatalog(a.CatalogFile);
//            contents.Add(await Addressables.LoadAssetAsync<GameObject>(a.Address));
//        }
//        return contents;
//    }


//    // �g�������J�^���O�����\�[�X���P�[�^�ɓo�^
//    public async UniTask<IResourceLocator> RegisterCatalog(string catalogFile)
//    {
//        // �V�����J�^���O���擾�B�t�@�C���p�X��URL
//        AsyncOperationHandle<IResourceLocator> requestCatalog
//            = Addressables.LoadContentCatalogAsync(GetCataLogPath(catalogFile));

//        // ���[�h������҂�
//        await requestCatalog;

//        // ���̃G���[��������
//        Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);

//        // ���\�[�X���P�[�^�ɃJ�^���O���o�^����Ă��Ȃ��ꍇ�͒ǉ�
//        if (Addressables.ResourceLocators.Contains(requestCatalog.Result)) Addressables.AddResourceLocator(requestCatalog.Result);
//        else Debug.LogWarning($"�ȉ��̃J�^���O�̓��\�[�X���P�[�^�Ɋ��ɓo�^����Ă�����̂ł���\n{requestCatalog.Result}");

//        // ���\�[�X���P�[�^�i�擾�����J�^���O���f�V���A���C�Y���ꂽ���́j��Ԃ�
//        return requestCatalog.Result;
//    }


//    // �f�[�^�x�[�X�ɓo�^����Ă���S�J�^���O����C�Ƀ��\�[�X���P�[�^�ɓo�^
//    public async UniTask RegisterAllCatalogs()
//    {
//        List<AsyncOperationHandle<IResourceLocator>> requestCatalogs
//            = new List<AsyncOperationHandle<IResourceLocator>>();

//        List<IResourceLocator> resourceLocators
//           = new List<IResourceLocator>();

//        // 1��1�̃��[�h������҂����A�Ƃ�ܑS�����[�h�𑖂点�ăo�b�t�@�ɒǉ����Ă���
//        foreach (var a in importedContents)
//        {
//            requestCatalogs.Add(Addressables.LoadContentCatalogAsync(GetCataLogPath(a.Value.CatalogFile)));
//        }

//        foreach (var a in requestCatalogs)
//        {
//            // ���[�h������҂�
//            await a;

//            // ���[�h�Ɏ��s���Ă������~
//            Assert.AreEqual(AsyncOperationStatus.Succeeded, a.Status);

//            // ���\�[�X���P�[�^�ɃJ�^���O��ǉ�
//            if (Addressables.ResourceLocators.Contains(a.Result)) Addressables.AddResourceLocator(a.Result);
//            else Debug.LogWarning($"�ȉ��̃J�^���O�̓��\�[�X���P�[�^�Ɋ��ɓo�^����Ă�����̂ł���\n{a.Result}");

//            // ���\�[�X���P�[�^�Q�i�擾�����J�^���O���f�V���A���C�Y���ꂽ���́j��Ԃ�
//            resourceLocators.Add(a.Result);
//        }
//    }


//    // �{�v���W�F�N�g�ɕ����ς݂�catalog�t�@�C���̃p�X���擾
//    string GetCataLogPath(string catalogFile)
//    {
//        return $"{AddressablesFolderPath}/{catalogFile}";
//    }

//    //string GetCataLogPath(string catalogFile, AssetPurpose purpose = AssetPurpose.AdditionalContents)
//    //{
//    //    if (purpose == AssetPurpose.DividedAssets) 
//    //        return $"{DividedAssetsFolderPath}/{catalogFile}";
//    //    else 
//    //        return $"{AddressablesFolderPath}/{catalogFile}";
//    //}
//}



//// ����̃C���X�^���X�P�ɂ��P�̃R���e���g��\������B
//// �R���e���g��Adress�ƁA���̃R���e���g����������catalog�t�@�C���̃p�X��ۗL����f�[�^�N���X
//public class ContentModel
//{
//    public string CatalogFile { get; set; }
//    public string Address { get; set; }

//    public ContentModel(string catalogFile, string adress)
//    {
//        CatalogFile = catalogFile;
//        Address = adress;
//    }
//}



//// ����̃C���X�^���X�P�ɂ��P�̃J�^���O��\������B
//// CatalogModel�F�Ƃ��� Catalog�t�@�C���̃p�X�ƁA���̒��̃R���e���cAdress������ۗL����
//public class CatalogModel
//{
//    public string CatalogFile { get; set; }
//    public List<string> Addresses { get; set; }

//    // .bundle �t�@�C���� .hash �t�@�C���̃p�X�����Â�L�^����悤�ɂ���

//    public CatalogModel(string catalogFile, List<string> addresses)
//    {
//        CatalogFile = catalogFile;
//        Addresses = addresses;
//    }
//}



//// =================���ȍ~�́A�����܂��g���Ă��Ȃ� ========================



//// ContentsDatabase �ł� CatalogModel �� ContentModel ���Ǘ����Ă������A
//// AddressableWrapManager �ł� AddressableWrap ���Ǘ����銴��
//public class AddressableWrapManager : SavableSingleton<AddressableWrapManager>
//{
//    #region ====== Savable �̎d���� ================================================
//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
//    public override string SaveFolderPath { get; set; } = $"{Application.persistentDataPath}/DownloadContents";
//    public override string FileName { get; set; } = "AddressableWrapsManagementList";
//    #endregion =======================================


//    #region ====== �f�[�^ ================================================
//    public Dictionary<string, AddressableWrap> Wraps { get; set; } = new Dictionary<string, AddressableWrap>();
//    #endregion =======================================


//    void SaveCatalog(string AssetName, string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses, string saveFolderPath = "", string fileName = "")
//    {
//        if (Wraps.ContainsKey(AssetName))
//        {
//            Debug.LogWarning($"���L�̃J�^���O�͊��ɑ��݂���̂ŃZ�[�u�����Ȃ��ǂ��܂�\n{AssetName}");
//            return;
//        }

//        AddressableWrap wrap = new AddressableWrap(saveFolderPath, fileName)
//        {
//            AssetPath = assetPath,
//            CatalogFile = catalogFile,
//            HashFile = hashFile,
//            BundleFiles = bundleFiles,
//            Addresses = addresses,
//        };

//        wrap.Save();

//        Wraps.Add(AssetName, wrap);
//        Save();
//    }

//}




//// Addressable �̃��b�v�B�����Ɏg�������H
//public class AddressableWrap : Savable
//{
//    #region ====== Savable �̎d���� ================================================
//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
//    public override string SaveFolderPath { get; set; } = $"{Application.dataPath}/AddressableWraps";
//    #endregion =======================================


//    #region ====== Addressable�Ǘ��p�f�[�^ ================================================
//    // �J�^���O�A�n�b�V���A�o���h����3�_�������Ă���t�H���_�̃p�X
//    public string AssetPath { get; set; }
//    public string CatalogFile { get; set; }
//    public string HashFile { get; set; }
//    public List<string> BundleFiles { get; set; }
//    // �S Addressable �̃A�h���X
//    public List<string> Addresses { get; set; }
//    #endregion =======================================

//    // �����F�Z�[�u��̃p�X�A�Z�[�u����json�t�@�C����
//    public AddressableWrap(string saveFolderPath = "", string fileName = "")
//    {
//        SaveFolderPath = saveFolderPath;
//        FileName = fileName;
//    }


//    //public AddressableWrap(string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses)
//    //{
//    //    AssetPath = assetPath;
//    //    CatalogFile = catalogFile;
//    //    HashFile = hashFile;
//    //    BundleFiles = bundleFiles;
//    //    Addresses = addresses;
//    //}
//}








////public struct DownloadContentType
////{
////    public static string Prefab = "Prefab";
////    public static string Scene = "Scene";
////    public static string Motion = "Motion";
////}


////public enum AssetPurpose
////{
////    AdditionalContents,
////    DividedAssets
////}