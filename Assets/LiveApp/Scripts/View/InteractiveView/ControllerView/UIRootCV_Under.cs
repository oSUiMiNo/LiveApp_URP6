using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UniRx;


public class UIRootCV_Under : CV
{
    GameObject mask;

    Vector3 maskInitialPos = new Vector3(0, -1.5f, 0);
    Vector3 maskActivePos = new Vector3(0, 0.35f, 0);

    [SerializeField]
    Ease ease;
    
    float easeTime = 1.5f;

    [SerializeField]
    public List<GameObject> branches = new List<GameObject>();

    public override P Presenter { get; set; } = new UIRootP();
    public override Type ControllerType { get; set; } = typeof(MyButton);

    UIRootP rootPresenter;

    protected override sealed async UniTask Awake1()
    {
        mask = transform.Find("Mask").gameObject;
        mask.transform.localPosition = maskInitialPos;
        //DebugView.Log($"uirootview 初期化");
        //InputEventHandler.OnDown_MouseLeft += () => DebugView.Log($"どこかしらクリックした2");
        Controller.Clicked.Subscribe(value => Presenter.Clicked.Value = value);

        Controller.On_Click.Subscribe(async _ => await Presenter.Execute());
        Controller.On_ClickMargin.Subscribe(async _ => await Presenter.Desist());
       
        rootPresenter = (UIRootP)Presenter;
        rootPresenter.OnActivateBranch.Subscribe(async _ => await ActivateAction());
        rootPresenter.OnInActivateBranch.Subscribe(async _ => await InActivateAction());

        await SetBranches();
    }


    async UniTask SetBranches()
    {
        foreach (var ui in GetComponentsInChildren<CV>())
        {
            if (ui.gameObject == gameObject) continue;
            await UniTask.WaitUntil(() => ui.Initialized.Value);
            branches.Add(ui.gameObject);
            rootPresenter.branches = branches;
        }
        branches.ForEach(a => a.SetActive(false));
    }


    async UniTask ActivateAction()
    {
        DebugView.Log($"アクティベート");
        //SetBranches(); // UIが同的に変化するので、初期化時に全部セット出来てない場合があるので毎回呼ぶ
        mask.transform.DOLocalMove(maskActivePos, easeTime).SetEase(ease);
        await UniTask.Delay(TimeSpan.FromSeconds(easeTime / 3));
        branches.ForEach(branch => branch.SetActive(true));
    }


    async UniTask InActivateAction()
    {
        DebugView.Log($"インアクティベート");
        //SetBranches(); // UIが同的に変化するので、初期化時に全部セット出来てない場合があるので毎回呼ぶ
        mask.transform.DOLocalMove(maskInitialPos, easeTime - 0.1f).SetEase(ease);
        await UniTask.Delay(TimeSpan.FromSeconds(easeTime));
        branches.ForEach(branch => branch.SetActive(false));
    }
}