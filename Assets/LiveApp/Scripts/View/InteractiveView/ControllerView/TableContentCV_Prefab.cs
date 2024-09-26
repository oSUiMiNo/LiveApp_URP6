//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UniRx;
//using System;


//// =================↓以降は、多分まだ使っていない ========================



//public abstract class TableContentCV : CV
//{
//    // コントローラタイプを選択
//    public override Type ControllerType { get; set; } = typeof(MyButton);
//    // プレゼンタータイプを選択
//    public override P Presenter { get; set; } // なし？


//    // 何の種類のコンテンツのテーブルなのか指定する
//    public abstract DownloadContentType DLContentType { get; set; }
//    // ContentsDatabase内に、コンテンツ種類ごとのデータベースインスタンスを管理する辞書がある
//    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
//    // コンテントの名前？なんで？
//    public string ContentName => gameObject.name;
    
    
//    protected abstract UniTask DischargeOwnContent();
//    protected sealed override async UniTask Awake1()
//    {
//        //GetComponent<MyUI>().On_Click.Subscribe(async _ => await DischargeOwnContent());
//    }
//}



//public class TableContentCV_Prefab : TableContentCV
//{
//    public override DownloadContentType DLContentType { get; set; } = DownloadContentType.Prefab;


//    // 所有しているコンテントを放出
//    protected override async UniTask DischargeOwnContent()
//    {
//        GameObject content = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
//        Destroy(content.GetComponent<MyUI>());
//        Destroy(content.GetComponent<TableContentCV_Prefab>());
//    }
//}