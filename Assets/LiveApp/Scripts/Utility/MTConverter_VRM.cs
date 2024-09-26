using System.Collections.Generic;
using UnityEngine;

public class MTConverter_VRM : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> targets; // 対象となるオブジェクトの配列
    [SerializeField]
    public Shader urpShader; // URPへ切り替えるためのシェーダー
    [SerializeField]
    public bool toExport = false;    

    void Start()
    {
        string[,] propNames_Orig_Dest_Array =
        {
            //レンダリングプロパティ
            { "_AlphaMode", "_AlphaMode" },
            { "_TransparentWithZWrite", "_TransparentWithZWrite" },
            { "_Cutoff", "_Cutoff" },
            { "_RenderQueueOffset", "_RenderQueueOffset" },
            { "_DoubleSided", "_DoubleSided" },
            //ライティングプロパティ
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
