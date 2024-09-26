using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System;
using System.IO;
using System.Net;

public class CatalogNamer : MonoBehaviour
{
    MyButton button => transform.Find("Decide").gameObject.AddComponent<MyButton>();
    MyInputField inputField;

    public string BufferDir;
    public string ContentsDir;
    public string ContentsName;


    private async void Awake()
    {
        inputField = transform.Find("InputField").gameObject.AddComponent<MyInputField>();

        button.On_Click.Subscribe( async _ => 
        {
            ContentsName = inputField.text_Input.text;
            if (string.IsNullOrEmpty(ContentsName)) return;

            

            await new ContentsDivider(BufferDir, ContentsDir, ContentsName).Execute();
            Debug.Log("�J�^���O�̎�荞�݊���");

            Finish();
        });
    }


    void Finish()
    {
        if (Directory.Exists(BufferDir)) Directory.Delete(BufferDir, true);
        inputField._Text.Value = string.Empty;
        gameObject.SetActive(false);
    }

    //// �Y���R���e���c�̃A�h���X�ꗗ���擾
    //public async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator)
    //{
    //    Addressables.AddResourceLocator(resourceLocator);

    //    List<string> addresses = new List<string>();
    //    List<string> loadedAddresses = new List<string>();

    //    // �J�^���O(��������ł�IResourceLocator�Ƃ��Ĉ�����)����A�h���X�ꗗ�𒊏o
    //    // IResourceLocator �� key(�ʏ��Adress)�ƁA����ɕR�Â��A�Z�b�g�̑Ή��֌W�̏��������Ă���
    //    foreach (var a in resourceLocator.Keys)
    //    {
    //        addresses.Add(a.ToString());
    //    }

    //    // ���[�h�ł�����̂𔻕ʂ��āA���񃍁[�h�������^�C�v�̃A�Z�b�g�����ʂ��āA�S�����[�h
    //    foreach (var a in addresses)
    //    {
    //        Debug.Log($"------ �A�h���X : {a}");
    //        IList<IResourceLocation> resourceLocations;
    //        // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
    //        // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
    //        // �܂�J�^���O�� IResourceLocation �����Ă���
    //        if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


    //        foreach (var b in resourceLocations)
    //        {
    //            Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
    //            // IResourceLocation �� PrimaryKey ���A�h���X�炵��
    //            AsyncOperationHandle<GameObject> opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
    //            await opHandle_LoadedGObj;

    //            // ���[�h�ł��Ȃ������玟�̃��[�v��
    //            if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
    //            Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

    //            // �V�[�����̕��̂Ƃ��ẴQ�[���I�u�W�F�N�g(�܂�Renderer�R���|�[�l���g�����Ă���͂�)���ǂ����𔻕ʂ��āA
    //            // ������玟�̃��[�v��
    //            Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
    //            if (renderers.Length == 0) continue;
    //            Debug.Log($"------ �����_�������Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

    //            // ����̃A�h���X���A���[�h�ς݂̃A�h���X�ꗗloadedAddresses�ɋL�^
    //            if (loadedAddresses.Contains(b.PrimaryKey)) continue;
    //            loadedAddresses.Add(b.PrimaryKey);
    //        }
    //    }

    //    Addressables.RemoveResourceLocator(resourceLocator);
    //    return loadedAddresses;
    //}
}



class ContentsDivider
{
    public string BufferDir;
    public string DLContentsDir;
    public string ContentsDir;
    public string ContentsName;

    public string AddressablesDir;
    public string CatalogName;
    public string HashName;
    public string Hash;


    // �A�Z�b�g�o���h���O���[�v�̃p�X���S�擾
    public List<string> GroupPaths;
    // �t�H���_�U�蕪�����Catalog�̃p�X
    public string CatalogPath;

    public List<string> labels = new List<string>
    {
        "����",
        "�A�o�^�[",
        "���[�V����",
        "��",
        "�Ƌ�"
    };

