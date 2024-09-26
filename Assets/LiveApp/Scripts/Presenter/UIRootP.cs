using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class UIRootP : P
{
    public List<GameObject> branches = new List<GameObject>();
    bool branchIsActive = false;


    // このクラスは何度も再利用するので、イベントは、
    // RuntaimeStateではなくPのインスタンスが持つようにした
    public IObservable<Unit> OnActivateBranch => onActivateBranch;
    internal Subject<Unit> onActivateBranch = new Subject<Unit>();

    public IObservable<Unit> OnInActivateBranch => onInActivateBranch;
    internal Subject<Unit> onInActivateBranch = new Subject<Unit>();


    public override async UniTask Execute()
    {
        if (branches.Count == 0) return;
        foreach (var branch in branches) if (Children[branch].Clicked.Value) return;
        branchIsActive = true;
        onActivateBranch.OnNext(Unit.Default);
    }


    public override async UniTask Desist()
    {
        if (!branchIsActive) return;
        foreach (var branch in branches) if (Children[branch].Clicked.Value) return;
        branchIsActive = false;
        onInActivateBranch.OnNext(Unit.Default);
    }
}
