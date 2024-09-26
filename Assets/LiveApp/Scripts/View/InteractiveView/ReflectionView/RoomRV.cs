using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static RuntimeData;
using static RuntimeState;

public class RoomRV : RV<RoomP>
{
    internal override GameObject ControllerObj { get { return GameObject.Find("Button_ImportRoom"); } set { } }


    protected sealed override async UniTask Awake1()
    {
        OnSpawnRoom.Subscribe(body => Spawn(body));
    }


    void Spawn(GameObject body)
    {
        body.transform.parent = Room.transform;
        body.transform.localScale = Vector3.one;
        body.transform.position = Room.transform.position;
        body.transform.rotation = Quaternion.Euler(0, 90, 0);
    }
}
