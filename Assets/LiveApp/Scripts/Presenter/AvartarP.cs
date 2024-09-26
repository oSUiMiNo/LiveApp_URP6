using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using static RuntimeData;
using static RuntimeState;
using static VMCState;

public class AvartarP : P
{
    public override async UniTask Execute()
    {
        GameObject body = await VRMLoader.Execute();
        if (!body) return;
        
        // ボディを管理リストに追加
        AvatarBodies.Add(body);

        // アバタースポーンイベント
        onSpawnAvatar.OnNext(body);

        // 初回ロード時にアバターチェンジ処理も１回実行
        onLoadFirstAvatar.Subscribe(onChangeAvatar);
        
        // １回目の召喚だった場合演出　そうでなければ何もせずアンビジュアライズ
        if (AvatarBodies.Count == 1) onLoadFirstAvatar.OnNext(body);
        else
        if (AvatarBodies.Count >= 2) body.SetActive(false);

        // 外部モーキャプアプリと接続中の場合、召喚したボディを
        // Receiver(VMCプロトコル接続用のやつ)のモデルに突っ込むことでモーキャプ反映
        if (Connecting.Value) Receiver.Model = body;
        
        // インポート完了イベント
        onLoadAvatar.OnNext(body);
    }
}