    public ContentsDivider(string bufferDir, string contentsDir, string contentsName)
    {
        // �o���h���̕ۑ���
        AddressablesDir = $@"{Addressables.RuntimePath}/StandaloneWindows64";
        // �R���e���c�̖��O
        ContentsName = contentsName;
        // �o�b�t�@�̃p�X
        BufferDir = bufferDir;
        // �J�^���O�ƃn�b�V���̕ۑ���
        DLContentsDir = contentsDir;
        // �R���e���g�̃f�B���N�g��
        ContentsDir = @$"{contentsDir}/{contentsName}";
        // �J�^���O�̃t�@�C�����i�g���q���܂ށj
        CatalogName = Directory.GetFiles(bufferDir, $"*.json")[0].Replace($"{bufferDir}\\", "");
        // �n�b�V���̃t�@�C�����i�g���q���܂ށj
        HashName = Directory.GetFiles(bufferDir, $"*.hash")[0].Replace($"{bufferDir}\\", "");
        // �n�b�V���̒��g
        Hash = File.ReadAllText(@$"{BufferDir}/{HashName}");
    }


    public async UniTask Execute()
    {
        //// ���ɓ����n�b�V���̃f�[�^�����݂������荞�ݒ��~
        //DLContentsHandler.Data.Load();
        //foreach (var contentsCatalog in DLContentsHandler.Data.ContentsCatalogs.Values)
        //{
        //    if (contentsCatalog.Hash == Hash)
        //    {
        //        Debug.LogAssertion($"���ɓ����J�^���O������܂�");
        //        return;
        //    }
        //}

        DivideFolder(ContentsName);

        CatalogPath = @$"{DLContentsDir}/{ContentsName}/{CatalogName}";

        // ���\�[�X���P�[�^�i�擾�����J�^���O���f�V���A���C�Y���ꂽ���́j���擾
        IResourceLocator resourceLocator = await GetLocator(@$"{CatalogPath}");
        // �A�Z�b�g�o���h���O���[�v�̃p�X���S�擾
        GroupPaths = await ExtractGroupPaths(resourceLocator);
        //// �ΏۂƂȂ�v���n�u�R���e���c�̃A�h���X�̈ꗗ���擾
        //Addresses_Room = await ExtractContentAdresses(resourceLocator);

        Catalog catalog = new Catalog()
        {
            Hash = Hash,
            Path_Catalog = CatalogPath,
            Paths_Group = GroupPaths,
        };

        foreach (var label in labels)
        {
            List<string> addressList = new List<string>();
            List<string> list = new List<string>();
            if (label == "����" || label == "�Ƌ�")
            {
                addressList = await AddressExtracter.Go(resourceLocator, label);
            }
            if (label == "�A�o�^�[")
            {
                addressList = await AddressExtracter.Txt(resourceLocator, label);

                Addressables.AddResourceLocator(resourceLocator);
                for (int a = 0; a < addressList.Count; a++)
                {
                    string path = @$"{ContentsDir}\Avatar{a}.bytes";
                    TextAsset vRMAsBytes = await Addressables.LoadAssetAsync<TextAsset>(addressList[a]);
                    // TextAsset�̃o�C�g�f�[�^���擾
                    byte[] byteArray = vRMAsBytes.bytes;

                    // �w�肳�ꂽ�p�X��.bytes�t�@�C���Ƃ��ĕۑ�
                    File.WriteAllBytes(path, byteArray);
                    addressList[a] = path;
                    //await LoadVRMFromBytes.Execute(path);
                    //File.Delete(path);
                }
                Addressables.RemoveResourceLocator(resourceLocator);
            }
            if (label == "��")
            {
                addressList = await AddressExtracter.Sky(resourceLocator, label);
            }

            Addresses addresses = new Addresses()
            {
                BeforeDL = addressList,
                AfterDL = new List<string>(),
            };
            catalog.Labels.Add(label, addresses);
        }


        DLContentsHandler.Data.ContentsCatalogs.Add(ContentsName, catalog);
        DLContentsHandler.Data.Save();
        DLContentsHandler.Data.Load();
    }


