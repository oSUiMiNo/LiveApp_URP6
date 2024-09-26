using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RuntimeData;

public class WebCamPV : MonoBehaviour
{

    [SerializeField] GameObject FadePanel;

    public async void Fade(float time_FadeOut, float time_Wait, float time_FadeIn)
    {
        Debug.Log("フェード");

        // パネルの準備
        GameObject panel = Instantiate(FadePanel);
        Color color = panel.GetComponent<Renderer>().material.color;
        panel.GetComponent<Renderer>().material.color = Color.clear;
        panel.transform.parent = _WebCam.transform;
        panel.transform.localPosition = new Vector3(0, 0, 0.35f);
        panel.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        panel.transform.localScale = new Vector3(0.2f, 1, 0.1f);

        // フェードアウト
        panel.GetComponent<Renderer>().material.DOColor(color, time_FadeOut);
        await Delay.Second(time_FadeOut + time_Wait);

        // フェードイン
        panel.GetComponent<Renderer>().material.DOColor(Color.clear, time_FadeIn);
        await Delay.Second(time_FadeIn + 0.1f);
        Destroy(panel);
    }
}
