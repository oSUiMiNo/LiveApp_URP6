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


//// コンテントの種類
//public enum DownloadContentType
//{
//    Prefab,
//    Scene,
//    Motion,
//}



//// 取り込んだcatalogと、calatlogを元に作ったカタログからDLしたコンテントのデータベースをJsonとして管理する
//public class ContentsDatabase : Savable
//{
//    #region ====== Savable の仕込み ================================================
//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
//    public override string SaveFolderPath { get; set; } = "DownloadContents";　// DBのjsonを保存するパス
//    #endregion =======================================


//    #region ====== データ ================================================
//    // CatalogModel：Catalog ファイルと、その中のコンテンツアドレスたちを保有する

//    // インポート済みのCatalogModel一覧。key：ユーザーが命名したコンテント名
//    public Dictionary<string, ContentModel> importedContents { get; set; } = new Dictionary<string, ContentModel>();
    
//    // カタログに表示するコンテンツ一覧。key：ユーザーが命名したカタログ名
//    public Dictionary<string, CatalogModel> catalogContents{ get; set; } = new Dictionary<string, CatalogModel>();
//    #endregion =======================================


//    // コンテンツ種類ごとのDBインスタンスを管理する辞書
//    public static Dictionary<DownloadContentType, ContentsDatabase> Databases { get; set; } = new Dictionary<DownloadContentType, ContentsDatabase>()
//    {
//        { DownloadContentType.Prefab, new ContentsDatabase($"{Application.dataPath}/DownloadContents", "Prefabs") },
//        { DownloadContentType.Scene, new ContentsDatabase($"{Application.dataPath}/DownloadContents", "Scenes") },
//        { DownloadContentType.Motion, new ContentsDatabase($"{Application.dataPath}/DownloadContents", "Motions") },
//    };


//    // コンストラクタ 
//    // 引数：DBのjsonを保存するパス、DBのjsonファイル名
//    public ContentsDatabase(string saveFolderPath = "", string fileName = "")
//    {
//        SaveFolderPath = saveFolderPath;
//        FileName = fileName;
//    }


//    // DBの辞書catalogContentsにカタログをセーブ
//    // 引数：任意のカタログ名、 Catalogファイル、カタログに表示するコンテンツのアドレス達
//    public void SaveCatalog(string catalogName, string catalogFile, List<string> addresses)
//    {
//        if (catalogContents.ContainsKey(catalogName))
//        {
//            return;
//        }

//        catalogContents.Add(catalogName, new CatalogModel(catalogFile, addresses));
//        Save();
//    }


//    // DBの辞書catalogContentsにカタログをロード
//    // 引数：ロードしたいカタログの名前（ユーザーが命名）
//    public async UniTask<CatalogModel> LoadCatalog(string catalogName)
//    {
//        Load();
//        CatalogModel catalogModel = catalogContents[catalogName];
//        if (!catalogContents.ContainsKey(catalogName))
//        {
//            Debug.LogWarning($"{catalogName} というカタログはありません。インポートしてください。");
//            return null;
//        }
//        await RegisterCatalog(catalogModel.CatalogFile);
//        return catalogModel;
//    }


//    // カタログからユーザーがDLしたコンテントをセーブ（importedContentsに追加してSave）
//    // 引数：コンテントの名前（ユーザーが命名）、コンテントが所属するcatalogファイル、コンテントのAdress
//    public void SaveContent(string contentName, string catalogFile, string address)
//    {
//        if (importedContents.ContainsKey(contentName))
//        {
//            Debug.LogWarning($"下記のコンテントは既に存在するのでセーブをしないどきます\n{contentName}");
//            return;
//        }
//        importedContents.Add(contentName, new ContentModel(catalogFile, address));
//        Save();
//    }


//    // DL済みのプレハブコンテントをロード
//    // 引数：コンテントの名前（ユーザーが命名）
//    public async UniTask<GameObject> LoadConetent_Prefab(string contentName)
//    {
//        Load();
//        ContentModel contentModel = importedContents[contentName];
//        if (!importedContents.ContainsKey(contentName))
//        {
//            Debug.LogWarning($"{contentName} というコンテンツはありません。インポートしてください。");
//            return null;
//        }
//        await RegisterCatalog(contentModel.CatalogFile);
//        GameObject content = await Addressables.LoadAssetAsync<GameObject>(contentModel.Address);

//        return content;
//    }


//    // DL済みの全コンテンツをロード。ユーザーが管理する用のテーブルを生成する際のやつ
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


//    // 使いたいカタログをリソースロケータに登録
//    public async UniTask<IResourceLocator> RegisterCatalog(string catalogFile)
//    {
//        // 新しいカタログを取得。ファイルパスかURL
//        AsyncOperationHandle<IResourceLocator> requestCatalog
//            = Addressables.LoadContentCatalogAsync(GetCataLogPath(catalogFile));

