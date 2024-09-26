using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Newtonsoft.Json;
using System;
using NUnit.Framework.Internal;

public class CatalogImporter : MonoBehaviourMyExtention
{
    [SerializeField] GameObject catalogNamer;
    MyButton Controller => gameObject.AddComponent<MyButton>();

    [SerializeField] string BufferDir;
    [SerializeField] string ContentsDir;


    void Start()
    {
        //ContentsDir = @$"{Application.persistentDataPath}/DLContents";
        ContentsDir = DLContentsHandler.Data.SaveFolderPath;
        BufferDir = @$"{ContentsDir}/Buffer";


        Controller.On_Click.Subscribe(async _ =>
        {
            // �p�X�擾
            // �G�N�X�v���[���[����J�^���O���蓮�őI��
            string catalogPath = CatalogLoader.Execute();
            if (string.IsNullOrEmpty(catalogPath)) return;

            string catalogPath_New = await Duplicate(catalogPath, BufferDir);
            if (string.IsNullOrEmpty(catalogPath_New)) return;

            catalogNamer.SetActive(true);
            CatalogNamer namer = catalogNamer.GetComponent<CatalogNamer>();
            namer.BufferDir = BufferDir;
            namer.ContentsDir = ContentsDir;
        });

        Controller.On_Click.Subscribe(async _ =>
        {
            // �p�X�擾
            // �G�N�X�v���[���[����J�^���O���蓮�őI��
            string shijiPath = ShijiLoader.Execute();
            if (string.IsNullOrEmpty(shijiPath)) return;

            string hash = Path.GetFileNameWithoutExtension(shijiPath);
            // ���ɓ����n�b�V���̃f�[�^�����݂������荞�ݒ��~
            DLContentsHandler.Data.Load();
            foreach (var contentsCatalog in DLContentsHandler.Data.ContentsCatalogs.Values)
            {
                if (contentsCatalog.Hash == hash)
                {
                    Debug.LogAssertion($"���ɓ����J�^���O������܂�");
                    return;
                }
            }

            // .shiji�t�@�C����ǂݎ��A�f�V���A���C�Y����
            string shijiJson = File.ReadAllText(shijiPath);
            Shiji shiji = JsonConvert.DeserializeObject<Shiji>(shijiJson);

            await Decode(shiji, BufferDir);

            // Addresses��"|||"�ŕ��������X�g�ɕϊ�
            List<string> addressList = new List<string>(shiji.Addresses.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries));
            foreach (var address in addressList)
            {
                Debug.Log(address);
            }


            catalogNamer.SetActive(true);
            CatalogNamer namer = catalogNamer.GetComponent<CatalogNamer>();
            namer.BufferDir = BufferDir;
            namer.ContentsDir = ContentsDir;
        });
    }

    protected async UniTask Decode(Shiji shiji, string saveDir)
    {
        Directory.CreateDirectory(saveDir);
        // Hash�t�@�C�����쐬
        string hashFilePath = Path.Combine(saveDir, shiji.Hash.FileName);
        File.WriteAllText(hashFilePath, shiji.Hash.Content);
        Debug.Log($"Hash file created: {hashFilePath}");

        // Catalog�t�@�C�����쐬
        string catalogFilePath = Path.Combine(saveDir, shiji.Catalog.FileName);
        File.WriteAllText(catalogFilePath, shiji.Catalog.Content);
        Debug.Log($"Catalog file created: {catalogFilePath}");

        // Bundles�t�@�C�����쐬
        for (int i = 0; i < shiji.Bundles.Count; i++)
        {
            string bundleFilePath = Path.Combine(saveDir, shiji.Bundles[i].FileName);
            byte[] bundleContent = Convert.FromBase64String(shiji.Bundles[i].Content);
            File.WriteAllBytes(bundleFilePath, bundleContent);
            Debug.Log($"Bundle file created: {bundleFilePath}");
        }
    }

    // �o�b�t�@�p�t�H���_�ɕ���
    protected async UniTask<string> Duplicate(string orig, string dest)
    {
        Directory.CreateDirectory(dest);
        // �J�^���O���������Ă���f�B���N�g���̒��g�����v���W�F�N�g�̃t�H���_���ɕ���
        DirectoryUtil.CopyParentFolder(orig, dest);

        string newPath = $@"{dest}/{Path.GetFileName(orig)}";
        return newPath;
    }
}



// �G�N�X�v���[���[����A�C���|�[�g�i��PJT�ɕ������ăJ�^���O�Ƃ��ĕۑ��j������catalog���蓮�őI��
public class CatalogLoader
{
    public static string Execute()
    {
        string path = FileBrowser.SelectFilePath("json", "Select Catalog", @"C:\Users\osuim\Documents\Unity\Maku\ContentsCreator\Library\com.unity.addressables\aa\Windows\StandaloneWindows64");
        return path;
    }
}

public class ShijiLoader
{
    public static string Execute()
    {
        string path = FileBrowser.SelectFilePath("shiji", "Select Shiji", @"C:\Users\osuim\Documents\Unity\Maku\ContentsCreator\Assets\Publish\�e�X�g�R���e���c");
        return path;
    }
}
