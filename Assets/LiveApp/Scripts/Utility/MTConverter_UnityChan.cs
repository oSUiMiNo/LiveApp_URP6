using System.Collections.Generic;
using UnityEngine;

public class MTConverter_UnityChan : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> targets; // 対象となるオブジェクトの配列
    // Universal Render Pipeline/Toon
    [SerializeField]
    Shader urpShader; // URPへ切り替えるためのシェーダー
    [SerializeField]
    public bool toExport = false;

    void Start()
    {
        targets.ForEach(a =>
        {
            new MTConverter(a, urpShader)
            {
                targetToExept = new GameObject[]
                {
                    GameObject.Find("OnpuParticleL"),
                    GameObject.Find("OnpuParticleR")
                },
                setPropNamesAutomatic = true,
                toExport = this.toExport
            }.Execute();
        });
    }
}
