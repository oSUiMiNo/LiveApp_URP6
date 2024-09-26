using UnityEngine;
//using UniVRM10;
using VRM;
using UniGLTF;
using System.IO;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Core.PathCore;

public class LoadVRMFromBytes : MonoBehaviourMyExtention
{

    private async void Start()
    {
        await Execute(@$"Assets/VRM_As_TextAsset/Celestia_v.2.0.0.bytes");
    }


    public static async UniTask<GameObject> Execute(string path)
    {
        GameObject body = await LoadVRMFromTextAsset(path);
        if (body == null) return null;
        new MTConverter(body, Shader.Find("VRM10/Universal Render Pipeline/MToon10"))
        {
            propNames_Orig_Dest_Array = new string[,]
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
            }
        }.Execute();
        return body;
    }

    public static async UniTask<GameObject> LoadVRMFromTextAsset(string path)
    {
        if (path == null) return null;
        var instance = await VrmUtility.LoadAsync(path);
        
        // Rendererがオフになっていることがあるので全部オンにする
        SkinnedMeshRenderer[] components = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer component in components)
        {
            component.enabled = true;
        }
        return instance.gameObject;
    }
}