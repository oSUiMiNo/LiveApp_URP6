using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using static RuntimeData;
using Cysharp.Threading.Tasks;


public class SizeAdjusterCV : CV
{
    public override P Presenter { get; set; } = new SizeAdjusterP();
    public override Type ControllerType { get; set; } = typeof(MySlider);

    protected sealed override async UniTask Awake1()
    {
        //DebugView.Log($"�R���g���[���@{Controller}");
        //DebugView.Log($"�R���g���[���̃Q�[���I�u�W�F�N�g {Controller.gameObject}");
        //DebugView.Log($"�v���[���^�x�[�X {UIPresenterBase.Children[Controller.gameObject]}");
        //DebugView.Log($"�v���[���^�@{Presenter}");

        //OnInitializedObservable.Subscribe(_ =>
        //{
            MySlider Controller = this.Controller as MySlider;
            Controller.Clicked.Subscribe(value => Presenter.Clicked.Value = value);

            Controller.Value_Float.Skip(1).Subscribe(async value =>
            {
                Presenter.value_Float.Value = value;
                await Presenter.Execute();
            });

            Controller.Value_Int.Skip(1).Subscribe(async value =>
            {
                Presenter.value_Int.Value = value;
                await Presenter.Execute();
            });

            Controller.Value_Float.SetValueAndForceNotify(Controller.Value_Float.Value);
            Controller.Value_Int.SetValueAndForceNotify(Controller.Value_Int.Value);
        //});

    }
}
