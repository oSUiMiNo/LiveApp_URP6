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
        // 1�b��1����s
        if (time_UpdateConnectingState < 1) return;
        time_UpdateConnectingState = 0;

        // ���t���[���Ɉ��AOscServer�̎��ۂ̐ڑ���Ԃ�False�ɂȂ�A
        // �����ŃA�o�^�[�̃��[�V�������؂�ւ���Ă��܂��B
        // ���ۂ̐ڑ���Ԃ��R�b�ȏ�False�������ꍇ�̂݁A
        // �A�v����ł̐ڑ��X�e�[�gFalse�ɂ��邱�ƂŁA���o�X�g�����グ��B
        // ���ۂ̐ڑ���Ԃ���uFalse�S�����ꍇ�A���̈�u�����ł܂���x�Ȃ̂ŁA
        // �ڎ��ł͋C�Â��Ȃ����炨�����[�B
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
        //Debug.Log($"�R�l�N�g��ԍX�V�F{connecting.Value}");
    }
}
