using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class ImportRoomCV : CV
{
    public override P Presenter { get; set; } = new RoomP();
    public override Type ControllerType { get; set; } = typeof(MyButton);
    protected sealed override async UniTask Awake1()
    {
        Controller.Clicked.Subscribe(value => Presenter.Clicked.Value = value);

        Controller.On_Click.Subscribe(async _ => await Presenter.Execute());
        
        Controller.On_ClickMargin.Subscribe(async _ => await Presenter.Desist());
    }
}
