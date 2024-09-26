using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UniRx;
using UnityEngine;
using uOSC;
using static VMCState;

public class VMCConnecterP : P
{
    public override async UniTask Execute()
    {
        if (_Text.Value == string.Empty ||
          int.Parse(_Text.Value) == 0)
        {
            OscServer.port = 39539;
        }
        else
        {
            OscServer.port = int.Parse(_Text.Value);
        }
    }
}