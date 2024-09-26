using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using static RuntimeData;
using Cysharp.Threading.Tasks;
using System.Windows.Forms;
#if UNITY_EDITOR
using UnityEditor;
#endif


// �����̃Q�[���I�u�W�F�N�g�ɓ����R���|�[�l���g���g������
// ����̃V�[���\���Ɉˑ�������̂ƁAiV�̌q���n�̏������Ǘ�����
// �������̃^�C�~���O���AiV �ɑ΂��Ă��������ɂ������̂ŁAInteractiveView ���p������ Awake0() ���g��

// �R���|�[�l���g�̃C���X�^���X�́A�A�^�b�`���邱�ƂŐ�������̂ŁA�R���X�g���N�^�͎g���Ȃ��B
// ���������ĕ����̃Q�[���I�u�W�F�N�g�ɓ����R���|�[�l���g�𗬗p���A���ϐ����قȂ点�����ꍇ�A����3�̑I����������B
//�y1.�C���X�y�N�^�[����ݒ�z�y2.���̃X�N���v�g�Ōォ��ݒ�z�y3.���p�͒��߂ĕʂ̃X�N���v�g�Ƃ��ĕ�����ނ̃R���|�[�l���g�����z
// �S�R�قȂ�Q�[���I�u�W�F�N�g�ɕt���������R���|�[�l���g�̕ϐ����A�Q�[���̏�ʂɉ����Ăǂ����̃X�N���v�g����ύX���邱�Ƃ͕s���R�ł͂Ȃ����A
// UI�v�f�̂悤�ɕ����̓����悤�ȃQ�[���I�u�W�F�N�g�̓����R���|�[�l���g�̕ϐ����A�蓮�ŕύX����͖̂ʓ|�����A�ꍇ�ɂ���Ă͐ݒ肪���Z�b�g�����B
// �܂��A�ϐ��̒��g���Ⴄ�����ŃR���|�[�l���g�����������ʃX�N���v�g�Ƃ��č��킯�ɂ͂����Ȃ��B
// ����Ă��̂悤�Ȉꊇ�ǃN���X���K�v�ƂȂ����B

//�������AButton�R���|�[�l���g�̂悤�ɒ������ȉӏ��ŗ��p�����R���|�[�l���g�͂Ƃ������Q�`�R�ӏ����炢�ł������g���Ȃ����̂Ɋւ��ẮA���ꂼ��̃R���|�[�l���g�̏����������ŏ������򂵂Ēl���قȂ点��̂ł��\���ȋC������B�Ⴆ�Ύ��Q�[���I�u�W�F�N�g�̖��O��������������`�Ƃ��B

//[ExecuteInEditMode]
public class ViewSetter : MonoBehaviour
{
    [SerializeField]
    RuntimeAnimatorController animController_Button;

    public static BoolReactiveProperty Initialized { get; set; } = new BoolReactiveProperty(false);

    // IV �� Initialized ��҂Ă΁AIV�p������Awake0()�@����Ȃ��Ă��ǂ��C������̂Ŏ����B
    //protected sealed override async UniTask Awake0()
    //{
    //    ApplyAnim(animController_Button, "UI");
    //    ApplyAnim(animController_Button, "TestButton");
    //    ApplySlider(GameObject.Find("Slider_SizeAdjuster_Avatar"), _Avatar);
    //    ApplySlider(GameObject.Find("Slider_SizeAdjuster_Room"), Room);
    //}
    private void Awake()
    {
        ApplyAnim(animController_Button, "UI_Under");
        //ApplyAnim(animController_Button, "TestButton");
        
        //�����̓R���|�[�l���g���g���̏����������ɒS�킹�邱�Ƃɂ����B
        //ApplySlider(GameObject.Find("Slider_SizeAdjuster_Avatar"), _Avatar);
        //ApplySlider(GameObject.Find("Slider_SizeAdjuster_Room"), Room);

        Initialized.Value = true;
    }





    //UI�e�v�f�Ŏg���A�j���[�V����(�}�E�X�I�[�o�[����)���ꊇ�Őݒ�B
    //UI�v�f�̐e�I�u�W�F�N�g�̖��O���ƁA�ݒ肵�����A�j���[�^�[�R���g���[���������ɓn���ƁA�qUI�v�f�I�u�W�F�N�g�Ɉꊇ�ŃA�j���[�V���������Ă����
    async void ApplyAnim(RuntimeAnimatorController animatorController, string targetName)
    {
        CV[] views = GameObject.Find(targetName).GetComponentsInChildren<CV>();
        foreach (CV view in views)
        {
            await UniTask.WaitUntil(() => view.Controller != null);
            MyButton controller = view.Controller as MyButton;
            if (controller == null) continue;
            controller.animatorController.Value = animatorController;
        }
    }




    ////�A�v���̍\���I�u�W�F�N�g�ɃA�^�b�`����Ă���T�C�Y�X�V�p�R���| SizeReflector �ƁA�T�C�Y����p�̃X���C�_�[�I�u�W�F�N�g���q����
    //async void ApplySlider(GameObject sliderUI, GameObject target)
    //{
    //    SizeAdjusterRV view = target.GetComponent<SizeAdjusterRV>();
    //    if (view.ControllerObj == null) view.ControllerObj = sliderUI;
    //}



//    protected override sealed void Update()
//    {
//#if UNITY_EDITOR
//        if (!EditorApplication.isPlaying)
//        {
//            Awake();
//        }
//#endif
//    }
}