using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static RuntimeData;

public class SizeAdjusterP : P
{
    GameObject target;

    public override async UniTask Execute()
    {
        await UniTask.WaitUntil(() => target != null);
        ChangeEditorbleScale(target, value_Float.Value);
    }

    public EditorbleInfo SetEditorble(GameObject target)
    { 
        this.target = target;
        return AddEditorble(target);
    }
}
