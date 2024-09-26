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


//public abstract class TableGenerator : MyButton
//{
//    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
//    public GameObject Flexalon => GameObject.Find(DisplayName).transform.Find("Grid Layout").gameObject;
//    public abstract DownloadContentType DLContentType { get; set; }
//    public abstract string DisplayName { get; set; }
//    protected abstract UniTask CreateTable();
//    protected sealed override void Awake1()
//    {
//        On_Click.Subscribe(async _ => await CreateTable());
//    }
//}



//public class TableGenerator_Prefab : TableGenerator
//{
//    public override DownloadContentType DLContentType { get; set; } = DownloadContentType.Prefab;
//    public override string DisplayName { get; set; } = "Table_Prefab";

//    // カタログ生成

//    // 所有している全コンテンツを放出
//    protected override async UniTask CreateTable()
//    {
//        List<GameObject> contents = await Database.LoadAllConetents_Prefab();
//        foreach (var a in contents)
//        {
//            if (a == null)
//            {
//                Debug.LogWarning($"おかしなコンテントが含まれています。");
//                continue;
//            }
//            GameObject content = Instantiate(a);
//            content.transform.SetParent(Flexalon.transform);
//            content.AddComponent<MyButton>();
//            content.AddComponent<TableContentCV_Prefab>();
//        }
//    }
//}
