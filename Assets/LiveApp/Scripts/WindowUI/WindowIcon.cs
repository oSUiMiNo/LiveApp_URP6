using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class WindowIcon : MonoBehaviourMyExtention
{
    WindowUI windowUI;
    [SerializeField] GameObject altTabWindows;


    private void Start()
    {
        altTabWindows = GameObject.Find("Cam_UI").transform.Find("AltTabWIndows").gameObject;
        windowUI = transform.parent.parent.GetComponent<WindowUI>();

        Button button = GetComponent<Button>();
        button.OnClickAsObservable().Subscribe(_ => 
        {
            altTabWindows.SetActive(true);
        });
    }
}
 