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
using static RuntimeInputP;
using static VMCState;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class RuntimeInputCV : CV
{
    public override Type ControllerType { get; set; } = null;
    public override P Presenter { get; set; } = new RuntimeInputP();

    
    [SerializeField] float vrmSpeed_Move = 0.12f;
    [SerializeField] float vrmSpeed_Rotation = 2;
    [SerializeField] float cameraSpeed_Perspective = 0.1f;
    [SerializeField] float cameraSpeed_Rotation = 10f;
    [SerializeField] float cameraSpeed_Shift = 0.1f;
    [SerializeField] float cameraPivotOffset = 0.7f;
    

    float cursorZ;
    Transform camPivot;
    Transform camPivot_Vertical;
    public float camDistance;
    public Vector3 camDirection;


    // �J�����̃z�[���|�W�V����
    Vector3 HomePosition_Camera = Vector3.zero;
    Vector3 HomeRotation_Camera = Vector3.zero;


    [SerializeField] List<GameObject> cursor;
    [SerializeField] List<GameObject> effs = new List<GameObject>();
    [SerializeField] Material[] sky;


    protected async sealed override UniTask Awake1()
    {
        camPivot = new GameObject("CameraPivot").transform;
        camPivot_Vertical = new GameObject("CameraPivot_Vertical").transform;
        camPivot_Vertical.SetParent(camPivot);
        camPivot_Vertical.localPosition = Vector3.zero;

        cursor = GetChildren(GameObject.Find("Cursor")).ToList();
        cursorZ = MathF.Abs(cursor[0].transform.localPosition.z);
        effs.ForEach(a => a.SetActive(false));
        Profiler.SetActive(false);

        SetCAM();
        OnLoadAvatar.Subscribe(_ => SetCAM());

        OnSW_Sky.Subscribe(value => SW_Sky(value));
        OnSW_Cursor.Subscribe(value => SW_Cursor(value));
        //OnSW_Avatar.Subscribe(value => SW_Avatar(value));
        OnSW_WorldEff.Subscribe(value => SW_WorldEff(value));
        OnSW_Profiler.Subscribe(_ => Profiler.SetActive(!Profiler.activeSelf));

        OnVRM_Move.Subscribe(value =>
        {
            VRM_Move(value);
            FollowCAM();
        });
        OnVRM_Rotate.Subscribe(value => VRM_Rotate(value));

        OnCAM_Home.Subscribe(_ => CAM_Home());
        OnCAM_Distance.Subscribe(_ => CAM_Distance());
        OnCAM_Hold.Subscribe(_ => CAM_Hold());
        //OnCAM_Around
        //    .Buffer(OnCAM_Around.Throttle(TimeSpan.FromSeconds(2))) // 1�b�ȏ㉟����Ȃ���΃��Z�b�g
        //    .Where(buffer => buffer.Count > 1) // 1���ڂ̏����͖�������
        //    .Subscribe(_ => CAM_Around());
        OnCAM_Around.Subscribe(_ => Debug.Log("�J������]0"));


        // ���������̏���
        OnCAM_Around
            .Skip(1) // �ŏ���1��𖳎�
            .TimeInterval() // �C�x���g�̊Ԋu���v��
            .Where(interval => interval.Interval.TotalSeconds < 1) // 1�b�ȓ��Ɏ���OnNext�������璷�������Ɣ��f
            .Subscribe(_ =>
            {
                CAM_Around();
            })
            .AddTo(this);


        await Presenter.Execute();


        // �e�X�g
        InputEventHandler.OnDown_L += () => NagesenHanabi();
    }


    protected async sealed override void Update()
    {
        FollowCursor();
    }


    // �A�o�^�[�C���|�[�g���̏���
    void SetCAM()
    {
        Debug.Log("�J�������Z�b�g");
        // Debug.Log($"�J�����̈ʒu���A�o�^�[�ɍ��킹��");
        camPivot.position = _Avatar.transform.position + Vector3.up * cameraPivotOffset;
        Camera.main.transform.position = camPivot.position + Vector3.forward * 2;
        camDistance = (Camera.main.transform.position - camPivot.position).magnitude;
        camDirection = (Camera.main.transform.position - camPivot.position).normalized;
        
        // �J�����̃z�[���|�W�V�����ݒ�
        HomePosition_Camera = camPivot.position + Vector3.forward * 2;
        HomeRotation_Camera = Camera.main.transform.rotation.eulerAngles;
    }


    GameObject[] GetChildren(GameObject parent)
    {
        // �e�q�I�u�W�F�N�g���i�[����z��쐬
        var children = new GameObject[parent.transform.childCount + 1];
        // 0�ɂ͐e�I�u�W�F�N�g�����܂�
        children[0] = parent;
        // 0�`��-1�܂ł̎q�����Ԃɔz��Ɋi�[
        for (var i = 0; i < children.Length - 1; ++i)
        {
            children[i + 1] = parent.transform.GetChild(i).gameObject;
        }
        // �e�q�I�u�W�F�N�g���i�[���ꂽ�z��
        return children;
    }

    Camera Cam_UI => GameObject.Find("Cam_UI").GetComponent<Camera>();

    void FollowCursor()
    {
        Vector3 touchScreenPosition = Input.mousePosition;
        touchScreenPosition.z = cursorZ;

        //Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(touchScreenPosition);
        Vector3 touchWorldPosition = Cam_UI.ScreenToWorldPoint(touchScreenPosition);

        // �J�[�\���A�C�R���̃g�����X�t�H�[���ݒ�
        cursor[0].transform.position = touchWorldPosition;
        //cursor[0].transform.forward = Camera.main.transform.forward;
        cursor[0].transform.forward = Cam_UI.transform.forward;
    }


    void FollowCAM()
    {
        // �J�����̃A�o�^�[����̑��Έʒu
        Vector3 relativePos = Camera.main.transform.position - camPivot.position;

        // �J�������A�o�^�[�ƈꏏ�Ɉړ�
        camPivot.position = 
            _Avatar.transform.position + Vector3.up * cameraPivotOffset;
        Camera.main.transform.position = camPivot.position + relativePos;
        
        // �z�[���|�W�V�������X�V�@�A�o�^�[���瑊�ΓG�ɏ����̃z�[���|�W�W�����Ɠ����ɂȂ�悤��
        HomePosition_Camera = camPivot.position + Vector3.forward * 2;
    }


    // �A�o�^�[�ړ�
    void VRM_Move(Vector3 direction)
    {
        _Avatar.transform.position += direction * vrmSpeed_Move;
    }


    // �A�o�^�[��]
    void VRM_Rotate(Vector3 axis)
    {
        _Avatar.transform.Rotate(axis * vrmSpeed_Rotation);
    }


    void CAM_Home()
    {
        Camera.main.transform.position = HomePosition_Camera;
        Camera.main.transform.rotation = Quaternion.Euler(HomeRotation_Camera);
    }
    
    void CAM_Hold()
    {
        float rotX = Input.GetAxis("Mouse X") * cameraSpeed_Shift;
        float rotY = Input.GetAxis("Mouse Y") * cameraSpeed_Shift;
        Camera.main.transform.position += Camera.main.transform.rotation * new Vector3(-rotX, -rotY, 0);
        camDistance = (Camera.main.transform.position - camPivot.position).magnitude;
        camDirection = (Camera.main.transform.position - camPivot.position).normalized;
    }
    
    void CAM_Around()
    {
        Debug.Log("�J������]1");
        float rotX = Input.GetAxis("Mouse X") * cameraSpeed_Rotation;
        float rotY = Input.GetAxis("Mouse Y") * cameraSpeed_Rotation;
        //float rotX = Input.mousePositionDelta.x * cameraSpeed_Shift;
        //float rotY = Input.mousePositionDelta.y * cameraSpeed_Shift;
        //camPivot.transform.Rotate(new Vector3(rotY, rotX, 0));
        //Camera.main.transform.position = camPivot.transform.position + camPivot.transform.forward * (camDistance);
        //Camera.main.transform.LookAt(camPivot.transform.position);
        //camDirection = (Camera.main.transform.position - camPivot.transform.position).normalized;
        camPivot_Vertical.RotateAround(camPivot.position, camPivot.up, rotX * 0.4f);
        Camera.main.transform.RotateAround(camPivot.position, camPivot_Vertical.right, rotY * 0.4f);
        Camera.main.transform.RotateAround(camPivot.position, camPivot.up, rotX * 0.4f);
    }

    void CAM_Distance()
    {
        Camera.main.transform.position +=
              Camera.main.transform.forward * Input.mouseScrollDelta.y * cameraSpeed_Perspective;
        camDistance = Vector3.Distance(Camera.main.transform.position, camPivot.position);
    }


    // �X�J�C�{�b�N�X���X�C�b�`
    void SW_Sky(int num)
    {
        RenderSettings.skybox = sky[num];
    }


    // ���[���h�G�t�F�N�g���X�C�b�`
    void SW_WorldEff(int num)
    {
        if (!effs[num].activeSelf) effs[num].SetActive(true);
        else effs[num].SetActive(false);
    }


    // �J�[�\���A�C�R�����X�C�b�`
    void SW_Cursor(int CursorNumber)
    {
        foreach (var icon in cursor)
        {
            if (icon == cursor[0]) continue;
            icon.SetActive(false);
        }
        if (CursorNumber == 0) Cursor.visible = true;
        else
        {
            cursor[CursorNumber].SetActive(true);
            Cursor.visible = false;
        }
    }


    public async void NagesenHanabi()
    {
        int random = new System.Random().Next(0, 4);

        GameObject hanabi = Instantiate(await Addressables.LoadAssetAsync<GameObject>($"NagesenHanabi{random}"));

        hanabi.transform.parent = _Avatar.transform;
        hanabi.transform.localPosition = new Vector3(0, 0f, -0.2f);
        hanabi.transform.localScale = Vector3.one * 0.2f;
        await Delay.Second(8);
        Destroy(hanabi);
    }
}

