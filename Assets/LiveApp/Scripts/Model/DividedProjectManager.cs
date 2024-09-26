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


    #region ====== データ ================================================
    public Dictionary<string, DividedAssetModel> Catalogs = new Dictionary<string, DividedAssetModel>();
    #endregion =======================================

    // データベースに登録されている全カタログを一気にリソースロケータに登録
    public async UniTask RegisterAllCatalogs()
    {
        List<AsyncOperationHandle<IResourceLocator>> requestCatalogs
            = new List<AsyncOperationHandle<IResourceLocator>>();

        List<IResourceLocator> resourceLocators
           = new List<IResourceLocator>();

        // 1つ1つのロード完了を待たず、とりま全部ロードを走らせてバッファに追加していく
        foreach (var a in Catalogs)
        {
            requestCatalogs.Add(Addressables.LoadContentCatalogAsync(GetCataLogPath(a.Value.CatalogFile)));
        }

        foreach (var a in requestCatalogs)
        {
            // ロード完了を待つ
            await a;

            // ロードに失敗していたら停止
            Assert.AreEqual(AsyncOperationStatus.Succeeded, a.Status);

            // リソースロケータにカタログを追加
            if (Addressables.ResourceLocators.Contains(a.Result)) Addressables.AddResourceLocator(a.Result);
            else Debug.LogWarning($"以下のカタログはリソースロケータに既に登録されているものでした\n{a.Result}");

            // リソースロケータ群（取得したカタログがデシリアライズされたもの）を返す
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
            // カタログ、ハッシュ、バンドルの3点が入っているフォルダの名前をアセット名とする
            string assetName = a.FullName;

            // フォルダの中からカタログファイルを探す
            List<string> paths = Directory.EnumerateFiles(DividedAssetsFolderPath, "*.json").ToList();
            if (paths.Count > 1) Debug.LogError($"カタログがいっぱいある！");
            string path = paths[0];

            // パスの中からカタログのファイル名（拡張子を含む）の部分だけ抜き取り
            string catalogFile = Path.GetFileName(path);

            // カタログ、ハッシュ、バンドルの3点が入っているフォルダのパスを抽出
            string assetPath = path.Replace(catalogFile, string.Empty);

            // ハッシュファイルの名前を取得
            string hashFile = catalogFile.Replace("json", "hash");

            // カタログをロード
            AsyncOperationHandle<IResourceLocator> requestCatalog
            = Addressables.LoadContentCatalogAsync(GetCataLogPath(catalogFile));
            await requestCatalog;

            // アッセットバンドル本体のファイル名を取得
            List<string> bundleFiles = await ExtractBundleFile(requestCatalog.Result);

            // 対象となるプレハブコンテンツのアドレスの一覧を取得
            List<string> addresses = await ExtractContentAdresses(requestCatalog.Result);

            // カタログ名とそのカタログに紐づくロード可能なアドレス一覧を保存
            SaveCatalog(assetName, assetPath, catalogFile, hashFile, bundleFiles, addresses);
        }
    }





    void SaveCatalog(string AssetName, string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses)
    {
        if (Catalogs.ContainsKey(AssetName))
        {
            Debug.LogWarning($"下記のカタログは既に存在するのでセーブをしないどきます\n{AssetName}");
            return;
        }

        Catalogs.Add(AssetName, new DividedAssetModel(assetPath, catalogFile, hashFile, bundleFiles, addresses));
        Save();
    }



    // 該当コンテンツのアドレス一覧を取得
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
            Debug.Log($"------ アドレス : {a}");
            IList<IResourceLocation> resourceLocations;
            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;

            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
                // バンドルファイルの名前を抽出
                if (b.PrimaryKey.Contains(".bundle"))
                {
                    // パスの中からカタログのファイル名（拡張子を含む）の部分だけ抜き取ってリストに追加
                    bundleFiles.Add(Path.GetFileName(b.PrimaryKey));
                    continue;
                }

                // アドレスを抽出
                AsyncOperationHandle<GameObject> opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
                await opHandle_LoadedGObj;
                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;

                Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

                Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) continue;

                Debug.Log($"------ レンダラを持つゲームオブジェクト : {opHandle_LoadedGObj.Result}");

                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
                loadedAddresses.Add(b.PrimaryKey);
            }
        }

        return loadedAddresses;
    }

    // アッセットバンドル本体のファイル名を取得
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
            Debug.Log($"------ アドレス : {a}");
            IList<IResourceLocation> resourceLocations;
            if (!resourceLocator.Locate(a, typeof(AssetBundle), out resourceLocations)) continue;

            foreach (var b in resourceLocations)
            {
                Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
                if (!b.PrimaryKey.Contains(".bundle")) continue;

                // パスの中からカタログのファイル名（拡張子を含む）の部分だけ抜き取り
                string bundleFile = Path.GetFileName(b.PrimaryKey);

                string bundleFolderPath = b.PrimaryKey.Replace(bundleFile, string.Empty);
                //b.PrimaryKey = bundleFolderPath;



                // バンドルファイルの名前をリストに追加
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

    // .bundle ファイルや .hash ファイルのパスもいづれ記録するようにする

    public DividedAssetModel(string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses)
    {
        AssetJsonPath = assetPath;
        CatalogFile = catalogFile;
        HashFile = hashFile;
        BundleFiles = bundleFiles;
        Addresses = addresses;
    }
}