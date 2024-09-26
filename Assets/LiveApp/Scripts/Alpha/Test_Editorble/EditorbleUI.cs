using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using static InputEventHandler;
using System;
using System.Net;
using UnityEngine.AddressableAssets;
using System.Linq;


public class EditorbleUI : MonoBehaviourMyExtention
{
    MyButton Controller;
    EditorbleHeader editorbleHeader;
    EditorbleTransform editorbleTransform;


    GameObject overlay;

    // 移動
    public IObservable<Vector3> On_Move => on_Move;
    internal Subject<Vector3> on_Move = new Subject<Vector3>();
    // 回転
    public IObservable<Vector3> On_Rotate => on_Rotate;
    internal Subject<Vector3> on_Rotate = new Subject<Vector3>();
    // 90度回転
    public IObservable<Vector3> On_Rotate90 => on_Rotate90;
    internal Subject<Vector3> on_Rotate90 = new Subject<Vector3>();
    // リサイズ
    public IObservable<float> On_ReSize => on_ReSize;
    internal Subject<float> on_ReSize = new Subject<float>();


    bool selecting = false;
    Color baseColor;

    async void Awake()
    {
        //var renderers_Overlay = transform.parent.Find("overlay").GetComponents<Renderer>();
        //foreach (var renderer_Overlay in renderers_Overlay)
        //{
        //    renderer_Overlay.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        //    renderer_Overlay.material.color = Color.blue;
        //    renderer_Overlay.enabled = false;
        //}

        Controller = CheckAddComponent<MyButton>(gameObject);
        editorbleHeader = GetComponent<EditorbleHeader>();
        editorbleTransform = GetComponent<EditorbleTransform>();

        //baseColor = GetComponent<Renderer>().material.color;
        Select(false);

        Controller.On_Click.Subscribe((Action<Unit>)(async _ =>
        {
            Select(true);
            //GetComponent<Renderer>().enabled = false;
            //foreach (var renderer_Overlay in renderers_Overlay)
            //{
            //    renderer_Overlay.enabled = true;
            //}
        })).AddTo(gameObject);

        Controller.On_ClickMargin.Subscribe((Action<Unit>)(async _ =>
        {
            Select(false);
            //GetComponent<Renderer>().enabled = true;
            //foreach (var renderer_Overlay in renderers_Overlay)
            //{
            //    renderer_Overlay.enabled = false;
            //}
        })).AddTo(gameObject);


        // 削除
        OnDown_Delete += () => Delete();

        // リセット
        OnDown_R += () => ResetTrans();

        #region VRMの移動、回転、リサイズ ==================================
        //On_E += () => Move(Vector3.up);
        //On_Q += () => Move(Vector3.down);
        //On_A += () => Move(Vector3.right);
        //On_D += () => Move(Vector3.left);
        //On_W += () => Move(Vector3.back);
        //On_S += () => Move(Vector3.forward);
        //On_E += () => on_Move.OnNext(new Vector3(0, 1, 0));
        On_E += () => on_Move.OnNext(Vector3.up);
        On_Q += () => on_Move.OnNext(Vector3.down);
        On_A += () => on_Move.OnNext(Vector3.right);
        On_D += () => on_Move.OnNext(Vector3.left);
        On_W += () => on_Move.OnNext(Vector3.back);
        On_S += () => on_Move.OnNext(Vector3.forward);

        //OnDown_E += () => Rotate90(Vector3.up);
        //OnDown_Q += () => Rotate90(Vector3.down);
        //OnDown_A += () => Rotate90(Vector3.forward);
        //OnDown_D += () => Rotate90(Vector3.back);
        //OnDown_W += () => Rotate90(Vector3.right);
        //OnDown_S += () => Rotate90(Vector3.left);
        OnDown_E += () => on_Rotate90.OnNext(Vector3.up);
        OnDown_Q += () => on_Rotate90.OnNext(Vector3.down);
        OnDown_A += () => on_Rotate90.OnNext(Vector3.forward);
        OnDown_D += () => on_Rotate90.OnNext(Vector3.back);
        OnDown_W += () => on_Rotate90.OnNext(Vector3.right);
        OnDown_S += () => on_Rotate90.OnNext(Vector3.left);

        //OnDown_0 += () => Angle(Vector3Int.right);
        //OnDown_9 += () => Angle(Vector3Int.right * 2);

        //On_E += () => Rotate(Vector3.up);
        //On_Q += () => Rotate(Vector3.down);
        //On_A += () => Rotate(Vector3.forward);
        //On_D += () => Rotate(Vector3.back);
        //On_W += () => Rotate(Vector3.right);
        //On_S += () => Rotate(Vector3.left);
        On_E += () => on_Rotate.OnNext(Vector3.up);
        On_Q += () => on_Rotate.OnNext(Vector3.down);
        On_A += () => on_Rotate.OnNext(Vector3.forward);
        On_D += () => on_Rotate.OnNext(Vector3.back);
        On_W += () => on_Rotate.OnNext(Vector3.right);
        On_S += () => on_Rotate.OnNext(Vector3.left);

        //On_Z += () => ReSize(-0.1f);
        //On_C += () => ReSize(0.1f);
        On_Z += () => on_ReSize.OnNext(-0.1f);
        On_C += () => on_ReSize.OnNext(0.1f);
        #endregion　==================================

        on_Move.Subscribe(value => Move(value)).AddTo(gameObject);
        on_Rotate.Subscribe(value => Rotate(value)).AddTo(gameObject);
        on_ReSize.Subscribe(value => ReSize(value)).AddTo(gameObject);
        on_Rotate90.Subscribe(value => Rotate90(value)).AddTo(gameObject);
    }

