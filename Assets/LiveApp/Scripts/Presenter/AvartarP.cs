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
        
        // �{�f�B���Ǘ����X�g�ɒǉ�
        AvatarBodies.Add(body);

        // �A�o�^�[�X�|�[���C�x���g
        onSpawnAvatar.OnNext(body);

        // ���񃍁[�h���ɃA�o�^�[�`�F���W�������P����s
        onLoadFirstAvatar.Subscribe(onChangeAvatar);
        
        // �P��ڂ̏����������ꍇ���o�@�����łȂ���Ή��������A���r�W���A���C�Y
        if (AvatarBodies.Count == 1) onLoadFirstAvatar.OnNext(body);
        else
        if (AvatarBodies.Count >= 2) body.SetActive(false);

        // �O�����[�L���v�A�v���Ɛڑ����̏ꍇ�A���������{�f�B��
        // Receiver(VMC�v���g�R���ڑ��p�̂��)�̃��f���ɓ˂����ނ��ƂŃ��[�L���v���f
        if (Connecting.Value) Receiver.Model = body;
        
        // �C���|�[�g�����C�x���g
        onLoadAvatar.OnNext(body);
    }
}


