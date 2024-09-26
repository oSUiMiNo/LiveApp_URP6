//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using static RuntimeData;
//using UniRx;
//using System;


////public abstract class CatalogGenerator : MyButton
////{
////    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
////    public GameObject Flexalon => GameObject.Find(DisplayName).transform.Find("Grid Layout").gameObject;
////    public abstract DownloadContentType DLContentType { get; set; }
////    public abstract string DisplayName { get; set; }
////    public abstract string CatalogName { get; set; }
////    protected abstract UniTask CreateCatalog(string catalogName);
////    protected sealed override void Awake1()
////    {
////        GetComponent<MyUI>().On_Click.Subscribe(async _ => await CreateCatalog(CatalogName));
////    }
////}



//// ボタンにアタッチしといて、クリックしたらカタログを生成するCV
////（Addressableのcatalogではなく、ユーザーがDLしたいコンテンツを選ぶためのカタログ的なUI）
//public abstract class CatalogGeneratorCV : CV
//{
//    // コントローラタイプを選択
//    public override Type ControllerType { get; set; } = typeof(MyButton);
//    // プレゼンタータイプを選択
//    public override P Presenter { get; set; } // なし？
    

//    // 何の種類のコンテンツのカタログなのか指定する
//    public abstract DownloadContentType DLContentType { get; set; }
//    // ContentsDatabase内に、コンテンツ種類ごとのデータベースインスタンスを管理する辞書がある
//    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
//    // なんだっけ表示するカタログの名前？
//    public abstract string DisplayName { get; set; }
//    // なんだっけカタログの本名？
//    public abstract string CatalogName { get; set; }
    

//    // DL前のコンテンツであるのゲームオブジェクト達をカタログとして並べるためのFlexalonをしんから取得
//    public GameObject Flexalon => GameObject.Find(DisplayName).transform.Find("Grid Layout").gameObject;
    

//    // カタログ生成の関数。子クラス（コンテンツの種類ごとに派生）似て実装
//    protected abstract UniTask CreateCatalog(string catalogName);


//    protected sealed override async UniTask Awake1()
//    {
//        // カタログ生成のコントローラをクリック時にカタログ生成を実行
//        Controller.On_Click.Subscribe(async _ => await CreateCatalog(CatalogName));
        
//        await Awake2();
//    }
//    protected virtual async UniTask Awake2() { }
//}



//// プレハブコンテンツのカタログを生成
//// 詳しくは親クラスを参照
//public class CatalogGeneratorCV_Prefab : CatalogGeneratorCV
//{
//    // オーバーライドするプロパティについては親クラスを参照されたし
//    public override DownloadContentType DLContentType { get; set; } = DownloadContentType.Prefab;
//    public override string DisplayName { get; set; } = "Catalog_Prefab";
//    public override string CatalogName { get; set; } = "カタログ1";


//    // カタログ生成
//    protected override async UniTask CreateCatalog(string catalogName)
//    {
//        // CatalogModel：Catalog ファイルと、その中のコンテンツアドレスたちを保有する
//        CatalogModel catalogModel = await Database.LoadCatalog(catalogName);

//        // とあるカタログモデルの、各アドレスを下に全てのコンテンツを生成していく
//        foreach (var a in catalogModel.Addresses)
//        {
//            // アドレスから生成
//            GameObject content = await Addressables.InstantiateAsync(a);
//            // 生成したコンテントをFlexalonの子オブジェクトにする 
//            content.transform.SetParent(Flexalon.transform);
//            // 
//            CatalogContentHeaderRV catalogContentCV = content.AddComponent<CatalogContentHeaderRV>();
//            catalogContentCV.Address = a;
//            catalogContentCV.CatalogFile = catalogModel.CatalogFile;
//            catalogContentCV.DLContentType = DownloadContentType.Prefab;

//            // コントローラとプレゼンタの接続
//            //UIPresenterBase.AddGeneratedUI(content, new ImportContentPresenter(catalogContentController));

//            // 既にインポートされているコンテントにはマーカーをつける
//            foreach (var b in Database.importedContents.Values)
//            {
//                if (b.Address != a) continue;
//                GameObject Marker_IsImported = await Addressables.InstantiateAsync("Marker_IsImported");
//                Marker_IsImported.transform.SetParent(content.transform);
//                Marker_IsImported.transform.localPosition = Vector3.zero;
//            }
//        }
//    }
//}
