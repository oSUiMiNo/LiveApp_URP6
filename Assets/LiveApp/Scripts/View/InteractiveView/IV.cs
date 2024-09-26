using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using static RuntimeData;
using UnityEngine.EventSystems;
using UniRx;
using System.Windows.Forms;


// Interective View
public abstract class IV : SealableMonoBehaviourMyExtention
{
    public BoolReactiveProperty StartInitialize { get; set; } = new BoolReactiveProperty(false);
    public BoolReactiveProperty Initialized { get; set; } = new BoolReactiveProperty(false);
    //public bool Initialized { get; set; } = false;

    protected sealed async override void Awake()
    {
        await UniTask.WaitUntil(() => ViewSetter.Initialized.Value);
        StartInitialize.Value = true;

        //await UniTask.WaitUntil(() => RuntimeDataInitialized);
        await Awake0();
 
        Initialized.Value = true; // Awake0()����
        //DebugView.Log($"�C���^���N�e�B�u�r���[������ {gameObject}");
    }
    protected virtual async UniTask Awake0() { }

}



// Controller View
public abstract class CV : IV
{
    public abstract Type ControllerType { get; set; }
    public MyUI Controller => GetComponent<MyUI>();
    public abstract P Presenter { get; set; }

    protected sealed override async UniTask Awake0()
    {
        //P���Ǘ������ɓo�^
        P.Children.Add(gameObject, Presenter);
        //���I�u�W�F�N�g��C���A�^�b�`
        if (ControllerType != null) CheckAddComponent(ControllerType, gameObject);
        
        await Awake1();
    }
    protected virtual async UniTask Awake1() { }
}




// Reflection View
// <T>�Ɏg������P�C���X�^���X�̌^�����邱�ƂŁA�ϐ�Presenter �ɁAUIPresenterBase����_�E���L���X�g���ꂽ��̓I�ȃv���[���^�C���X�^���X������A�uPresenter.�v�Ŕh���^���L�̃����o�ɃA�N�Z�X�ł���悤�ɂȂ�B
public abstract class RV<T> : IV where T : P
{
    internal abstract GameObject ControllerObj { get; set; }  // ��rV�I�u�W�F�N�g��C�ƂȂ��Ă���I�u�W�F�N�g��������BViewSetter.ApplySlider() ����Őݒ肷��d�l�ɂ��Ă���
    protected MyUI Controller => ControllerObj.GetComponent<MyUI>(); // C�I�u�W�F�N�g����C���擾
    protected T Presenter => (T)P.Children[ControllerObj]; // �h���^�Ƀ_�E���L���X�g����P�C���X�^���X���Ǘ���������擾

     
    protected sealed override async UniTask Awake0()
    {   
        await Awake1();
    }
    protected virtual async UniTask Awake1() { }
}

