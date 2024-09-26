using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using uOSC;
using static RuntimeData;
using TMPro;


public class VMCState : MonoBehaviour
{
    public static uOscServer OscServer;

    public static IReadOnlyReactiveProperty<bool> Connecting => connecting;
    private static ReactiveProperty<bool> connecting = new ReactiveProperty<bool>(false);


    private void Start()
    {
        OscServer = Receiver.GetComponent<uOscServer>();
    }


    float time_IntervalToDisconnect = 0;
    void FixedUpdate()
    {
        time_UpdateConnectingState += Time.deltaTime;
        if (!OscServer.connecting) time_IntervalToDisconnect += Time.deltaTime;
        else time_IntervalToDisconnect = 0;
        UpdateConnectingState();
    }



    float time_UpdateConnectingState = 0;
    void UpdateConnectingState()
    {
        // 1秒に1回実行
        if (time_UpdateConnectingState < 1) return;
        time_UpdateConnectingState = 0;

        // 数フレームに一回、OscServerの実際の接続状態がFalseになり、
        // 高速でアバターのモーションが切り替わってしまう。
        // 実際の接続状態が３秒以上Falseだった場合のみ、
        // アプリ上での接続ステートFalseにすることで、ロバスト性を上げる。
        // 実際の接続状態が一瞬False担った場合、その一瞬だけ固まる程度なので、
        // 目視では気づかないからおっけー。
        if (time_IntervalToDisconnect > 3)
        {
            connecting.Value = false;
            time_IntervalToDisconnect = 0;
        }
        else
        if (OscServer.connecting != connecting.Value)
        {
            connecting.Value = true;
        }
        //Debug.Log($"コネクト状態更新：{connecting.Value}");
    }
}
