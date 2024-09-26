using System.Collections.Generic;
using UnityEngine;

public class MTConverter_UnityChan : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> targets; // �ΏۂƂȂ�I�u�W�F�N�g�̔z��
    // Universal Render Pipeline/Toon
    [SerializeField]
    Shader urpShader; // URP�֐؂�ւ��邽�߂̃V�F�[�_�[
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
