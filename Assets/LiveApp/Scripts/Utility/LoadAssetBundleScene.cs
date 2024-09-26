using Cysharp.Threading.Tasks;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

public class LoadAssetBundleScene : MonoBehaviour
{
    void Start()
    {
        //InputEventHandler.OnDown_0 += () => LoadPrefab();
    }


    async void LoadPrefab()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select VRM", "", "", false);
        await AssetBundle.LoadFromFileAsync(paths[0]);
        await SceneManager.LoadSceneAsync("tutorial scene", LoadSceneMode.Additive);

        string[,] propNames_Orig_Dest =
        {
            { "_Color", "_BaseColor" },
            { "_MainTex", "_BaseMap" },

            { "_Cutoff", "_Cutoff" },

            { "_Glossiness",  "_Smoothness" },
            { "_GlossMapScale", "" },   //
            { "_SmoothnessTextureChannel", "_SmoothnessTextureChannel" },

            { "_Metallic", "_Metallic" },
            { "_MetallicGlossMap", "_MetallicGlossMap" },

            { "_Metallic", "_SpecColor" },
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
            { "", "_DetailAlbedoMapScale" },    //
            { "_DetailNormalMap", "_DetailNormalMap" },
            { "_DetailNormalMapScale", "_DetailNormalMapScale" },

            { "_ShadeColor", "_ShadeColor" },
            { "_ShadeTexture", "_ShadeTex" },
        };

        //await UniTask.Delay(TimeSpan.FromSeconds(3));

        /*シーンのインデックスから取得できる
         * ロード済みのシーンじゃないと出来ないっぽい*/
        Scene roomScene = SceneManager.GetSceneAt(1);

        // 新しくロードした方のシーンから "MainCamera"タグのついたもの（＝メインカメラ）を取得
        var mainCamera = roomScene.GetRootGameObjects()
            .Where(x => x.CompareTag("MainCamera"))
            .First();

        Debug.Log(mainCamera);
        Destroy(mainCamera);

        GameObject room = GameObject.Find("Room");
        Debug.Log(room.name);

        MTConverter mTConverter = new MTConverter(room, Shader.Find("Universal Render Pipeline/Lit"))
        {
            propNames_Orig_Dest_Array = propNames_Orig_Dest
        };
        mTConverter.Execute();
    }


    async UniTask LoadScene()
    {

    }
}
