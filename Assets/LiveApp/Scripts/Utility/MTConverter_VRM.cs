using System.Collections.Generic;
using UnityEngine;

public class MTConverter_VRM : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> targets; // �ΏۂƂȂ�I�u�W�F�N�g�̔z��
    [SerializeField]
    public Shader urpShader; // URP�֐؂�ւ��邽�߂̃V�F�[�_�[
    [SerializeField]
    public bool toExport = false;    

    void Start()
    {
        string[,] propNames_Orig_Dest_Array =
        {
            //�����_�����O�v���p�e�B
            { "_AlphaMode", "_AlphaMode" },
            { "_TransparentWithZWrite", "_TransparentWithZWrite" },
            { "_Cutoff", "_Cutoff" },
            { "_RenderQueueOffset", "_RenderQueueOffset" },
            { "_DoubleSided", "_DoubleSided" },
            //���C�e�B���O�v���p�e�B
            { "_Color", "_Color" },
            { "_MainTex", "_MainTex" },
            { "_ShadeColor", "_ShadeColor" },
            { "_ShadeTexture", "_ShadeTex" },
            { "_BumpMap", "_BumpMap" },
            { "_BumpScale", "_BumpScale" },
        };
        targets.ForEach(a =>
        {
            new MTConverter(a, urpShader)
            {
                propNames_Orig_Dest_Array = propNames_Orig_Dest_Array,
                toExport = this.toExport
            }.Execute();
        });
    }
}
