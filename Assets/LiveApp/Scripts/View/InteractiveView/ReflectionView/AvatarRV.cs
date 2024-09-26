using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static RuntimeData;
using static RuntimeState;
using static RuntimeInputP;
using static VMCState;
using UnityEngine.AddressableAssets;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;


public class AvatarRV : RV<AvartarP>
{
    internal override GameObject ControllerObj { get { return GameObject.Find("Button_ImportAvatar"); } set { } }

    static float easeTime = 4f;

    public RuntimeAnimatorController AvatarController;
    public RuntimeAnimatorController CutInAvatarController;
    public GameObject Eff;
    public GameObject FireWorks;

    // 初回演出完了フラグ
    bool FirstDirectionFinished = false;

    // スポーン時の、ローカルポジション（親オブジェクトAvatarからどれだけずらすか）
    Vector3 spawnLocalPos = new Vector3(-2, 0, 0);



    protected sealed override async UniTask Awake1()
    {
        AvatarController = await Addressables.LoadAssetAsync<RuntimeAnimatorController>("AvatarController");
        CutInAvatarController = await Addressables.LoadAssetAsync<RuntimeAnimatorController>("CutInAvatarController");
        Eff = Instantiate(await Addressables.LoadAssetAsync<GameObject>("Eff_star01"));
        Eff.SetActive(false);
      
        OnSpawnAvatar.Subscribe(body =>
        {
            Spawn(body);
        });

        OnLoadFirstAvatar.Subscribe(async body =>
        {
            SW_Caputure_Animation(Connecting.Value, body);
            Direction_First(body);
        });

        OnLoadAvatar.Subscribe(body =>
        {
            Connecting.Subscribe(isConnecting =>
            {
                SW_Caputure_Animation(isConnecting, body);
            });
        });

        OnSW_Avatar.Subscribe(index =>
        {
            Direction_SW(index);
        });
    }


    // スポーン
    void Spawn(GameObject body)
    {
        // 3DモデルをAvatarの子オブジェクトにする
        body.transform.parent = _Avatar.transform;
        // Avatarに対してのローカルポジションを0にする
        body.transform.localPosition = Vector3.zero;
        // Avatarに対してのローカルスケールを1にする。仮
        body.transform.localScale = Vector3.one;

        body.SetActive(false);
    }


    // 初回演出
    async void Direction_First(GameObject body)
    {
        _Avatar.transform.Find("AvatarUI").gameObject.SetActive(false);

        if (!Connecting.Value) await FirstAnim(body);
        else
        {
            _WebCam.GetComponent<WebCamPV>().Fade(1.2f, 2.5f, 3);
            await Delay.Second(1.2f);
            body.SetActive(true);
        }

        FirstDirectionFinished = true;
    }


    // アバターのスイッチと演出
    public async void Direction_SW(int index)
    {
        NowAvatarBodyNum = index;
        GameObject body = AvatarBodies[NowAvatarBodyNum];

        // カットイン
        CutIn(body);
        
        await Delay.Second(1.2f);
        // モーキャプとアニメーションをスイッチ
        SW_Caputure_Animation(Connecting.Value, body);

        // Body切り替え
        Receiver.Model = body;
        AvatarBodies.ForEach(a => a.SetActive(false));
        body.SetActive(true);
    }


