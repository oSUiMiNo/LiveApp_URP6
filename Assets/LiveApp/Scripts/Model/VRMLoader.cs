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
            }
        }.Execute();
        return body;     
    }  
    
    
    // MVP�ǂ��ɒu�������Y�C���̂Ƃ���M���
    static async UniTask<GameObject> LoadVRMAsync(string path)
    {
        if (path == null) return null;
        var instance = await VrmUtility.LoadAsync(path);

        // Renderer���I�t�ɂȂ��Ă��邱�Ƃ�����̂őS���I���ɂ���
        SkinnedMeshRenderer[] components = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer component in components)
        {
            component.enabled = true;
        }
        return instance.gameObject;
    }
}
