using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class RuntimeState : MonoBehaviour
{
    #region �A�o�^�[ ========================
    //�A�o�^�[���[�h
    public static IObservable<GameObject> OnLoadAvatar => onLoadAvatar;
    internal static Subject<GameObject> onLoadAvatar = new Subject<GameObject>();

    // �A�o�^�[�X�|�[��
    public static IObservable<GameObject> OnSpawnAvatar => onSpawnAvatar;
    internal static Subject<GameObject> onSpawnAvatar = new Subject<GameObject>();

    // ����̃A�o�^�[���o
    public static IObservable<GameObject> OnLoadFirstAvatar => onLoadFirstAvatar;
    internal static Subject<GameObject> onLoadFirstAvatar = new Subject<GameObject>();

    // �A�o�^�[�؂�ւ�
    public static IObservable<GameObject> OnChangeAvatar => onChangeAvatar;
    internal static Subject<GameObject> onChangeAvatar = new Subject<GameObject>();
    #endregion =======================


    #region ���[�� ========================
    //���[�����[�h
    public static IObservable<Unit> OnLoadRoom => onLoadRoom;
    internal static Subject<Unit> onLoadRoom = new Subject<Unit>();

    // ���[���X�|�[��
    public static IObservable<GameObject> OnSpawnRoom => onSpawnRoom;
    internal static Subject<GameObject> onSpawnRoom = new Subject<GameObject>();
    #endregion =======================


    #region RuntimeInput ========================

    #endregion =======================


    // ���[�L���v�����ǂ���
    public static IReadOnlyReactiveProperty<bool> MoCapIsRunning => moCapIsRunning;
    internal static BoolReactiveProperty moCapIsRunning = new BoolReactiveProperty(false);
}