    void DivideFolder(string contentsName)
    {
        // �R���e���g�f�B���N�g���쐬
        Directory.CreateDirectory(ContentsDir);

        // Catalog���R���e���g�f�B���N�g���Ɉڂ�
        File.Copy(@$"{BufferDir}/{CatalogName}", @$"{ContentsDir}/{CatalogName}", true);
        File.Delete($@"{BufferDir}/{CatalogName}");

        File.Copy(@$"{BufferDir}/{HashName}", @$"{ContentsDir}/{HashName}", true);
        File.Delete($@"{BufferDir}/{HashName}");

        Debug.Log($"�A�h���b�T�u���t�H���_ {AddressablesDir}");
        DirectoryUtil.CopyParentFolder(BufferDir, AddressablesDir);
        Directory.Delete(BufferDir, true);
    }



    // �g�������J�^���O�����\�[�X���P�[�^�ɓo�^
    public async UniTask<IResourceLocator> GetLocator(string catalogPath)
    {
        // �V�����J�^���O���擾�B�t�@�C���p�X��URL
        AsyncOperationHandle<IResourceLocator> requestCatalog
            = Addressables.LoadContentCatalogAsync(catalogPath);

        // ���[�h������҂�
        await requestCatalog;

        // ���̃G���[��������
        Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);

        // ���\�[�X���P�[�^�i�擾�����J�^���O���f�V���A���C�Y���ꂽ���́j��Ԃ�
        return requestCatalog.Result;
    }


    //// �Y���R���e���c�̃A�h���X�ꗗ���擾
    //public async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator, string label)
    //{
    //    Addressables.AddResourceLocator(resourceLocator);

    //    List<string> addresses = new List<string>();
    //    List<string> loadedAddresses = new List<string>();

    //    //Addressables.LoadAssetsAsync<GameObject>(label, null,)
    //    AsyncOperationHandle<IList<GameObject>> handle_LabelGobj = new AsyncOperationHandle<IList<GameObject>>();
    //    try
    //    {
    //        handle_LabelGobj = Addressables.LoadAssetsAsync<GameObject>(label, null);
    //        await handle_LabelGobj;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log($"���̃J�^���O�Ƀ��x�� {label} �͖���");
    //        return new List<string>();
    //    }


    //    // �J�^���O(��������ł�IResourceLocator�Ƃ��Ĉ�����)����A�h���X�ꗗ�𒊏o
    //    // IResourceLocator �� key(�ʏ��Adress)�ƁA����ɕR�Â��A�Z�b�g�̑Ή��֌W�̏��������Ă���
    //    foreach (var a in resourceLocator.Keys)
    //    {
    //        addresses.Add(a.ToString());
    //    }

    //    // ���[�h�ł�����̂𔻕ʂ��āA���񃍁[�h�������^�C�v�̃A�Z�b�g�����ʂ��āA�S�����[�h
    //    foreach (var a in addresses)
    //    {
    //        Debug.Log($"------ �A�h���X : {a}");
    //        IList<IResourceLocation> resourceLocations;
    //        // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
    //        // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
    //        // �܂�J�^���O�� IResourceLocation �����Ă���
    //        if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


    //        foreach (var b in resourceLocations)
    //        {
    //            Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
    //            // IResourceLocation �� PrimaryKey ���A�h���X�炵��

    //            AsyncOperationHandle<GameObject> opHandle_LoadedGObj = new AsyncOperationHandle<GameObject>();
    //            try
    //            {
    //                opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
    //                await opHandle_LoadedGObj;
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.Log($"{b.PrimaryKey} �̓��[�h�ł��Ȃ�����");
    //                continue;
    //            }

    //            // ����̃��x���Ɋ܂܂�Ă���Q�[���I�u�W�F�N�g���ǂ���
    //            bool containedInLabel = false;
    //            foreach (GameObject labelGobj in handle_LabelGobj.Result)
    //            {
    //                if (opHandle_LoadedGObj.Result == labelGobj)
    //                {
    //                    Debug.Log($"���x���Ɋ܂܂�Ă��� {opHandle_LoadedGObj.Result}");
    //                    containedInLabel = true;
    //                    continue;
    //                }
    //            }
    //            if (containedInLabel == false) continue;

    //            // ���[�h�ł��Ȃ������玟�̃��[�v��
    //            if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
    //            //Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

    //            // �V�[�����̕��̂Ƃ��ẴQ�[���I�u�W�F�N�g(�܂�Renderer�R���|�[�l���g�����Ă���͂�)���ǂ����𔻕ʂ��āA
    //            // ������玟�̃��[�v��
    //            Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
    //            if (renderers.Length == 0) continue;
    //            //Debug.Log($"------ �����_�������Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

    //            // ���Ƀ��[�h�ς݂̃A�h���X�Ɋ܂܂�Ă����玟�̃��[�v��
    //            if (loadedAddresses.Contains(b.PrimaryKey)) continue;
    //            // ����̃A�h���X���A���[�h�ς݂̃A�h���X�ꗗloadedAddresses�ɋL�^
    //            loadedAddresses.Add(b.PrimaryKey);
    //        }
    //    }

    //    Addressables.RemoveResourceLocator(resourceLocator);
    //    return loadedAddresses;
    //}



    public async UniTask<List<string>> ExtractGroupPaths(IResourceLocator resourceLocator)
    {
        List<string> groupPaths = new List<string>();
        Debug.Log($"===============================================");

        foreach (var a in resourceLocator.Keys)
        {
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
            // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
            // �܂�J�^���O�� IResourceLocation �����Ă���

            // typeof()�̒��g�́A�A�v����DL�A�Z�b�g�Ƃ��ċ����^
            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;
            Debug.Log($"�A�h���X {a}");

        }

        // �J�^���O�Ɋ܂܂��S�A�Z�b�g�o���h���O���[�v�̃p�X��؂�o��
        foreach (var a in resourceLocator.Keys)
        {
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
            // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
            // �܂�J�^���O�� IResourceLocation �����Ă���

            // typeof()�̒��g�́A�A�v����DL�A�Z�b�g�Ƃ��ċ����^
            if (resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) break;
            if (resourceLocator.Locate(a, typeof(Animator), out resourceLocations)) break;
            if (resourceLocator.Locate(a, typeof(Skybox), out resourceLocations)) break;

            // �O���[�v�̃p�X��Keys�̏��ՂɏW�܂��Ă��āA���̌�� address�B��������ۂ�
            // �Ȃ̂ŁALocate(Keys) �� true �ɂȂ�܂Łi�܂�address�̑O�܂Łj�� Keys ��؂�o���΃O���[�v�̃p�X�̃��X�g������
            groupPaths.Add($"{AddressablesDir}/{a.ToString()}");
            Debug.Log($"------- �O���[�v�p�X {AddressablesDir}/{a.ToString()}");
        }

        return groupPaths;
    }
}



