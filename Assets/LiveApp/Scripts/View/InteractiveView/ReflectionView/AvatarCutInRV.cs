using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static RuntimeData;
using static RuntimeState;

public class AvatarCutInRV : RV<AvartarP>
{
    internal override GameObject ControllerObj { get; set; } = null;

    protected sealed override async UniTask Awake1()
    {
        OnChangeAvatar.Subscribe(body => ChangeCutInAvatar(body));
    }

    
    void ChangeCutInAvatar(GameObject body)
    {
        // 3DモデルをAvatarの子オブジェクトにする
        body.transform.parent = _Avatar.transform;
        // Avatarに対してのローカルスケールを1にする。仮
        body.transform.localScale = Vector3.one;
    }
}
