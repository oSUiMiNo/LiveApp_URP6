using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ConnectVMCView : CV
{
    public override P Presenter { get; set; } = new VMCConnecterP();
    public override Type ControllerType { get; set; } = typeof(MyInputField);

    protected override sealed async UniTask Awake1()
    {
        MyInputField Controller = this.Controller as MyInputField;

        Controller.Clicked.Subscribe(value => Presenter.Clicked.Value = value);
        //Controller.Clicked.Subscribe(value => DebugView.Log($"{value}"));

        Controller._Text.Subscribe(async value =>
        {
            Presenter._Text.Value = value;
            await Presenter.Execute();
        });
    }
}