    async void CutIn(GameObject body)
    {
        // バーチャルカメラの照準をカットインアバターに合わせる
        vWebCam_VirtualCamera.Follow = CutInAvatar.transform;
        vWebCam_VirtualCamera.LookAt = CutInAvatar.transform;

        await Delay.Second(0.1f);
        GameObject cutInBody = Instantiate(body);
        cutInBody.transform.parent = CutInAvatar.transform;
        cutInBody.transform.localPosition = Vector3.zero;
        cutInBody.transform.localScale = Vector3.one;
        cutInBody.SetActive(true);

        // 花火
        GameObject fireWorks = Instantiate(await Addressables.LoadAssetAsync<GameObject>("FireWorks"));
        fireWorks .transform.parent = CutInAvatar.transform;
        fireWorks.transform.localPosition = new Vector3(0, -1, -6);
        
        // 花火の音
        AudioSource[] audioSources = fireWorks.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.clip.name == "打ち上げ花火1") PlaySound(audioSource, 3);
            if (audioSource.clip.name == "花火大会") PlaySound(audioSource, 1);
        }

        // エフェクト再生
        Eff.SetActive(true);
        Eff.transform.position = CutInAvatar.transform.position;

        await Delay.Second(1.5f);
        // アニメータをセット
        Animator animator = cutInBody.GetComponent<Animator>();
        animator.runtimeAnimatorController = CutInAvatarController;
        animator.SetInteger("selection", new System.Random().Next(1, 4));

        await Delay.Second(3);
        // エフェクト停止
        Eff.SetActive(false);

        await Delay.Second(6);
        // フェード
        if (FirstDirectionFinished) _WebCam.GetComponent<WebCamPV>().Fade(1, 2.2f, 0.3f);
        
        await Delay.Second(1);
        vWebCam_VirtualCamera.Follow = _Avatar.transform;
        vWebCam_VirtualCamera.LookAt = _Avatar.transform;

        await Delay.Second(1);
        Destroy(cutInBody);

        foreach (AudioSource audioSource in audioSources) DOFade(audioSource, 0, 3);

        await Delay.Second(3);
        Destroy(fireWorks);
    }

    // 何故か audioSource.DOFade() が使えないので
    // DOTweenModuleAudioクラスから抜いてきた
    public static TweenerCore<float, float, FloatOptions> DOFade(AudioSource target, float endValue, float duration)
    {
        if (endValue < 0) endValue = 0;
        else if (endValue > 1) endValue = 1;
        TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.volume, x => target.volume = x, endValue, duration);
        t.SetTarget(target);
        return t;
    }



    async void PlaySound(AudioSource audioSource, float delay)
    {
        await Delay.Second(delay);
        audioSource.Play();
    }



    // 初回アニメーション
    async UniTask FirstAnim(GameObject body)
    {
        // 初期位置はAvatarから少しずらした位置に配置。演出のため
        body.transform.localPosition = spawnLocalPos;
        // 初期の向き決め。今のところ、この後の演出のために横に向かせている
        body.transform.localRotation = Quaternion.Euler(0, 90, 0);

        body.SetActive(true);

        Animator animator = body.GetComponent<Animator>();
        animator.runtimeAnimatorController = AvatarController;

        animator.SetInteger("FirstState", 1);
        body.transform.DOLocalMove(_Avatar.transform.position, easeTime);
        
        await Delay.Second(easeTime - 2);
        body.transform.DORotate(new Vector3(0, -90, 0), 2.5f, RotateMode.LocalAxisAdd);
        
        await Delay.Second(2 - 0.8f);
        animator.SetInteger("FirstState", 2);
        
        await Delay.Second(5);
        animator.SetInteger("FirstState", 0);

        FirstDirectionFinished = true;

        IdleAnim(body);
    }


    // アイドリングアニメーション
    public async void IdleAnim(GameObject body)
    {
        Animator animator = body.GetComponent<Animator>();
        animator.applyRootMotion = true;
        animator.runtimeAnimatorController = AvatarController;

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        while (Connecting.Value == false)
        {
            int randomTime = UnityEngine.Random.Range(10, 23); //minは含むがmaxは含まないらしい。
            int randomState = UnityEngine.Random.Range(1, 3);
            
            await Delay.Second(randomTime);
            animator.SetInteger("IdleState", randomState);
            
            await Delay.Frame(1);
            animator.SetInteger("IdleState", 0);
        }
    }


    // アニメーションとモーキャプの切り替え
    async void SW_Caputure_Animation(bool connecting, GameObject body)
    {
        Debug.Log($"モーキャプスイッチ0{connecting}");

        Animator animator = body.GetComponent<Animator>();
        animator.applyRootMotion = true;

        switch (connecting)
        {
            case true:
                if (animator.runtimeAnimatorController != null) animator.runtimeAnimatorController = null;
                Receiver.Model = AvatarBodies[NowAvatarBodyNum];
                break;
            case false:
                animator.runtimeAnimatorController = AvatarController;
                IdleAnim(body);
                break;
        }
    }
}



