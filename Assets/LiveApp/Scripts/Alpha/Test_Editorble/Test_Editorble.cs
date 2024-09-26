using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class Test_Editorble : MonoBehaviourMyExtention
{
    [SerializeField] string catalogDir;
    [SerializeField] string bundleDir;
    List<IResourceLocator> resourceLocators = new List<IResourceLocator>();


    async void Start()
    {
        foreach(var a in DLContentsHandler.Data.ContentsCatalogs.Values)
        {
            // �V�����J�^���O���擾�B�t�@�C���p�X��URL
            AsyncOperationHandle<IResourceLocator> requestCatalog
                = Addressables.LoadContentCatalogAsync(a.Path_Catalog);
            Debug.Log($"�p�X�J�^���O{a.Path_Catalog}" );
            // ���[�h������҂�
            await requestCatalog;
            // ���̃G���[��������
            Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);
            resourceLocators.Add(requestCatalog.Result);
        }
        foreach (var a in resourceLocators)
        {
            Addressables.AddResourceLocator(a);
        }
        
       


        //EditorblesHandler
        await Reconstruct();


        foreach (var a in resourceLocators)
        {
            Addressables.RemoveResourceLocator(a);
        }

        //InputEventHandler.OnDown_1 += async () =>
        //{
        //    await Discharge("TestCylinder");
        //};
        //InputEventHandler.OnDown_2 += async () =>
        //{
        //    //await Discharge("TestPinokio");
        //};
        //InputEventHandler.OnDown_3 += async () =>
        //{
        //};
        //await Reconstruct();

        //catalogDir = @$"{Application.dataPath}/ObjectTable/Contents";
        //bundleDir = $@"{Addressables.RuntimePath}/StandaloneWindows64";
        //Debug.Log($"{bundleDir}");
        //string catalogPath = $@"{catalogDir}/catalog_0.1.0.json";

        ////Debug.Log($"{resourceLocator.}");

        //// �V�����J�^���O���擾�B�t�@�C���p�X��URL
        //AsyncOperationHandle<IResourceLocator> requestCatalog
        //    = Addressables.LoadContentCatalogAsync(catalogPath);
        //// ���[�h������҂�
        //await requestCatalog;
        //// ���̃G���[��������
        //Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);

        //resourceLocator = requestCatalog.Result;
        //Debug.Log($"{resourceLocator.LocatorId}");


        //List<string> groupPaths = new List<string>();
        //Debug.Log($"===============================================");

        //foreach (var a in resourceLocator.Keys)
        //{
        //    IList<IResourceLocation> resourceLocations;
        //    // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
        //    // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
        //    // �܂�J�^���O�� IResourceLocation �����Ă���

        //    // typeof()�̒��g�́A�A�v����DL�A�Z�b�g�Ƃ��ċ����^
        //    if (!resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) continue;
        //    Debug.Log($"�A�h���X {a}");

        //}

        //// �J�^���O�Ɋ܂܂��S�A�Z�b�g�o���h���O���[�v�̃p�X��؂�o��
        //foreach (var a in resourceLocator.Keys)
        //{
        //    IList<IResourceLocation> resourceLocations;
        //    // IResourceLocation �̓A�Z�b�g�����[�h���邽�߂ɕK�v�ȏ��������Ă���
        //    // IResourceLocator �� Locate() �ɓ���̃A�h���X��n���� IResourceLocation ���Ԃ��Ă���
        //    // �܂�J�^���O�� IResourceLocation �����Ă���

        //    // typeof()�̒��g�́A�A�v����DL�A�Z�b�g�Ƃ��ċ����^
        //    if (resourceLocator.Locate(a, typeof(GameObject), out resourceLocations)) break;
        //    if (resourceLocator.Locate(a, typeof(Animator), out resourceLocations)) break;
        //    if (resourceLocator.Locate(a, typeof(Skybox), out resourceLocations)) break;

        //    // �O���[�v�̃p�X��Keys�̏��ՂɏW�܂��Ă��āA���̌�� address�B��������ۂ�
        //    // �Ȃ̂ŁALocate(Keys) �� true �ɂȂ�܂Łi�܂�address�̑O�܂Łj�� Keys ��؂�o���΃O���[�v�̃p�X�̃��X�g������
        //    groupPaths.Add($"{bundleDir}/{a.ToString()}");
        //    Debug.Log($"------- �O���[�v�p�X {bundleDir}/{a.ToString()}");
        //}
    }

    async UniTask Reconstruct()
    {
        foreach (var address in Editorbles.Ins.IDListDict.Keys)
            foreach (var id in Editorbles.Ins.IDListDict[address])
            {
                Debug.Log("�����ł��[");
                Debug.Log(id);
                //Addressables.AddResourceLocator(resourceLocator);
                await Discharge(address, id);
                //Addressables.RemoveResourceLocator(resourceLocator);
            }
    }


    public async UniTask Discharge(string address, string id = null)
    {
        Pool_ClickMarker pool_ImpactOverlay = GetComponent<Pool_ClickMarker>();

        GameObject exterior = await Addressables.InstantiateAsync("FunitureExterior");
        //exterior.SetActive(false);

        GameObject dischargable = await Addressables.LoadAssetAsync<GameObject>(address);
        GameObject body = Instantiate(dischargable);
        body.transform.SetParent(exterior.transform);

        EditorbleCollider editorbleCollider = exterior.GetComponent<EditorbleCollider>();
        editorbleCollider.IniPos = Vector3.zero;
        editorbleCollider.Init();

        await Delay.Second(1);
        Debug.Log($"�f�B�X�`���[�W0");


       

        var editorbleHeader = exterior.AddComponent<EditorbleHeader>();
        if (!string.IsNullOrEmpty(id)) editorbleHeader.SetID(id, address);
        else editorbleHeader.CreateID(address);

        EditorblesHandler.headers.Add(editorbleHeader);

        EditorbleTransform ediTrans = exterior.AddComponent<EditorbleTransform>();

        exterior.AddComponent<EditorbleUI>();

        //SetTag(exterior.transform, label);

        await Delay.Second(1);
        Debug.Log($"�f�B�X�`���[�W1");

        //ediTrans.Position = pos;
        ediTrans.BaseScale = exterior.transform.localScale;

        ediTrans.LoadEditorble();
        await Delay.Second(2);
        Debug.Log($"�f�B�X�`���[�W2");

        await Delay.Second(2);
        Debug.Log($"�f�B�X�`���[�W4");
        ediTrans.LoadEditorble();

        exterior.SetActive(true);
    }

    //async UniTask Reconstruct()
    //{
    //    foreach (var address in Editorbles.Ins.IDListDict.Keys)
    //        foreach (var id in Editorbles.Ins.IDListDict[address])
    //            await Discharge(address, id);
    //}


    //async UniTask Discharge(string address, string id = null)
    //{
    //    GameObject content = new GameObject($"Object_{address}");

    //    GameObject overlay = await Addressables.InstantiateAsync(address, content.transform);
    //    TransReset_Local(overlay.transform);
    //    overlay.name = "overlay";
    //    foreach (var a in overlay.GetComponentsInChildren(typeof(Collider)))
    //    {
    //        Destroy(a);
    //    }

    //    var renderers_Overlay = overlay.GetComponents<Renderer>();
    //    foreach (var renderer_Overlay in renderers_Overlay)
    //    {
    //        renderer_Overlay.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
    //        renderer_Overlay.material.color = Color.blue;
    //        renderer_Overlay.enabled = false;
    //    }

    //    GameObject body = await Addressables.InstantiateAsync(address, content.transform);
    //    TransReset_Local(overlay.transform);
    //    body.name = "body";

    //    var editorbleHeader = content.AddComponent<EditorbleHeader>();
    //    if (!string.IsNullOrEmpty(id)) editorbleHeader.SetID(id, address);
    //    else editorbleHeader.CreateID(address);
    //    EditorblesHandler.headers.Add(editorbleHeader);

    //    var editorbleTransform = content.AddComponent<EditorbleTransform>();
    //    var editorbleUI = body.AddComponent<EditorbleUI>();
    //    editorbleTransform.Init();
    //}

    void SetTag(Transform parent, string tagName)
    {
        parent.tag = tagName;
        foreach (Transform child in parent) SetTag(child, tagName);
    }
}