    public void Select(bool select)
    {
        if(select)
        {
            selecting = true;
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            selecting = false;
            GetComponent<Renderer>().enabled = false;
        }
    }

    void Delete()
    {
        if (!selecting) return;
        editorbleHeader.Delete();
    }

    void Move(Vector3 value)
    {
        if (!selecting) return;
        if (Flag_Shift || Flag_Ctrl) return;
        //on_Move.OnNext(direction);
        editorbleTransform.Position += value * 0.05f;
    }

    void Rotate(Vector3 value)
    {
        if (!selecting) return;
        if (!Flag_Ctrl) return;
        //on_Rotate.OnNext(direction);
        editorbleTransform.Rotation += value;
    }

    void Rotate90(Vector3 axis)
    {
        if (!selecting) return;
        if (!Flag_Shift) return;
        //on_Rotate.OnNext(direction);

        float targetValue = 0;
        if (axis == Vector3.up || axis == Vector3.down)
            targetValue = editorbleTransform.Rotation.y;
        else
        if (axis == Vector3.right || axis == Vector3.left)
            targetValue = editorbleTransform.Rotation.x;
        else
        if (axis == Vector3.forward || axis == Vector3.back)
            targetValue = editorbleTransform.Rotation.z;

        float angle = 0;
        // プラスの回転
        if (axis == Vector3.up || axis == Vector3.right || axis == Vector3.forward)
        {
            angle = 90 - targetValue % 90;
        }
        // マイナスの回転
        else
        {
            if (targetValue % 90 == 0) angle = 90;
            else angle = targetValue % 90;
        }

        editorbleTransform.Rotation += axis * angle;
    }

    //void Angle( Vector3Int value )
    //{
    //    editorbleTransform.Angle90 += value;
    //}

    void ReSize(float value)
    {
        if (!selecting) return;
        editorbleTransform.Scale += value;
    }

    void ResetTrans()
    {
        if (!selecting) return;
        editorbleTransform.Rotation = Vector3.zero;
        editorbleTransform.Scale = 1;
    }
}
