using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class CatalogElementDischargerCV : CV
{
    public override P Presenter { get; set; } = null;
    public override Type ControllerType { get; set; } = typeof(MyButton);

    [SerializeField] GameObject Dischargable;
    [SerializeField] Vector3 Position;

    protected sealed override async UniTask Awake1()
    {
        Controller.Clicked.Subscribe(value =>
        {
            if (value) Instantiate(Dischargable, Position, Quaternion.Euler(0, 180, 0));
        });
    }
}
