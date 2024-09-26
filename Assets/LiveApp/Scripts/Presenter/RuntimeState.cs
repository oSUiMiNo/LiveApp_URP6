using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class RuntimeState : MonoBehaviour
{
    #region アバター ========================
    //アバターロード
    public static IObservable<GameObject> OnLoadAvatar => onLoadAvatar;
    internal static Subject<GameObject> onLoadAvatar = new Subject<GameObject>();

    // アバタースポーン
    public static IObservable<GameObject> OnSpawnAvatar => onSpawnAvatar;
    internal static Subject<GameObject> onSpawnAvatar = new Subject<GameObject>();

    // 初回のアバター演出
    public static IObservable<GameObject> OnLoadFirstAvatar => onLoadFirstAvatar;
    internal static Subject<GameObject> onLoadFirstAvatar = new Subject<GameObject>();

    // アバター切り替え
    public static IObservable<GameObject> OnChangeAvatar => onChangeAvatar;
    internal static Subject<GameObject> onChangeAvatar = new Subject<GameObject>();
    #endregion =======================


    #region ルーム ========================
    //ルームロード
    public static IObservable<Unit> OnLoadRoom => onLoadRoom;
    internal static Subject<Unit> onLoadRoom = new Subject<Unit>();

    // ルームスポーン
    public static IObservable<GameObject> OnSpawnRoom => onSpawnRoom;
    internal static Subject<GameObject> onSpawnRoom = new Subject<GameObject>();
    #endregion =======================


    #region RuntimeInput ========================

    #endregion =======================


    // モーキャプ中がどうか
    public static IReadOnlyReactiveProperty<bool> MoCapIsRunning => moCapIsRunning;
    internal static BoolReactiveProperty moCapIsRunning = new BoolReactiveProperty(false);
}
