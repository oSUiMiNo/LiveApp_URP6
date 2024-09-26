using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static RuntimeData;
using static RuntimeState;

public class RoomP : P
{
    public override async UniTask Execute()
    {
        Debug.Log("ロードルームプレゼンター");
        GameObject body = await RoomLoader.Execute();
        if (body == null) return;

        onSpawnRoom.OnNext(body);

        if (RoomBodies.Count != 0) body.SetActive(false);
        
        RoomBodies.Add(body);

        onLoadRoom.OnNext(Unit.Default);　// イベントをObservableに置き換えてみた
    }
}