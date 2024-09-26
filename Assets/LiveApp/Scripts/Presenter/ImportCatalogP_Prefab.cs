//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using Cysharp.Threading.Tasks;
//using UnityEngine.ResourceManagement;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using static RuntimeData;
//using System.Linq;
//using UniRx;


//// �J�^���O���C���|�[�g����P�̃x�[�X
//public abstract class ImportCatalogP : P
//{
//    // �w�肵���R���e���g�^�C�v�̃f�[�^�x�[�X���Q��
//    public ContentsDatabase Database => ContentsDatabase.Databases[ContentType];
//    // �R���e���g�̎�ށB�v���n�u�Ȃ̂��A����Ȃ̂��A��
//    public abstract DownloadContentType ContentType { get; set; }
//    // �C���|�[�g�̎��s 
//    protected abstract UniTask ImportCatalog();
//    // �S�R���e���c�̃A�h���X�𒊏o
//    protected abstract UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator);


//    //public abstract string DisplayName { get; set; }
//    //protected abstract UniTask CreateCatalog(string catalogName);
//}



//// �v���n�u�R���e���g�̃J�^���O���C���|�[�g����P
//public class ImportCatalogP_Prefab : ImportCatalogP
//{
//    // �R���e���g�̎�ނ̓v���n�u�ł���
//    public override DownloadContentType ContentType { get; set; } = DownloadContentType.Prefab;

//    //public override string DisplayName { get; set; } = "Catalog_Prefab";


//    public override async UniTask Execute()
//    {
//        // ContentDatabase�ŁA�ۑ��ς݂̃R���e���g�����[�h
//        Database.Load();
//        // �J�^���O�̃C���|�[�g���s
//        await ImportCatalog();

//        //InputEventHandler.OnDown_S += async () => await DischargeContent("�R���e���g1");
//    }


//    protected override async UniTask ImportCatalog()
//    {
//        // �p�X�擾
//        // �G�N�X�v���[���[����J�^���O���蓮�őI��
//        string path = CatalogLoader.Execute();
//        //string path = @"C:\Users\osuim\Documents\Unity\Maku\Test_Addressable5\Library\com.unity.addressables\aa\Windows\StandaloneWindows64\catalog_0.1.0.json";
//        if (path == null) return;

//        // �J�^���O���������Ă���f�B���N�g�����̒��g�����v���W�F�N�g�̃t�H���_���ɕ���
//        DirectoryUtil.CopyParentFolder(path, AddressablesFolderPath);

//        // �p�X�̒�����J�^���O�̃t�@�C�����i�g���q���܂ށj�̕��������������
//        string catalogFile = Path.GetFileName(path);

//        // �J�^���O�����\�[�X���P�[�^�ɓo�^
//        IResourceLocator resourceLocator = await Database.RegisterCatalog(catalogFile);

//        // �ΏۂƂȂ�v���n�u�R���e���c�̃A�h���X�̈ꗗ���擾
//        List<string> loadedAddresses = await ExtractContentAdresses(resourceLocator);

//        // �J�^���O���Ƃ��̃J�^���O�ɕR�Â����[�h�\�ȃA�h���X�ꗗ��ۑ�
//        // �J�^���O���̓A�v�����œƎ��Ŗ����\�B�Ƃ�܁u�J�^���O1�v
//        Database.SaveCatalog("�J�^���O1", catalogFile, loadedAddresses);

//        // �Ώۂ̃R���e���c��S�ĕ\������J�^���O����o
//        //await CreateCatalog("�J�^���O1");
//    }



//    // �Y���R���e���c�̃A�h���X�ꗗ���擾
//    protected override async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator)
//    {
//        List<string> addresses = new List<string>();
//        List<string> loadedAddresses = new List<string>();

//        // �J�^���O(��������ł�IResourceLocator�Ƃ��Ĉ�����)����A�h���X�ꗗ�𒊏o
//        // IResourceLocator �� key(�ʏ��Adress)�ƁA����ɕR�Â��A�Z�b�g�̑Ή��֌W�̏��������Ă���
//        foreach (var a in resourceLocator.Keys)
//        {
//            addresses.Add(a.ToString());
//        }

//        // ���[�h�ł�����̂𔻕ʂ��āA���񃍁[�h�������^�C�v�̃A�Z�b�g�����ʂ��āA�S�����[�h
//        foreach (var a in addresses)
//        {
//            Debug.Log($"------ �A�h���X : {a}");
//            IList<IResourceLocation> resourceLocations;
//            // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
//            // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
//            // �܂�J�^���O�� IResourceLocation �����Ă���
//            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


//            foreach (var b in resourceLocations)
//            {
//                Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
//                // IResourceLocation �� PrimaryKey ���A�h���X�炵��
//                AsyncOperationHandle<GameObject> opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
//                await opHandle_LoadedGObj;
                
//                // ���[�h�ł��Ȃ������玟�̃��[�v��
//                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
//                Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

//                // �V�[�����̕��̂Ƃ��ẴQ�[���I�u�W�F�N�g(�܂�Renderer�R���|�[�l���g�����Ă���͂�)���ǂ����𔻕ʂ��āA
//                // ������玟�̃��[�v��
//                Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
//                if (renderers.Length == 0) continue;
//                Debug.Log($"------ �����_�������Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

//                // ����̃A�h���X���A���[�h�ς݂̃A�h���X�ꗗloadedAddresses�ɋL�^
//                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
//                loadedAddresses.Add(b.PrimaryKey);
//            }
//        }

//        return loadedAddresses;
//    }



//    //// �J�^���O����
//    //protected override async UniTask CreateCatalog(string catalogName)
//    //{
//    //    CatalogModel catalogModel = await Database.LoadCatalog(catalogName);

//    //    foreach (var a in catalogModel.Addresses)
//    //    {
//    //        GameObject gObj = await Addressables.InstantiateAsync(a);
//    //        gObj.transform.SetParent(Flexalon.transform);
//    //        CatalogContentController catalogContentController = gObj.AddComponent<CatalogContentController>();
//    //        catalogContentController.Address = a;
//    //        catalogContentController.CatalogFile = catalogModel.CatalogFile;
//    //        catalogContentController.ContentType = ContentTypes.Prefab;
            
//    //        // �R���g���[���ƃv���[���^�̐ڑ�
//    //        AddGeneratedUI(gObj, new ImportContentPresenter(catalogContentController));

//    //        // ���ɃC���|�[�g����Ă���R���e���g�ɂ̓}�[�J�[������
//    //        foreach (var b in Database.importedContents.Values)
//    //        {
//    //            if (b.Address != a) continue;
//    //            GameObject Marker_IsImported = await Addressables.InstantiateAsync("Marker_IsImported");
//    //            Marker_IsImported.transform.SetParent(gObj.transform);
//    //            Marker_IsImported.transform.localPosition = Vector3.zero;
//    //        }
//    //    }
//    //}
//}

