//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using System.Xml.Serialization;
//using UnityEngine;
//using UniRx;
//using System;

//public class ImportContentP : P
//{
//    MyUI controller;
//    ContentsDatabase database;

//    public static IObservable<Unit> On_Click => On_Click;
//    static Subject<Unit> on_Click = new Subject<Unit>();

//    public ImportContentP(MyUI controller)
//    {
//        this.controller = controller;

//        controller.On_Click.Subscribe(async _ => await Execute());
//    }


//    public override async UniTask Execute()
//    {
//        on_Click.OnNext(Unit.Default);

//        //database = ContentsDatabase.Databases[controller.ContentType];
//        //await ImportContent(controller.ContentName, controller.CatalogFile, controller.Address);
//    }


//    public async UniTask ImportContent(string contentName, string catalogFile, string address)
//    {
//        database.SaveContent(contentName, catalogFile, address);

//        DebugView.Log($"インポートした {contentName} {catalogFile} {address}");
//        // ↓ビューへの処理書く
//    }
//}


