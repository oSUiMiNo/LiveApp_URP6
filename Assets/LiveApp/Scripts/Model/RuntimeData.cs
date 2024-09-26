using Cinemachine;
using EVMC4U;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uOSC;
using UniRx;
using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using TMPro;

// 【メモ】　セーブフォルダ : C:/Users/osuim/AppData/LocalLow/DefaultCompany/LiveApp_URP4

public class RuntimeData
{
    public static ExternalReceiver Receiver { get; } = GameObject.Find("ExternalReceiver").GetComponent<ExternalReceiver>();
    

    public static GameObject Room { get; } = GameObject.Find("Room");
    public static List<GameObject> RoomBodies { get; } = new List<GameObject>();
    public static bool RoomExist { get; } = RoomBodies.Count == 0;
    public static int NowRoomBodyNum { get; set; }

    
    public static GameObject _Avatar { get; } = GameObject.Find("Avatar");
    // アバターボディスタック用のリスト　着替えの時などに使う
    public static List<GameObject> AvatarBodies { get; } = new List<GameObject>();
    public static bool AvatarExist => AvatarBodies.Count != 0;
    public static int NowAvatarBodyNum { get; set; }
    

    public static GameObject CutInAvatar { get; } = GameObject.Find("CutInAvatar");
    public static List<GameObject> CutInAvatarBodies { get; } = new List<GameObject>();


    public static Shader Shader_MToonURP { get; } = Shader.Find("VRM10/Universal Render Pipeline/MToon10");

    public static GameObject MovieField { get; } = GameObject.Find("MovieField");

    public static GameObject _WebCam { get; } = GameObject.Find("WebCAM");
    public static TextMeshPro WebCamText { get; } = _WebCam.transform.Find("Text_WebCam").GetComponent<TextMeshPro>();
    public static GameObject vWebCam { get; } = GameObject.Find("vWebCAM");
    public static CinemachineVirtualCamera vWebCam_VirtualCamera = vWebCam.GetComponent<CinemachineVirtualCamera>();
    public static CinemachineFollowZoom vWebCam_FollowZoom = vWebCam.GetComponent<CinemachineFollowZoom>();

    public static GameObject vWebCam_CutIn { get; } = GameObject.Find("vWebCAM_CutIn");
    public static CinemachineVirtualCamera vWebCam_CutIn_VirtualCamera = vWebCam_CutIn.GetComponent<CinemachineVirtualCamera>();
    public static CinemachineFollowZoom vWebCam_CutIn_FollowZoom = vWebCam_CutIn.GetComponent<CinemachineFollowZoom>();

    public static GameObject Profiler { get; } = GameObject.Find("Profiler");

    public static string AddressablesFolderPath { get; } = $@"{Addressables.RuntimePath}\StandaloneWindows64";
    public static string DividedAssetsFolderPath { get; } = $@"{Application.dataPath}\DividedAssets";

    public static GameObject flexalon_PrefabCatalog { get; } = GameObject.Find("Profiler");
    public static GameObject flexalon_MotionCatalog { get; set; }
    public static GameObject flexalon_SceneCatalog { get; set; }



    //public static bool RuntimeDataInitialized { get; private set; } = false;
    // ↑ MonoBehaviourをとって純粋なModelクラスにする。現状上記はModelとして扱っていい。


    




    public class EditorbleInfo
    {
        public FloatReactiveProperty sizeMagnification = new FloatReactiveProperty(0);
        public float plusSpeed;
        public float minusSpeed;
        public float SizeMagnification_Upper;
        public float SizeMagnification_Lower;
        public Vector3 firstScale;

        public EditorbleInfo(float SizeMagnification_Upper, float SizeMagnification_Lower, Vector3 firstScale)
        {
            this.SizeMagnification_Upper = SizeMagnification_Upper;
            this.SizeMagnification_Lower = SizeMagnification_Lower;
            this.firstScale = firstScale;

            List<float> sizeComponents = new List<float>();
            sizeComponents.Add(firstScale.x);
            sizeComponents.Add(firstScale.y);
            sizeComponents.Add(firstScale.z);
            // スケールのx,y,zの中で一番小さい値
            float minSizeComponent = sizeComponents.Min();

            float sizelimit_Upper = minSizeComponent * SizeMagnification_Upper;
            float sizelimit_Lower = minSizeComponent * SizeMagnification_Lower;

            ///<summary>
            ///【求め方】
            ///スライダーを一番小さくした時にモデルのスケールx,y,zのなかの一番小さい値が
            ///sizelimit_Lowerになるようにする。
            ///minSizeComponent + (-10 * minusSpeed) = sizelimit_Lower;
            ///10 * minusSpeed = minSizeComponent - sizelimit_Lower;
            /// </summary>
            minusSpeed = (minSizeComponent - sizelimit_Lower) / 10;
            ///<summary>
            ///【求め方】
            ///上記と同様に
            ///minSizeComponent + (10 * plusSpeed) = sizelimit_Upper;
            ///10 * plusSpeed = sizelimit_Upper - minSizeComponent;
            /// </summary>
            plusSpeed = (sizelimit_Upper - minSizeComponent) / 10;
        }
    }
    public static Dictionary<GameObject, EditorbleInfo> editorbles = new Dictionary<GameObject, EditorbleInfo>();

    public static EditorbleInfo AddEditorble(GameObject target)
    {
        editorbles.Add(target, new EditorbleInfo(2, 0.5f, target.transform.localScale));
        return editorbles[target];
    }


    public static void ChangeEditorbleScale(GameObject target, float adjustValue)
    {
        //DebugView.Log($"{editorbles[target]}");
        EditorbleInfo editorble = editorbles[target];
        if(adjustValue > 0)
        {
            editorble.sizeMagnification.Value = adjustValue * editorble.plusSpeed;
        }
        else
        if (adjustValue == 0)
        {
            editorble.sizeMagnification.Value = 0;
        }
        else
        {
            editorble.sizeMagnification.Value = adjustValue * editorble.minusSpeed;
        }
    }    
}


//public class RunTimeDataSetter : SingletonCompo<RunTimeDataSetter>
//{
//    [SerializeField] public RuntimeAnimatorController avatarController;


 
//}

