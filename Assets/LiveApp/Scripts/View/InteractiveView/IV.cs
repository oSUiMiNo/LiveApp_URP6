using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using static RuntimeData;
using UnityEngine.EventSystems;
using UniRx;
using System.Windows.Forms;


// Interective View
public abstract class IV : SealableMonoBehaviourMyExtention
{
    public BoolReactiveProperty StartInitialize { get; set; } = new BoolReactiveProperty(false);
    public BoolReactiveProperty Initialized { get; set; } = new BoolReactiveProperty(false);
    //public bool Initialized { get; set; } = false;

    protected sealed async override void Awake()
    {
        await UniTask.WaitUntil(() => ViewSetter.Initialized.Value);
        StartInitialize.Value = true;

        //await UniTask.WaitUntil(() => RuntimeDataInitialized);
        await Awake0();
 
        Initialized.Value = true; // Awake0()より後
        //DebugView.Log($"インタラクティブビュー初期化 {gameObject}");
    }
    protected virtual async UniTask Awake0() { }

}



// Controller View
public abstract class CV : IV
{
    public abstract Type ControllerType { get; set; }
    public MyUI Controller => GetComponent<MyUI>();
    public abstract P Presenter { get; set; }

    protected sealed override async UniTask Awake0()
    {
        //Pを管理辞書に登録
        P.Children.Add(gameObject, Presenter);
        //自オブジェクトにCをアタッチ
        if (ControllerType != null) CheckAddComponent(ControllerType, gameObject);
        
        await Awake1();
    }
    protected virtual async UniTask Awake1() { }
}




// Reflection View
// <T>に使いたいPインスタンスの型を入れることで、変数Presenter に、UIPresenterBaseからダウンキャストされた具体的なプレゼンタインスタンスが入り、「Presenter.」で派生型特有のメンバにアクセスできるようになる。
public abstract class RV<T> : IV where T : P
{
    internal abstract GameObject ControllerObj { get; set; }  // 自rVオブジェクトのCとなっているオブジェクトを代入する。ViewSetter.ApplySlider() からで設定する仕様にしてある
    protected MyUI Controller => ControllerObj.GetComponent<MyUI>(); // CオブジェクトからCを取得
    protected T Presenter => (T)P.Children[ControllerObj]; // 派生型にダウンキャストしたPインスタンスを管理辞書から取得

     
    protected sealed override async UniTask Awake0()
    {   
        await Awake1();
    }
    protected virtual async UniTask Awake1() { }
}

