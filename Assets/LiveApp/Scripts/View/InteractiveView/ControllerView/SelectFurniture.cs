using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class SelectFurnitureCV : CV
{
    public override P Presenter { get; set; } = null;
    public override Type ControllerType { get; set; } = typeof(MyButton);

    GameObject objectTable;

    protected sealed override async UniTask Awake1()
    {
        objectTable = GameObject.Find("ObjectTable_Funiture");
        //objectTable.transform.position = new Vector3(0.5f, 0.2f, 14);

        Controller.Clicked.Subscribe(value =>
        {
            if(value) objectTable.SetActive(!objectTable.activeSelf);
        });
    }

    protected sealed override async void Start()
    {
        await Delay.Frame(1);
        
        //objectTable.SetActive(false);
    }
}