public class AddressExtracter
{
    // �Y���R���e���c�̃A�h���X�ꗗ���擾
    public static async UniTask<List<string>> Go(IResourceLocator resourceLocator, string label)
    {
        Addressables.AddResourceLocator(resourceLocator);

        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();

        AsyncOperationHandle<IList<GameObject>> handle_LabelGobj = new AsyncOperationHandle<IList<GameObject>>();
        try
        {
            handle_LabelGobj = Addressables.LoadAssetsAsync<GameObject>(label, null);
            await handle_LabelGobj;
        }
        catch (Exception e)
        {
            Debug.Log($"���̃J�^���O�Ƀ��x�� {label} �͖���");
            return new List<string>();
        }


        // �J�^���O(��������ł�IResourceLocator�Ƃ��Ĉ�����)����A�h���X�ꗗ�𒊏o
        // IResourceLocator �� key(�ʏ��Adress)�ƁA����ɕR�Â��A�Z�b�g�̑Ή��֌W�̏��������Ă���
        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        // ���[�h�ł�����̂𔻕ʂ��āA���񃍁[�h�������^�C�v�̃A�Z�b�g�����ʂ��āA�S�����[�h
        foreach (var a in addresses)
        {
            Debug.Log($"------ �A�h���X : {a}");
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
            // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
            // �܂�J�^���O�� IResourceLocation �����Ă���
            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
                // IResourceLocation �� PrimaryKey ���A�h���X�炵��

                AsyncOperationHandle<GameObject> opHandle_LoadedGObj = new AsyncOperationHandle<GameObject>();
                try
                {
                    opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
                    await opHandle_LoadedGObj;
                }
                catch (Exception e)
                {
                    //Debug.Log($"{b.PrimaryKey} �̓��[�h�ł��Ȃ�����");
                    continue;
                }

                // ����̃��x���Ɋ܂܂�Ă���Q�[���I�u�W�F�N�g���ǂ���
                bool containedInLabel = false;
                foreach (GameObject labelGobj in handle_LabelGobj.Result)
                {
                    if (opHandle_LoadedGObj.Result == labelGobj)
                    {
                        Debug.Log($"���x���Ɋ܂܂�Ă��� {opHandle_LoadedGObj.Result}");
                        containedInLabel = true;
                        continue;
                    }
                }
                if (containedInLabel == false) continue;

                // ���[�h�ł��Ȃ������玟�̃��[�v��
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
                //Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

                // �V�[�����̕��̂Ƃ��ẴQ�[���I�u�W�F�N�g(�܂�Renderer�R���|�[�l���g�����Ă���͂�)���ǂ����𔻕ʂ��āA
                // ������玟�̃��[�v��
                Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) continue;
                //Debug.Log($"------ �����_�������Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

                // ���Ƀ��[�h�ς݂̃A�h���X�Ɋ܂܂�Ă����玟�̃��[�v��
                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                // ����̃A�h���X���A���[�h�ς݂̃A�h���X�ꗗloadedAddresses�ɋL�^
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        Addressables.RemoveResourceLocator(resourceLocator);
        return loadedAddresses;
    }

