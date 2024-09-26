using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using static RuntimeData;
using System.Linq;
using UnityEditor;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.IO;


public class DividedProjectManager : Savable
{
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    public override string SaveFolderPath { get; set; } = $"{Application.dataPath}/DividedAssets";


    #region ====== �f�[�^ ================================================
    public Dictionary<string, DividedAssetModel> Catalogs = new Dictionary<string, DividedAssetModel>();
    #endregion =======================================

    // �f�[�^�x�[�X�ɓo�^����Ă���S�J�^���O����C�Ƀ��\�[�X���P�[�^�ɓo�^
    public async UniTask RegisterAllCatalogs()
    {
        List<AsyncOperationHandle<IResourceLocator>> requestCatalogs
            = new List<AsyncOperationHandle<IResourceLocator>>();

        List<IResourceLocator> resourceLocators
           = new List<IResourceLocator>();

        // 1��1�̃��[�h������҂����A�Ƃ�ܑS�����[�h�𑖂点�ăo�b�t�@�ɒǉ����Ă���
        foreach (var a in Catalogs)
        {
            requestCatalogs.Add(Addressables.LoadContentCatalogAsync(GetCataLogPath(a.Value.CatalogFile)));
        }

        foreach (var a in requestCatalogs)
        {
            // ���[�h������҂�
            await a;

            // ���[�h�Ɏ��s���Ă������~
            Assert.AreEqual(AsyncOperationStatus.Succeeded, a.Status);

            // ���\�[�X���P�[�^�ɃJ�^���O��ǉ�
            if (Addressables.ResourceLocators.Contains(a.Result)) Addressables.AddResourceLocator(a.Result);
            else Debug.LogWarning($"�ȉ��̃J�^���O�̓��\�[�X���P�[�^�Ɋ��ɓo�^����Ă�����̂ł���\n{a.Result}");

            // ���\�[�X���P�[�^�Q�i�擾�����J�^���O���f�V���A���C�Y���ꂽ���́j��Ԃ�
            resourceLocators.Add(a.Result);
        }
    }


    string GetCataLogPath(string catalogFile)
    {
        return $"{DividedAssetsFolderPath}/{catalogFile}";
    }


    //[MenuItem("MyTool / Add DividedInternalAddressable" , false, 1)]
    async UniTask Import()
    {
        DirectoryInfo dir = new DirectoryInfo(DividedAssetsFolderPath);
        DirectoryInfo[] dirs = dir.GetDirectories();


        foreach (var a in dirs)
        {
            // �J�^���O�A�n�b�V���A�o���h����3�_�������Ă���t�H���_�̖��O���A�Z�b�g���Ƃ���
            string assetName = a.FullName;

            // �t�H���_�̒�����J�^���O�t�@�C����T��
            List<string> paths = Directory.EnumerateFiles(DividedAssetsFolderPath, "*.json").ToList();
            if (paths.Count > 1) Debug.LogError($"�J�^���O�������ς�����I");
            string path = paths[0];

            // �p�X�̒�����J�^���O�̃t�@�C�����i�g���q���܂ށj�̕��������������
            string catalogFile = Path.GetFileName(path);

            // �J�^���O�A�n�b�V���A�o���h����3�_�������Ă���t�H���_�̃p�X�𒊏o
            string assetPath = path.Replace(catalogFile, string.Empty);

            // �n�b�V���t�@�C���̖��O���擾
            string hashFile = catalogFile.Replace("json", "hash");

            // �J�^���O�����[�h
            AsyncOperationHandle<IResourceLocator> requestCatalog
            = Addressables.LoadContentCatalogAsync(GetCataLogPath(catalogFile));
            await requestCatalog;

            // �A�b�Z�b�g�o���h���{�̂̃t�@�C�������擾
            List<string> bundleFiles = await ExtractBundleFile(requestCatalog.Result);

            // �ΏۂƂȂ�v���n�u�R���e���c�̃A�h���X�̈ꗗ���擾
            List<string> addresses = await ExtractContentAdresses(requestCatalog.Result);

            // �J�^���O���Ƃ��̃J�^���O�ɕR�Â����[�h�\�ȃA�h���X�ꗗ��ۑ�
            SaveCatalog(assetName, assetPath, catalogFile, hashFile, bundleFiles, addresses);
        }
    }





    void SaveCatalog(string AssetName, string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses)
    {
        if (Catalogs.ContainsKey(AssetName))
        {
            Debug.LogWarning($"���L�̃J�^���O�͊��ɑ��݂���̂ŃZ�[�u�����Ȃ��ǂ��܂�\n{AssetName}");
            return;
        }

        Catalogs.Add(AssetName, new DividedAssetModel(assetPath, catalogFile, hashFile, bundleFiles, addresses));
        Save();
    }



    // �Y���R���e���c�̃A�h���X�ꗗ���擾
    async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator)
    {
        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();
        List<string> bundleFiles = new List<string>();

        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        foreach (var a in addresses)
        {
            Debug.Log($"------ �A�h���X : {a}");
            IList<IResourceLocation> resourceLocations;
            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;

            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
                // �o���h���t�@�C���̖��O�𒊏o
                if (b.PrimaryKey.Contains(".bundle"))
                {
                    // �p�X�̒�����J�^���O�̃t�@�C�����i�g���q���܂ށj�̕���������������ă��X�g�ɒǉ�
                    bundleFiles.Add(Path.GetFileName(b.PrimaryKey));
                    continue;
                }

                // �A�h���X�𒊏o
                AsyncOperationHandle<GameObject> opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
                await opHandle_LoadedGObj;
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;

                Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

                Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) continue;

                Debug.Log($"------ �����_�������Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        return loadedAddresses;
    }

    // �A�b�Z�b�g�o���h���{�̂̃t�@�C�������擾
    async UniTask<List<string>> ExtractBundleFile(IResourceLocator resourceLocator)
    {
        List<string> addresses = new List<string>();
        List<string> bundleFiles = new List<string>();

        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        foreach (var a in addresses)
        {
            Debug.Log($"------ �A�h���X : {a}");
            IList<IResourceLocation> resourceLocations;
            if (!resourceLocator.Locate(a, typeof(AssetBundle), out resourceLocations)) continue;

            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
                if (!b.PrimaryKey.Contains(".bundle")) continue;

                // �p�X�̒�����J�^���O�̃t�@�C�����i�g���q���܂ށj�̕��������������
                string bundleFile = Path.GetFileName(b.PrimaryKey);

                string bundleFolderPath = b.PrimaryKey.Replace(bundleFile, string.Empty);
                //b.PrimaryKey = bundleFolderPath;



                // �o���h���t�@�C���̖��O�����X�g�ɒǉ�
                bundleFiles.Add(bundleFile);
            }
        }

        return bundleFiles;
    }
}







public class DividedAssetModel
{
    public string AssetJsonPath { get; }
    public string CatalogFile { get; }
    public string HashFile { get; }
    public List<string> BundleFiles { get; }
    public List<string> Addresses { get; }

    // .bundle �t�@�C���� .hash �t�@�C���̃p�X�����Â�L�^����悤�ɂ���

    public DividedAssetModel(string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses)
    {
        AssetJsonPath = assetPath;
        CatalogFile = catalogFile;
        HashFile = hashFile;
        BundleFiles = bundleFiles;
        Addresses = addresses;
    }
}