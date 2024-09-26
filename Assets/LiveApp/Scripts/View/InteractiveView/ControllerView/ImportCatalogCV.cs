//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UniRx;
//using System;
//using Cysharp.Threading.Tasks; 

//public class ImportCatalogCV : CV
//{
//    public override P Presenter { get; set; } = new ImportCatalogP_Prefab();
//    public override Type ControllerType { get; set; } = typeof(MyButton);


//    protected sealed override async UniTask Awake1()
//    {
//        Controller.Clicked.Subscribe(value => Presenter.Clicked.Value = value);

//        Controller.On_Click.Subscribe(async _ => await Presenter.Execute());
//        Controller.On_ClickMargin.Subscribe(async _ => await Presenter.Desist());
//    }
//}
