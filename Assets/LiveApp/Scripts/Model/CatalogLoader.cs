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


// エクスプローラーから、インポート（自PJTに複製してカタログとして保存）したいcatalogを手動で選択
public class CatalogLoader
{
    public static string Execute()
    {
        string path = FileBrowser.SelectFilePath("json", "Select Catalog", @"C:\Users\osuim\Documents\Unity\Maku\Test_Addressable5\Library\com.unity.addressables\aa\Windows\StandaloneWindows64");
        return path;
    }
}