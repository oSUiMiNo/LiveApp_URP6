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

    // ���񉉏o�����t���O
    bool FirstDirectionFinished = false;

    // �X�|�[�����́A���[�J���|�W�V�����i�e�I�u�W�F�N�gAvatar����ǂꂾ�����炷���j
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


    // �X�|�[��
    void Spawn(GameObject body)
    {
        // 3D���f����Avatar�̎q�I�u�W�F�N�g�ɂ���
        body.transform.parent = _Avatar.transform;
        // Avatar�ɑ΂��Ẵ��[�J���|�W�V������0�ɂ���
        body.transform.localPosition = Vector3.zero;
        // Avatar�ɑ΂��Ẵ��[�J���X�P�[����1�ɂ���B��
        body.transform.localScale = Vector3.one;

        body.SetActive(false);
    }


    // ���񉉏o
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


    // �A�o�^�[�̃X�C�b�`�Ɖ��o
    public async void Direction_SW(int index)
    {
        NowAvatarBodyNum = index;
        GameObject body = AvatarBodies[NowAvatarBodyNum];

        // �J�b�g�C��
        CutIn(body);
        
        await Delay.Second(1.2f);
        // ���[�L���v�ƃA�j���[�V�������X�C�b�`
        SW_Caputure_Animation(Connecting.Value, body);

        // Body�؂�ւ�
        Receiver.Model = body;
        AvatarBodies.ForEach(a => a.SetActive(false));
        body.SetActive(true);
    }


    async void CutIn(GameObject body)
    {
        // �o�[�`�����J�����̏Ə����J�b�g�C���A�o�^�[�ɍ��킹��
        vWebCam_VirtualCamera.Follow = CutInAvatar.transform;
        vWebCam_VirtualCamera.LookAt = CutInAvatar.transform;

        await Delay.Second(0.1f);
        GameObject cutInBody = Instantiate(body);
        cutInBody.transform.parent = CutInAvatar.transform;
        cutInBody.transform.localPosition = Vector3.zero;
        cutInBody.transform.localScale = Vector3.one;
        cutInBody.SetActive(true);

        // �ԉ�
        GameObject fireWorks = Instantiate(await Addressables.LoadAssetAsync<GameObject>("FireWorks"));
        fireWorks .transform.parent = CutInAvatar.transform;
        fireWorks.transform.localPosition = new Vector3(0, -1, -6);
        
        // �ԉ΂̉�
        AudioSource[] audioSources = fireWorks.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.clip.name == "�ł��グ�ԉ�1") PlaySound(audioSource, 3);
            if (audioSource.clip.name == "�ԉΑ��") PlaySound(audioSource, 1);
        }

        // �G�t�F�N�g�Đ�
        Eff.SetActive(true);
        Eff.transform.position = CutInAvatar.transform.position;

        await Delay.Second(1.5f);
        // �A�j���[�^���Z�b�g
        Animator animator = cutInBody.GetComponent<Animator>();
        animator.runtimeAnimatorController = CutInAvatarController;
        animator.SetInteger("selection", new System.Random().Next(1, 4));

        await Delay.Second(3);
        // �G�t�F�N�g��~
        Eff.SetActive(false);

        await Delay.Second(6);
        // �t�F�[�h
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

    // ���̂� audioSource.DOFade() ���g���Ȃ��̂�
    // DOTweenModuleAudio�N���X���甲���Ă���
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



    // ����A�j���[�V����
    async UniTask FirstAnim(GameObject body)
    {
        // �����ʒu��Avatar���班�����炵���ʒu�ɔz�u�B���o�̂���
        body.transform.localPosition = spawnLocalPos;
        // �����̌������߁B���̂Ƃ���A���̌�̉��o�̂��߂ɉ��Ɍ������Ă���
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


    // �A�C�h�����O�A�j���[�V����
    public async void IdleAnim(GameObject body)
    {
        Animator animator = body.GetComponent<Animator>();
        animator.applyRootMotion = true;
        animator.runtimeAnimatorController = AvatarController;

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        while (Connecting.Value == false)
        {
            int randomTime = UnityEngine.Random.Range(10, 23); //min�͊܂ނ�max�͊܂܂Ȃ��炵���B
            int randomState = UnityEngine.Random.Range(1, 3);
            
            await Delay.Second(randomTime);
            animator.SetInteger("IdleState", randomState);
            
            await Delay.Frame(1);
            animator.SetInteger("IdleState", 0);
        }
    }


    // �A�j���[�V�����ƃ��[�L���v�̐؂�ւ�
    async void SW_Caputure_Animation(bool connecting, GameObject body)
    {
        Debug.Log($"���[�L���v�X�C�b�`0{connecting}");

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



