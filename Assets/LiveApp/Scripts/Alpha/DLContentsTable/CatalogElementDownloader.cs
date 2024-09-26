using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Assertions;
using TMPro;


public class CatalogElementDownloader : MonoBehaviour
{
    public MyButton Controller => gameObject.AddComponent<MyButton>();

    public bool elementClicked = false;

    //[SerializeField] GameObject Dischargable;
    [SerializeField] public string contentName;
    [SerializeField] public string label;
    [SerializeField] public string address;
    [SerializeField] public bool downloaded;


    [SerializeField] public Vector3 Position;
    [SerializeField] public Vector3 Rotation;

    async void Start()
    {   
        Controller.On_Click.Subscribe(async _ =>
        {
            elementClicked = true;
            if (downloaded) return;
            Debug.Log($"ƒNƒŠƒbƒN {Controller.Clicked}");
            DLContentsHandler.Data.ContentsCatalogs[contentName].Labels[label].BeforeDL.Remove(address);
            DLContentsHandler.Data.ContentsCatalogs[contentName].Labels[label].AfterDL.Add(address);
            downloaded = true;
            transform.Find("CheckMark").gameObject.SetActive(true);
            //gameObject.GetComponent<Renderer>().enabled = true;
        });
    }
}
