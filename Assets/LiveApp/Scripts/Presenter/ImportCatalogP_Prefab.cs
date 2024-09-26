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


//// カタログをインポートするPのベース
//public abstract class ImportCatalogP : P
//{
//    // 指定したコンテントタイプのデータベースを参照
//    public ContentsDatabase Database => ContentsDatabase.Databases[ContentType];
//    // コンテントの種類。プレハブなのか、動画なのか、等
//    public abstract DownloadContentType ContentType { get; set; }
//    // インポートの実行 
//    protected abstract UniTask ImportCatalog();
//    // 全コンテンツのアドレスを抽出
//    protected abstract UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator);


//    //public abstract string DisplayName { get; set; }
//    //protected abstract UniTask CreateCatalog(string catalogName);
//}



//// プレハブコンテントのカタログをインポートするP
//public class ImportCatalogP_Prefab : ImportCatalogP
//{
//    // コンテントの種類はプレハブですよ
//    public override DownloadContentType ContentType { get; set; } = DownloadContentType.Prefab;

//    //public override string DisplayName { get; set; } = "Catalog_Prefab";


//    public override async UniTask Execute()
//    {
//        // ContentDatabaseで、保存済みのコンテントをロード
//        Database.Load();
//        // カタログのインポート実行
//        await ImportCatalog();

//        //InputEventHandler.OnDown_S += async () => await DischargeContent("コンテント1");
//    }


//    protected override async UniTask ImportCatalog()
//    {
//        // パス取得
//        // エクスプローラーからカタログを手動で選ぶ
//        string path = CatalogLoader.Execute();
//        //string path = @"C:\Users\osuim\Documents\Unity\Maku\Test_Addressable5\Library\com.unity.addressables\aa\Windows\StandaloneWindows64\catalog_0.1.0.json";
//        if (path == null) return;

//        // カタログ等が入っているディレクトリをの中身を自プロジェクトのフォルダ内に複製
//        DirectoryUtil.CopyParentFolder(path, AddressablesFolderPath);

//        // パスの中からカタログのファイル名（拡張子を含む）の部分だけ抜き取り
//        string catalogFile = Path.GetFileName(path);

//        // カタログをリソースロケータに登録
//        IResourceLocator resourceLocator = await Database.RegisterCatalog(catalogFile);

//        // 対象となるプレハブコンテンツのアドレスの一覧を取得
//        List<string> loadedAddresses = await ExtractContentAdresses(resourceLocator);

//        // カタログ名とそのカタログに紐づくロード可能なアドレス一覧を保存
//        // カタログ名はアプリ内で独自で命名可能。とりま「カタログ1」
//        Database.SaveCatalog("カタログ1", catalogFile, loadedAddresses);

//        // 対象のコンテンツを全て表示するカタログを放出
//        //await CreateCatalog("カタログ1");
//    }



//    // 該当コンテンツのアドレス一覧を取得
//    protected override async UniTask<List<string>> ExtractContentAdresses(IResourceLocator resourceLocator)
//    {
//        List<string> addresses = new List<string>();
//        List<string> loadedAddresses = new List<string>();

//        // カタログ(メモリ上ではIResourceLocatorとして扱われる)からアドレス一覧を抽出
//        // IResourceLocator は key(通常はAdress)と、それに紐づくアセットの対応関係の情報を持っている
//        foreach (var a in resourceLocator.Keys)
//        {
//            addresses.Add(a.ToString());
//        }

//        // ロードできるものを判別して、今回ロードしたいタイプのアセットか判別して、全部ロード
//        foreach (var a in addresses)
//        {
//            Debug.Log($"------ アドレス : {a}");
//            IList<IResourceLocation> resourceLocations;
//            // IResourceLocation はアセットをロードするために必要な情報を持っている
//            // IResourceLocator の Locate() に特定のアドレスを渡すと IResourceLocation が返ってくる
//            // つまりカタログが IResourceLocation を内包している
//            if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;


//            foreach (var b in resourceLocations)
//            {
//                Debug.Log($"------- プライマリーキー {b.PrimaryKey}");
//                // IResourceLocation の PrimaryKey がアドレスらしい
//                AsyncOperationHandle<GameObject> opHandle_LoadedGObj = Addressables.LoadAssetAsync<GameObject>(b.PrimaryKey);
//                await opHandle_LoadedGObj;
                
//                // ロードできなかったら次のループへ
//                if (opHandle_LoadedGObj.Status != AsyncOperationStatus.Succeeded) continue;
//                Debug.Log($"------ ロードしたゲームオブジェクト : {opHandle_LoadedGObj.Result}");

//                // シーン内の物体としてのゲームオブジェクト(つまりRendererコンポーネントがついているはず)かどうかを判別して、
//                // 違ったら次のループへ
//                Renderer[] renderers = opHandle_LoadedGObj.Result.GetComponentsInChildren<Renderer>();
//                if (renderers.Length == 0) continue;
//                Debug.Log($"------ レンダラを持つゲームオブジェクト : {opHandle_LoadedGObj.Result}");

//                // 今回のアドレスを、ロード済みのアドレス一覧loadedAddressesに記録
//                if (loadedAddresses.Contains(b.PrimaryKey)) continue;
//                loadedAddresses.Add(b.PrimaryKey);
//            }
//        }

//        return loadedAddresses;
//    }



//    //// カタログ生成
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
            
//    //        // コントローラとプレゼンタの接続
//    //        AddGeneratedUI(gObj, new ImportContentPresenter(catalogContentController));

//    //        // 既にインポートされているコンテントにはマーカーをつける
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

