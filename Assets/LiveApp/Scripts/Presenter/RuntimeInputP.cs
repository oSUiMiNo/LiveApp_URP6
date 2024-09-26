using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using uOSC;
using EVMC4U;
using Unity.VisualScripting;
using UniRx;
using static RuntimeData;
using static RuntimeState;
using static VMCState;
using DG.Tweening;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

public class RuntimeInputP : P
{
    // VRM�ړ�
    public static IObservable<Vector3> OnVRM_Move => onVRM_Move;
    internal static Subject<Vector3> onVRM_Move = new Subject<Vector3>();
    // VRM��]
    public static IObservable<Vector3> OnVRM_Rotate => onVRM_Rotate;
    internal static Subject<Vector3> onVRM_Rotate = new Subject<Vector3>();



    // �v���t�@�C���̕\�������X�C�b�`
    public static IObservable<Unit> OnSW_Profiler => onSW_Profiler;
    internal static Subject<Unit> onSW_Profiler = new Subject<Unit>();
    // �X�J�C�{�b�N�X���X�C�b�`
    public static IObservable<int> OnSW_Sky => onSW_Sky;
    internal static Subject<int> onSW_Sky = new Subject<int>();
    // ���[���h�G�t�F�N�g���X�C�b�`
    public static IObservable<int> OnSW_WorldEff => onSW_WorldEff;
    internal static Subject<int> onSW_WorldEff = new Subject<int>();
    // �A�o�^�[���X�C�b�`
    public static IObservable<int> OnSW_Avatar => onSW_Avatar;
    //internal static Subject<int> onSW_Avatar = new Subject<int>();
    public static Subject<int> onSW_Avatar = new Subject<int>();

    // �J�[�\���A�C�R�����X�C�b�`
    public static IObservable<int> OnSW_Cursor => onSW_Cursor;
    internal static Subject<int> onSW_Cursor = new Subject<int>();




    // �J�����̋����ύX
    public static IObservable<Unit> OnCAM_Distance => onCAM_Distance;
    internal static Subject<Unit> onCAM_Distance = new Subject<Unit>();
    // �J�������n��
    public static IObservable<Unit> OnCAM_Around => onCAM_Around;
    internal static Subject<Unit> onCAM_Around = new Subject<Unit>();
    // �J�����z�[���h
    public static IObservable<Unit> OnCAM_Hold => onCAM_Hold;
    internal static Subject<Unit> onCAM_Hold = new Subject<Unit>();
    // �J�������z�[���|�W�V������
    public static IObservable<Unit> OnCAM_Home => onCAM_Home;
    internal static Subject<Unit> onCAM_Home = new Subject<Unit>();



    public override async UniTask Execute()
    {
        #region VRM�̈ړ��A��] ==================================
        InputEventHandler.On_Z += () => { if (!AvatarExist) return; onVRM_Move.OnNext(Vector3.down); };
        InputEventHandler.On_C += () => { if (!AvatarExist) return; onVRM_Move.OnNext(Vector3.up); };
        InputEventHandler.On_A += () => { if (!AvatarExist) return; onVRM_Move.OnNext(Vector3.right); };
        InputEventHandler.On_D += () => { if (!AvatarExist) return; onVRM_Move.OnNext(Vector3.left); };
        InputEventHandler.On_W += () => { if (!AvatarExist) return; onVRM_Move.OnNext(Vector3.back); };
        InputEventHandler.On_S += () => { if (!AvatarExist) return; onVRM_Move.OnNext(Vector3.forward); };
        InputEventHandler.On_Q += () => { if (!AvatarExist) return; onVRM_Rotate.OnNext(Vector3.up); };
        InputEventHandler.On_E += () => { if (!AvatarExist) return; onVRM_Rotate.OnNext(Vector3.down); };
        #endregion�@==================================


        #region �J�������� ==================================
        InputEventHandler.OnStart_Wheel += () => onSW_Cursor.OnNext(3);
        InputEventHandler.OnDown_MouseMiddle += () => onSW_Cursor.OnNext(1);
        InputEventHandler.OnDown_MouseRight += () => onSW_Cursor.OnNext(2);
        InputEventHandler.OnStop_Wheel += () => onSW_Cursor.OnNext(0);
        InputEventHandler.OnUp_MouseRight += () => onSW_Cursor.OnNext(0);
        InputEventHandler.OnUp_MouseMiddle += () => onSW_Cursor.OnNext(0);
        onSW_Cursor.OnNext(0);
        #endregion ==================================


        #region ���[���h�G�t�F�N�g�؂�ւ� ==================================
        InputEventHandler.OnDown_F5 += () => onSW_WorldEff.OnNext(0);
        InputEventHandler.OnDown_F6 += () => onSW_WorldEff.OnNext(1);
        InputEventHandler.OnDown_F7 += () => onSW_WorldEff.OnNext(2);
        #endregion ==================================


        #region �A�o�^�[Body�؂�ւ� ==================================
        InputEventHandler.OnDown_N += () => onSW_Avatar.OnNext(0);
        InputEventHandler.OnDown_M += () => onSW_Avatar.OnNext(1);
        #endregion ==================================


        #region �X�J�C�{�b�N�X�؂�ւ� ==================================
        InputEventHandler.OnDown_1 += () => onSW_Sky.OnNext(0);
        InputEventHandler.OnDown_2 += () => onSW_Sky.OnNext(1);
        InputEventHandler.OnDown_3 += () => onSW_Sky.OnNext(2);
        #endregion ==================================


        #region �v���t�@�C���\�� ==================================
        InputEventHandler.OnDown_P += () => onSW_Profiler.OnNext(Unit.Default);
        #endregion ==================================


        #region �J�������� ==================================
        // �J�������z�[���|�W�V�����ɖ߂�
        //InputEventHandler.OnDown_H += () => { if (!AvatarExist) return; onCAM_Home.OnNext(Unit.Default); };
        //InputEventHandler.On_Wheel += () => { if (!AvatarExist) return; onCAM_Distance.OnNext(Unit.Default); };
        //InputEventHandler.On_MouseMiddle += () => { if (!AvatarExist) return; onCAM_Hold.OnNext(Unit.Default); };
        //InputEventHandler.On_MouseRight += () => { if (!AvatarExist) return; onCAM_Around.OnNext(Unit.Default); };
        // �A�o�^�[�C���|�[�g���ł͂Ȃ�����ɕύX
        InputEventHandler.OnDown_H += () => { onCAM_Home.OnNext(Unit.Default); };
        InputEventHandler.On_Wheel += () => { onCAM_Distance.OnNext(Unit.Default); };
        InputEventHandler.On_MouseMiddle += () => { onCAM_Hold.OnNext(Unit.Default); };
        InputEventHandler.On_MouseRight += () => { onCAM_Around.OnNext(Unit.Default); };
        #endregion ==================================
    }
}