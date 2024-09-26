using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static RuntimeData;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;


// リファクタリングしてみたがテストはまだ
public class SizeAdjusterRV : RV<SizeAdjusterP>
{
    internal override GameObject ControllerObj 
    {
        set { } get
        {
            if (gameObject == _Avatar) return GameObject.Find("Slider_SizeAdjuster_Avatar");
            else
            if (gameObject == Room) return GameObject.Find("Slider_SizeAdjuster_Room");
            else return null;
        }
    }
    

    protected sealed override async UniTask Awake1()
    {

        await UniTask.WaitUntil(() => ControllerObj != null);

        //EditorbleInfo editorble = Presenter.SetEditorble(ControllerObj);
        EditorbleInfo editorble = Presenter.SetEditorble(gameObject);

        editorble.sizeMagnification.Subscribe(value => gameObject.transform.localScale = editorble.firstScale + Vector3.one * value);

        await Awake2();
    }
    protected virtual async UniTask Awake2() { }



    //async void Awake()
    //{
    //    //var token = this.GetCancellationTokenOnDestroy();
    //    //await UniTask.WaitUntil(() => slider != null, cancellationToken: token);
    //    //DebugView.Log($"{slider}");
                
    //    //DebugView.Log($"コントローラ　{Controller}");
    //    //DebugView.Log($"コントローラのゲームオブジェクト {Controller.gameObject}");
    //    //DebugView.Log($"プレゼンタベース {UIPresenterBase.Children[Controller.gameObject]}");
    //    //DebugView.Log($"プレゼンタ　{Presenter}");

    //    //Controller.Clicked.Subscribe(value => Presenter.Clicked.Value = value);

    //    //Controller.Value_Float.Skip(1).Subscribe(async value =>
    //    //{
    //    //    Presenter.value_Float.Value = value;
    //    //    await Presenter.Execute();
    //    //});

    //    //Controller.Value_Int.Skip(1).Subscribe(async value =>
    //    //{
    //    //    Presenter.value_Int.Value = value;
    //    //    await Presenter.Execute();
    //    //});

    //    await UniTask.WaitUntil(() => ControllerObj != null);
    //    //DebugView.Log($"コントローラのゲームオブジェクト {ControllerObj}");
    //    //DebugView.Log($"プレゼンタベース {UIPresenterBase.Children[ControllerObj]}");
    //    //DebugView.Log($"プレゼンタ　{Presenter}");

    //    //SizeAdjusterPresenter presenter = (SizeAdjusterPresenter)Presenter;
    //    EditorbleInfo editorble = Presenter.SetEditorble(gameObject);

    //    editorble.sizeMagnification.Subscribe(value => gameObject.transform.localScale = editorble.firstScale + Vector3.one * value);

    //    //Controller.Value_Float.SetValueAndForceNotify(Controller.Value_Float.Value);
    //    //Controller.Value_Int.SetValueAndForceNotify(Controller.Value_Int.Value);
    //}
}
