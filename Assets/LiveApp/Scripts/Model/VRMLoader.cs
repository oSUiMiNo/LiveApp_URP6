using Cysharp.Threading.Tasks;
using EVMC4U;
using UnityEngine;
using VRM;
using UniRx;

public class VRMLoader : UIModel
{
    public static async UniTask<GameObject> Execute()
    {
        GameObject body = await LoadVRMAsync(FileBrowser.SelectFilePath("VRM", "Select VRM"));
        if (body == null) return null;
        new MTConverter(body, RuntimeData.Shader_MToonURP)
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
    
    
    // MVPどこに置くかムズイ今のところM寄り
    static async UniTask<GameObject> LoadVRMAsync(string path)
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
