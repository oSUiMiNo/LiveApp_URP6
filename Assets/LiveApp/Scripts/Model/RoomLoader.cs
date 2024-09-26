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

//Roomローダー
public class RoomLoader : UIModel  
{
    public static async UniTask<GameObject> Execute()
    {
        GameObject body = await LoadRoomAsync(FileBrowser.SelectFilePath("glb", "Select Room"));
        Debug.Log($"ロードしたルーム{body}");

        new MTConverter(body, Shader.Find("Universal Render Pipeline/Lit"))
        {
            propNames_Orig_Dest_Array = new string[,]
            {
                { "_Color", "_BaseColor "},
                { "_MainTex","_BaseMap" },

                { "_Cutoff", "_Cutoff" },

                { "_Glossiness",  "_Smoothness" },
                { "_GlossMapScale", "NONE" }, //片方無し
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
                { "NONE", "_DetailAlbedoMapScale" }, //片方無し
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

    //    // Rendererがオフになっていることがあるので全部オンにする
    //    Renderer[] components = instance.GetComponentsInChildren<Renderer>();
    //    foreach (Renderer component in components)
    //    {
    //        component.enabled = true;
    //    }
    //    return instance.gameObject;
    //}

    #region uniGLTFのURP用関数
    /*uniGLTFの機能で、URPシェーダ用のインポート用関数がありみたいなので使ったが、
     * シェーダ各プロパティのコピーが雑なのでやめた。自分でコンバートする。*/
    static async UniTask<GameObject> LoadRoomAsync(string path)
    {
        //ファイル拡張子で自動判定します
        GltfData data = new AutoGltfFileParser(path).Parse();

        //IMaterialDescriptorGenerator を実装することで import 時に適用されるマテリアルを差し替えることができます。
        IMaterialDescriptorGenerator materialGenerator = new GltfUrpMaterialDescriptorGenerator();

        //ロード
        // ImporterContext は使用後に Dispose を呼び出してください。
        // using で自動的に呼び出すことができます。
        using (ImporterContext loader = new ImporterContext(data, materialGenerator: materialGenerator))
        {
            IAwaitCaller awaitCaller = new ImmediateCaller(); //こいつは謎
            var instance = await loader.LoadAsync(awaitCaller);

            //Rendererがオフになっていることがあるので全部オンにする
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


