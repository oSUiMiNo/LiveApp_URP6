using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using uOSC;
using EVMC4U;
using Unity.VisualScripting;
using UniRx;
using static RuntimeData;
using static RuntimeState;
using Cinemachine;
using DG.Tweening;

public class WebCamOV : MonoBehaviour
{
    [SerializeField]
    float cameraSpeed_Perspective = 0.1f;
    [SerializeField]
    float cameraSpeed_Rotation = 10f;
    [SerializeField]
    float cameraSpeed_Shift = 0.1f;

    [SerializeField]
    float cameraPivotOffset = 0.7f;
    GameObject cameraPivot;
    public float distance;
    public Vector3 direction;

    // カメラのホームポジション
    Vector3 HomePosition_Camera = Vector3.zero;
    Vector3 HomeRotation_Camera = Vector3.zero;


    public static event Action On_VRMMove;

    //CinemachineVirtualCamera vWebCam_VirtualCamera;
    //CinemachineFollowZoom vWebCam_FollowZoom;


    private async void Awake()
    {   
        cameraPivot = new GameObject("CameraPivot_WebCam");

        WebCamText.text = string.Empty;

        vWebCam_VirtualCamera.Follow = _Avatar.transform;
        vWebCam_VirtualCamera.LookAt = _Avatar.transform;
        vWebCam_VirtualCamera.Priority = 100;

        //vWebCam_CutIn_VirtualCamera.Follow = CutInAvatar.transform;
        //vWebCam_CutIn_VirtualCamera.LookAt = CutInAvatar.transform;
        vWebCam_CutIn_VirtualCamera.Priority = 1;
    }


    private async void Start()
    {
        OnLoadAvatar.Subscribe(_ => OnImportAvatar());
        SetManipulationLogic();
        await UniTask.WaitUntil(() => editorbles.ContainsKey(_Avatar));
        editorbles[_Avatar].sizeMagnification.Subscribe(_ => vWebCam_FollowZoom.m_MaxFOV += 10);
    }


    // アバターインポート時の処理
    void OnImportAvatar()
    {
        //Debug.Log($"カメラの位置をアバターに合わせる");
        cameraPivot.transform.position = RuntimeData._Avatar.transform.position + Vector3.up * cameraPivotOffset;
        gameObject.transform.position = cameraPivot.transform.position + Vector3.forward * 2;
        distance = (gameObject.transform.position - cameraPivot.transform.position).magnitude;
        direction = (gameObject.transform.position - cameraPivot.transform.position).normalized;

        // カメラのホームポジション設定
        HomePosition_Camera = gameObject.transform.position;
        HomeRotation_Camera = gameObject.transform.rotation.eulerAngles;
    }


    // ユーザー操作のロジック設定
    async void SetManipulationLogic()
    {
        #region カメラ操作 ==================================
        // カメラをホームポジションに戻す
        InputEventHandler.OnDown_H += () =>
        {
            gameObject.transform.position = HomePosition_Camera;
            gameObject.transform.rotation = Quaternion.Euler(HomeRotation_Camera);
        };

        InputEventHandler.On_Wheel += () =>
        {
            if (!RuntimeData.AvatarExist) return;
            gameObject.transform.position += gameObject.transform.forward * Input.mouseScrollDelta.y * cameraSpeed_Perspective;
            distance = Vector3.Distance(gameObject.transform.position, cameraPivot.transform.position);
        };
        InputEventHandler.On_MouseMiddle += () =>
        {
            if (!RuntimeData.AvatarExist) return;
            float rotX = Input.GetAxis("Mouse X") * cameraSpeed_Shift;
            float rotY = Input.GetAxis("Mouse Y") * cameraSpeed_Shift;
            gameObject.transform.position += gameObject.transform.rotation * new Vector3(-rotX, -rotY, 0);
            distance = (gameObject.transform.position - cameraPivot.transform.position).magnitude;
            direction = (gameObject.transform.position - cameraPivot.transform.position).normalized;
        };
        InputEventHandler.On_MouseRight += () =>
        {
            if (!RuntimeData.AvatarExist) return;
            float rotX = Input.GetAxis("Mouse X") * cameraSpeed_Rotation;
            float rotY = Input.GetAxis("Mouse Y") * cameraSpeed_Rotation;

            cameraPivot.transform.Rotate(new Vector3(rotY, rotX, 0));

            gameObject.transform.position = cameraPivot.transform.position + cameraPivot.transform.forward * (distance);
            gameObject.transform.LookAt(cameraPivot.transform.position);
            direction = (gameObject.transform.position - cameraPivot.transform.position).normalized;
        };
        #endregion ==================================
    }
}

