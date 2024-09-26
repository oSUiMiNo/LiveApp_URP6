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

public class CatalogElementDischarger : MonoBehaviourMyExtention
{
    public MyButton Controller => CheckAddComponent<MyButton>(gameObject);

    public bool elementClicked = false;

    [SerializeField] public string contentName;
    [SerializeField] public string label;
    [SerializeField] public string address;
    IResourceLocator resourceLocator;

    [SerializeField] public GameObject dischargable;
    [SerializeField] public RaycastHit hitInfo;

    [SerializeField] public Vector3 Position;
    [SerializeField] public Vector3 Rotation;

    async void Start()
    {
        // 新しいカタログを取得。ファイルパスかURL
        AsyncOperationHandle<IResourceLocator> requestCatalog
            = Addressables.LoadContentCatalogAsync(DLContentsHandler.Data.ContentsCatalogs[contentName].Path_Catalog);
        // ロード完了を待つ
        await requestCatalog;
        // 何のエラー処理だろ
        Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);
        resourceLocator = requestCatalog.Result;

        Addressables.AddResourceLocator(resourceLocator);
        dischargable = await Addressables.LoadAssetAsync<GameObject>(address);
        Addressables.RemoveResourceLocator(resourceLocator);



        Controller.On_Click.Subscribe(async _ =>
        {
            elementClicked = true;
            Discharge(hitInfo);
        });
    }


    public async void Discharge(RaycastHit hitInfo, string id = null)
    {
        Pool_ClickMarker pool_ImpactOverlay = GetComponent<Pool_ClickMarker>();
        Vector3 pos = hitInfo.point + hitInfo.normal * dischargable.transform.localScale.y / 2;

        GameObject exterior = await Addressables.InstantiateAsync("FunitureExterior");
        //exterior.SetActive(false);

        GameObject body = Instantiate(dischargable);
        body.transform.SetParent(exterior.transform);

        EditorbleCollider editorbleCollider = exterior.GetComponent<EditorbleCollider>();
        editorbleCollider.IniPos = Vector3.zero;
        editorbleCollider.Init();

        //await Delay.Second(1);
        Debug.Log($"ディスチャージ0");


        /*
        //if (hitInfo.normal.x == 1)
        //{
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.up, 90);
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.forward, -90);
        //}
        //if (hitInfo.normal.x == -1)
        //{
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.up, -90);
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.forward, 90);
        //}
        //if (hitInfo.normal.y == -1)
        //{
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.forward, 180);
        //}
        //if (hitInfo.normal.z == 1)
        //{
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.right, 90);
        //}
        //if (hitInfo.normal.z == -1)
        //{
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.up, 180);
        //    exterior.transform.RotateAround(exterior.transform.position, Vector3.right, -90);
        //}
        */

        var editorbleHeader = exterior.AddComponent<EditorbleHeader>();
        if (!string.IsNullOrEmpty(id)) editorbleHeader.SetID(id, address);
        else editorbleHeader.CreateID(address);

        EditorblesHandler.headers.Add(editorbleHeader);

        EditorbleTransform ediTrans = exterior.AddComponent<EditorbleTransform>();
        
        exterior.AddComponent<EditorbleUI>();

        SetTag(exterior.transform, label);

        //await Delay.Second(1);
        Debug.Log($"ディスチャージ1");

        ediTrans.Position = pos;
        ediTrans.BaseScale = exterior.transform.localScale;

        ediTrans.LoadEditorble();
        //await Delay.Second(2);
        Debug.Log($"ディスチャージ2");

        if (hitInfo.normal.x == 1)
        {
            ediTrans.Rotation += Vector3.up * 90;
            ediTrans.Rotation += Vector3.forward * -90;
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.up, 90);
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.forward, -90);
        }
        if (hitInfo.normal.x == -1)
        {
            ediTrans.Rotation += Vector3.up * 270;
            //await Delay.Second(2);
            Debug.Log($"ディスチャージ3");
            ediTrans.Rotation += Vector3.forward * 90;
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.up, -90);
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.forward, 90);
        }
        if (hitInfo.normal.y == -1)
        {
            ediTrans.Rotation += Vector3.forward * 180;
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.forward, 180);
        }
        if (hitInfo.normal.z == 1)
        {
            ediTrans.Rotation += Vector3.right * 90;
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.right, 90);
        }
        if (hitInfo.normal.z == -1)
        {
            ediTrans.Rotation += Vector3.up * 180;
            ediTrans.Rotation += Vector3.right * -90;
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.up, 180);
            //exterior.transform.RotateAround(exterior.transform.position, Vector3.right, -90);
        }

        //await Delay.Second(2);
        Debug.Log($"ディスチャージ4");
        ediTrans.LoadEditorble();

        exterior.SetActive(true);
    }


    void SetTag(Transform parent, string tagName)
    {
        parent.tag = tagName;
        foreach (Transform child in parent) SetTag(child, tagName);
    }
}