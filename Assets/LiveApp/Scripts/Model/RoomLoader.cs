using Cysharp.Threading.Tasks;
using UnityEngine;
using VRM;
//using 
using System.Collections.Generic;
using UniGLTF;
using VRMShaders;
using UniRx;
using System;
//using RoslynCSharp;

//Room���[�_�[
public class RoomLoader : UIModel  
{
    public static async UniTask<GameObject> Execute()
    {
        GameObject body = await LoadRoomAsync(FileBrowser.SelectFilePath("glb", "Select Room"));
        Debug.Log($"���[�h�������[��{body}");

        new MTConverter(body, Shader.Find("Universal Render Pipeline/Lit"))
        {
            propNames_Orig_Dest_Array = new string[,]
            {
                { "_Color", "_BaseColor "},
                { "_MainTex","_BaseMap" },

                { "_Cutoff", "_Cutoff" },

                { "_Glossiness",  "_Smoothness" },
                { "_GlossMapScale", "NONE" }, //�Е�����
                { "_SmoothnessTextureChannel", "_SmoothnessTextureChannel" },

                { "_Metallic", "_Metallic" },
                { "_MetallicGlossMap", "_MetallicGlossMap" },

                //{ "_Metallic", "_SpecColor" },
                { "_MetallicGlossMap", "_SpecGlossMap" },

                { "_SpecularHighlights", "_SpecularHighlights" },
                { "_GlossyReflections", "_EnvironmentReflections" },

                { "_BumpMap", "_BumpMap" },
                { "_BumpScale", "_BumpScale" },

                { "_Parallax", "_Parallax" },
                { "_ParallaxMap", "_ParallaxMap" },

                { "_OcclusionStrength", "_OcclusionStrength" },
                { "_OcclusionMap", "_OcclusionMap" },

                { "_EmissionColor", "_EmissionColor" },
                { "_EmissionMap", "_EmissionMap" },

                { "_DetailMask", "_DetailMask" },
                { "_DetailAlbedoMap", "_DetailAlbedoMap" },
                { "NONE", "_DetailAlbedoMapScale" }, //�Е�����
                { "_DetailNormalMap", "_DetailNormalMap" },
                { "_DetailNormalMapScale", "_DetailNormalMapScale" },

                { "_ShadeColor", "_ShadeColor" },
                { "_ShadeTexture","_ShadeTex" },
            }
        }.Execute();

        return body;
        return null;
    }


    //static async UniTask<GameObject> LoadRoomAsync(string path)
    //{
    //    if (path != null) return null;
    //    var instance = await VrmUtility.LoadAsync(path);

    //    // Renderer���I�t�ɂȂ��Ă��邱�Ƃ�����̂őS���I���ɂ���
    //    Renderer[] components = instance.GetComponentsInChildren<Renderer>();
    //    foreach (Renderer component in components)
    //    {
    //        component.enabled = true;
    //    }
    //    return instance.gameObject;
    //}

    #region uniGLTF��URP�p�֐�
    /*uniGLTF�̋@�\�ŁAURP�V�F�[�_�p�̃C���|�[�g�p�֐�������݂����Ȃ̂Ŏg�������A
     * �V�F�[�_�e�v���p�e�B�̃R�s�[���G�Ȃ̂ł�߂��B�����ŃR���o�[�g����B*/
    static async UniTask<GameObject> LoadRoomAsync(string path)
    {
        //�t�@�C���g���q�Ŏ������肵�܂�
        GltfData data = new AutoGltfFileParser(path).Parse();

        //IMaterialDescriptorGenerator ���������邱�Ƃ� import ���ɓK�p�����}�e���A���������ւ��邱�Ƃ��ł��܂��B
        IMaterialDescriptorGenerator materialGenerator = new GltfUrpMaterialDescriptorGenerator();

        //���[�h
        // ImporterContext �͎g�p��� Dispose ���Ăяo���Ă��������B
        // using �Ŏ����I�ɌĂяo�����Ƃ��ł��܂��B
        using (ImporterContext loader = new ImporterContext(data, materialGenerator: materialGenerator))
        {
            IAwaitCaller awaitCaller = new ImmediateCaller(); //�����͓�
            var instance = await loader.LoadAsync(awaitCaller);

            //Renderer���I�t�ɂȂ��Ă��邱�Ƃ�����̂őS���I���ɂ���
            Renderer[] components = instance.GetComponentsInChildren<Renderer>();
            foreach (Renderer component in components)
            {
                component.enabled = true;
            }
            return instance.gameObject;
        }
    }
    #endregion
}


