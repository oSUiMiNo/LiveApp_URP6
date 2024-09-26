using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
#endif

// UIボタンのサイズ倍率
[ExecuteInEditMode]
[System.Serializable]
public class ScaleMagnifire_MenuButton : MonoBehaviour
{
    GameObject target;
    Vector3 firstScale;

    public Vector3 Magnification = Vector3.one;
    public float MagmitudeMagnification = 1f;

    FloatReactiveProperty magmitudeMagnification = new FloatReactiveProperty(1);
    FloatReactiveProperty XMagnification = new FloatReactiveProperty(1);
    FloatReactiveProperty YMagnification = new FloatReactiveProperty(1);
    FloatReactiveProperty ZMagnification = new FloatReactiveProperty(1);

    private async void Awake()
    {
        target = transform.Find("Icon").gameObject;
        firstScale = target.transform.localScale;

        A();
    }


    private void Update()
    {
        ReflectProp_2_Reactive();
    }

    void A()
    {
        magmitudeMagnification.Subscribe(value =>
        {
            target.transform.localScale = firstScale * value;
            //XMagnification.Value = YMagnification.Value = ZMagnification.Value = value;
        });

        XMagnification.Subscribe(value =>
        {
            firstScale.x = firstScale.x * value;
            target.transform.localScale = firstScale;
        });
        YMagnification.Subscribe(value =>
        {
            firstScale.y = firstScale.y * value;
            target.transform.localScale = firstScale;
        });
        ZMagnification.Subscribe(value =>
        {
            firstScale.z = firstScale.z * value;
            target.transform.localScale = firstScale;
        });
    }

    void ReflectProp_2_Reactive()
    {
        magmitudeMagnification.Value = MagmitudeMagnification;
        XMagnification.Value = Magnification.x;
        YMagnification.Value = Magnification.y;
        ZMagnification.Value = Magnification.z;
    }
}
