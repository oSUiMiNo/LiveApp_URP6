using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using static RuntimeData;
using System.Linq;


// �G�N�X�v���[���[����A�C���|�[�g�i��PJT�ɕ������ăJ�^���O�Ƃ��ĕۑ��j������catalog���蓮�őI��
public class CatalogLoader
{
    public static string Execute()
    {
        string path = FileBrowser.SelectFilePath("json", "Select Catalog", @"C:\Users\osuim\Documents\Unity\Maku\Test_Addressable5\Library\com.unity.addressables\aa\Windows\StandaloneWindows64");
        return path;
    }
}