    // �Y���R���e���c�̃A�h���X�ꗗ���擾
    public static async UniTask<List<string>> Txt(IResourceLocator resourceLocator, string label)
    {
        Addressables.AddResourceLocator(resourceLocator);

        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();

        AsyncOperationHandle<IList<TextAsset>> handle_LabelGobj = new AsyncOperationHandle<IList<TextAsset>>();
        try
        {
            handle_LabelGobj = Addressables.LoadAssetsAsync<TextAsset>(label, null);
            await handle_LabelGobj;
        }
        catch (Exception e)
        {
            Debug.Log($"���̃J�^���O�Ƀ��x�� {label} �͖���");
            return new List<string>();
        }


        // �J�^���O(��������ł�IResourceLocator�Ƃ��Ĉ�����)����A�h���X�ꗗ�𒊏o
        // IResourceLocator �� key(�ʏ��Adress)�ƁA����ɕR�Â��A�Z�b�g�̑Ή��֌W�̏��������Ă���
        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        // ���[�h�ł�����̂𔻕ʂ��āA���񃍁[�h�������^�C�v�̃A�Z�b�g�����ʂ��āA�S�����[�h
        foreach (var a in addresses)
        {
            Debug.Log($"------ �A�h���X : {a}");
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
            // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
            // �܂�J�^���O�� IResourceLocation �����Ă���
            if (!resourceLocator.Locate(a, typeof(TextAsset), out resourceLocations)) continue;


            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
                // IResourceLocation �� PrimaryKey ���A�h���X�炵��

                AsyncOperationHandle<TextAsset> opHandle_LoadedGObj = new AsyncOperationHandle<TextAsset>();
                try
                {
                    opHandle_LoadedGObj = Addressables.LoadAssetAsync<TextAsset>(b.PrimaryKey);
                    await opHandle_LoadedGObj;
                }
                catch (Exception e)
                {
                    Debug.Log($"{b.PrimaryKey} �̓��[�h�ł��Ȃ�����");
                    continue;
                }

                // ����̃��x���Ɋ܂܂�Ă���Q�[���I�u�W�F�N�g���ǂ���
                bool containedInLabel = false;
                foreach (TextAsset labelGobj in handle_LabelGobj.Result)
                {
                    if (opHandle_LoadedGObj.Result == labelGobj)
                    {
                        Debug.Log($"���x���Ɋ܂܂�Ă��� {opHandle_LoadedGObj.Result}");
                        containedInLabel = true;
                        continue;
                    }
                }
                if (containedInLabel == false) continue;

                // ���[�h�ł��Ȃ������玟�̃��[�v��
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
                //Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

                // ���Ƀ��[�h�ς݂̃A�h���X�Ɋ܂܂�Ă����玟�̃��[�v��
                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                // ����̃A�h���X���A���[�h�ς݂̃A�h���X�ꗗloadedAddresses�ɋL�^
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        Addressables.RemoveResourceLocator(resourceLocator);
        return loadedAddresses;
    }

    // �Y���R���e���c�̃A�h���X�ꗗ���擾
    public static async UniTask<List<string>> Sky(IResourceLocator resourceLocator, string label)
    {
        Addressables.AddResourceLocator(resourceLocator);

        List<string> addresses = new List<string>();
        List<string> loadedAddresses = new List<string>();

        AsyncOperationHandle<IList<Material>> handle_LabelGobj = new AsyncOperationHandle<IList<Material>>();
        try
        {
            handle_LabelGobj = Addressables.LoadAssetsAsync<Material>(label, null);
            await handle_LabelGobj;
        }
        catch (Exception e)
        {
            Debug.Log($"���̃J�^���O�Ƀ��x�� {label} �͖���");
            return new List<string>();
        }


        // �J�^���O(��������ł�IResourceLocator�Ƃ��Ĉ�����)����A�h���X�ꗗ�𒊏o
        // IResourceLocator �� key(�ʏ��Adress)�ƁA����ɕR�Â��A�Z�b�g�̑Ή��֌W�̏��������Ă���
        foreach (var a in resourceLocator.Keys)
        {
            addresses.Add(a.ToString());
        }

        // ���[�h�ł�����̂𔻕ʂ��āA���񃍁[�h�������^�C�v�̃A�Z�b�g�����ʂ��āA�S�����[�h
        foreach (var a in addresses)
        {
            Debug.Log($"------ �A�h���X : {a}");
            IList<IResourceLocation> resourceLocations;
            // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
            // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
            // �܂�J�^���O�� IResourceLocation �����Ă���
            if (!resourceLocator.Locate(a, typeof(Material), out resourceLocations)) continue;


            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- �v���C�}���[�L�[ {b.PrimaryKey}");
                // IResourceLocation �� PrimaryKey ���A�h���X�炵��

                AsyncOperationHandle<Material> opHandle_LoadedGObj = new AsyncOperationHandle<Material>();
                try
                {
                    opHandle_LoadedGObj = Addressables.LoadAssetAsync<Material>(b.PrimaryKey);
                    await opHandle_LoadedGObj;
                }
                catch (Exception e)
                {
                    Debug.Log($"{b.PrimaryKey} �̓��[�h�ł��Ȃ�����");
                    continue;
                }

                // ����̃��x���Ɋ܂܂�Ă���Q�[���I�u�W�F�N�g���ǂ���
                bool containedInLabel = false;
                foreach (Material labelGobj in handle_LabelGobj.Result)
                {
                    if (opHandle_LoadedGObj.Result == labelGobj)
                    {
                        Debug.Log($"���x���Ɋ܂܂�Ă��� {opHandle_LoadedGObj.Result}");
                        containedInLabel = true;
                        continue;
                    }
                }
                if (containedInLabel == false) continue;

                // ���[�h�ł��Ȃ������玟�̃��[�v��
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
                //Debug.Log($"------ ���[�h�����Q�[���I�u�W�F�N�g : {opHandle_LoadedGObj.Result}");

                // ���Ƀ��[�h�ς݂̃A�h���X�Ɋ܂܂�Ă����玟�̃��[�v��
                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                // ����̃A�h���X���A���[�h�ς݂̃A�h���X�ꗗloadedAddresses�ɋL�^
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        Addressables.RemoveResourceLocator(resourceLocator);
        return loadedAddresses;
    }
}