//        // ロード完了を待つ
//        await requestCatalog;

//        // 何のエラー処理だろ
//        Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);

//        // リソースロケータにカタログが登録されていない場合は追加
//        if (Addressables.ResourceLocators.Contains(requestCatalog.Result)) Addressables.AddResourceLocator(requestCatalog.Result);
//        else Debug.LogWarning($"以下のカタログはリソースロケータに既に登録されているものでした\n{requestCatalog.Result}");

//        // リソースロケータ（取得したカタログがデシリアライズされたもの）を返す
//        return requestCatalog.Result;
//    }


//    // データベースに登録されている全カタログを一気にリソースロケータに登録
//    public async UniTask RegisterAllCatalogs()
//    {
//        List<AsyncOperationHandle<IResourceLocator>> requestCatalogs
//            = new List<AsyncOperationHandle<IResourceLocator>>();

//        List<IResourceLocator> resourceLocators
//           = new List<IResourceLocator>();

//        // 1つ1つのロード完了を待たず、とりま全部ロードを走らせてバッファに追加していく
//        foreach (var a in importedContents)
//        {
//            requestCatalogs.Add(Addressables.LoadContentCatalogAsync(GetCataLogPath(a.Value.CatalogFile)));
//        }

//        foreach (var a in requestCatalogs)
//        {
//            // ロード完了を待つ
//            await a;

//            // ロードに失敗していたら停止
//            Assert.AreEqual(AsyncOperationStatus.Succeeded, a.Status);

//            // リソースロケータにカタログを追加
//            if (Addressables.ResourceLocators.Contains(a.Result)) Addressables.AddResourceLocator(a.Result);
//            else Debug.LogWarning($"以下のカタログはリソースロケータに既に登録されているものでした\n{a.Result}");

//            // リソースロケータ群（取得したカタログがデシリアライズされたもの）を返す
//            resourceLocators.Add(a.Result);
//        }
//    }


//    // 本プロジェクトに複製済みのcatalogファイルのパスを取得
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



//// これのインスタンス１につき１つのコンテントを表現する。
//// コンテントのAdressと、そのコンテントが所属するcatalogファイルのパスを保有するデータクラス
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



//// これのインスタンス１につき１つのカタログを表現する。
//// CatalogModel：とある Catalogファイルのパスと、その中のコンテンツAdressたちを保有する
//public class CatalogModel
//{
//    public string CatalogFile { get; set; }
//    public List<string> Addresses { get; set; }

//    // .bundle ファイルや .hash ファイルのパスもいづれ記録するようにする

//    public CatalogModel(string catalogFile, List<string> addresses)
//    {
//        CatalogFile = catalogFile;
//        Addresses = addresses;
//    }
//}



//// =================↓以降は、多分まだ使っていない ========================



//// ContentsDatabase では CatalogModel と ContentModel を管理していたが、
//// AddressableWrapManager では AddressableWrap を管理する感じ
//public class AddressableWrapManager : SavableSingleton<AddressableWrapManager>
//{
//    #region ====== Savable の仕込み ================================================
//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
//    public override string SaveFolderPath { get; set; } = $"{Application.persistentDataPath}/DownloadContents";
//    public override string FileName { get; set; } = "AddressableWrapsManagementList";
//    #endregion =======================================


//    #region ====== データ ================================================
//    public Dictionary<string, AddressableWrap> Wraps { get; set; } = new Dictionary<string, AddressableWrap>();
//    #endregion =======================================


//    void SaveCatalog(string AssetName, string assetPath, string catalogFile, string hashFile, List<string> bundleFiles, List<string> addresses, string saveFolderPath = "", string fileName = "")
//    {
//        if (Wraps.ContainsKey(AssetName))
//        {
//            Debug.LogWarning($"下記のカタログは既に存在するのでセーブをしないどきます\n{AssetName}");
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




//// Addressable のラップ。何処に使うっけ？
//public class AddressableWrap : Savable
//{
//    #region ====== Savable の仕込み ================================================
//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
//    public override string SaveFolderPath { get; set; } = $"{Application.dataPath}/AddressableWraps";
//    #endregion =======================================


//    #region ====== Addressable管理用データ ================================================
//    // カタログ、ハッシュ、バンドルの3点が入っているフォルダのパス
//    public string AssetPath { get; set; }
//    public string CatalogFile { get; set; }
//    public string HashFile { get; set; }
//    public List<string> BundleFiles { get; set; }
//    // 全 Addressable のアドレス
//    public List<string> Addresses { get; set; }
//    #endregion =======================================

//    // 引数：セーブ先のパス、セーブ時のjsonファイル名
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