using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using TMPro;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Net;
using VRM;

public class DischargeTableCreator_Funiture : MonoBehaviourMyExtention
{
    [SerializeField] public RaycastHit hitInfo;

    GameObject table;
    ObjectTable objectTable;
    [SerializeField] public string label;
    IResourceLocator resourceLocator;

    bool clickedMargin, clickedTableMargin = false;


    MyButton Controller => CheckAddComponent<MyButton>(gameObject);
    MyButton tableController;

    async void Start()
    {
        await Delay.Frame(1);
        label = transform.Find("Label").GetComponent<TextMeshPro>().text;

        Debug.Log($"ラベル　{label}");

        //foreach (var contentName in CatalogsHandler.Data.ContentsCatalogs.Keys)
        //{
            //// 新しいカタログを取得。ファイルパスかURL
            //AsyncOperationHandle<IResourceLocator> requestCatalog
            //    = Addressables.LoadContentCatalogAsync(CatalogsHandler.Data.ContentsCatalogs[contentName].Path_Catalog);
            //// ロード完了を待つ
            //await requestCatalog;
            //// 何のエラー処理だろ
            //Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);
            ////resourceLocator = requestCatalog.Result;
            //Addressables.AddResourceLocator(requestCatalog.Result);

            //table = await Addressables.InstantiateAsync("ObjectTableBody");
            //table.transform.localPosition = new Vector3(-1, 0.5f, 11);
            //table.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            //objectTable = table.AddComponent<ObjectTable>();
            //table.SetActive(false);
            //tableController = table.transform.Find("BackGround").gameObject.AddComponent<MyButton>();
            //Debug.Log($"コンテントネーム０{contentName}");

            //Controller.On_Click.Subscribe(async _ =>
            //{
            //    await Delay.Frame(1);
            //    if (table.activeSelf)
            //    {
            //        foreach (var specimen in objectTable.specimens)
            //        {
            //            Destroy(specimen);
            //        }
            //        objectTable.specimens = new List<GameObject>();
            //        table.SetActive(false);
            //    }
            //    else
            //    {
            //        table.SetActive(true);
            //        table.transform.Find("BackGround/Text").gameObject.SetActive(true);

            //        //CatalogDataHandler.Data.Load();
            //        Debug.Log($"コンテントネーム１{contentName}");
            //        Debug.Log($"{CatalogsHandler.Data.ContentsCatalogs[contentName].Labels[label].BeforeDL.Count}");
            //        Debug.Log($"{CatalogsHandler.Data.ContentsCatalogs[contentName].Labels[label].AfterDL.Count}");

            //        foreach (var a in CatalogsHandler.Data.ContentsCatalogs[contentName].Labels[label].AfterDL)
            //        {
            //            GameObject specimen = await Addressables.InstantiateAsync("SpecimenBody");
            //            specimen.SetActive(false);
            //            specimen.name = "Specimen";
            //            specimen.transform.SetParent(table.transform);
            //            specimen.AddComponent<Specimen>().createCollider = true;
            //            objectTable.specimens.Add(specimen);

            //            CatalogElementDischarger discharger = specimen.AddComponent<CatalogElementDischarger>();
            //            discharger.contentName = contentName;
            //            discharger.label = label;
            //            discharger.address = a;
            //            discharger.hitInfo = hitInfo;

            //            Addressables.AddResourceLocator(resourceLocator);
            //            GameObject sample = await Addressables.InstantiateAsync(a);
            //            Addressables.RemoveResourceLocator(resourceLocator);
            //            sample.AddComponent<MyMeshOptimizer_VRM>();
            //            sample.SetActive(false);
            //            sample.name = "Sample";
            //            sample.transform.SetParent(specimen.transform);
            //            sample.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

            //            sample.SetActive(true);
            //            specimen.SetActive(true);
            //            SetLayer(specimen.transform, "UI");
            //        }
            //    }
            //    SetLayer(table.transform, "UI");
            //    table.transform.Find("BackGround/Text").gameObject.SetActive(false);
            //});

            //Controller.On_ClickMargin.Subscribe(_ =>
            //{
            //    foreach (var specimen in table.GetComponent<ObjectTable>().specimens)
            //    {
            //        if (specimen.GetComponent<CatalogElementDischarger>().elementClicked)
            //        {
            //            specimen.GetComponent<CatalogElementDischarger>().elementClicked = false;
            //            return;
            //        }
            //    }
            //    if (tableController.Clicked.Value)
            //    {
            //        return;
            //    }

            //    foreach (var specimen in objectTable.specimens)
            //    {
            //        Destroy(specimen);
            //    }
            //    objectTable.specimens = new List<GameObject>();
            //    table.SetActive(false);
            //});
        //}

        table = await Addressables.InstantiateAsync("ObjectTableBody");
        table.transform.localPosition = new Vector3(-1, 0.5f, 11);
        table.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        objectTable = table.AddComponent<ObjectTable>();
        table.SetActive(false);
        tableController = table.transform.Find("BackGround").gameObject.AddComponent<MyButton>();
       
        Controller.On_Click.Subscribe(async _ =>
        {
            foreach (var contentName in DLContentsHandler.Data.ContentsCatalogs.Keys)
            {
                // 新しいカタログを取得。ファイルパスかURL
                AsyncOperationHandle<IResourceLocator> requestCatalog
                    = Addressables.LoadContentCatalogAsync(DLContentsHandler.Data.ContentsCatalogs[contentName].Path_Catalog);
                // ロード完了を待つ
                await requestCatalog;
                // 何のエラー処理だろ
                Assert.AreEqual(AsyncOperationStatus.Succeeded, requestCatalog.Status);
                //resourceLocator = requestCatalog.Result;
                Addressables.AddResourceLocator(requestCatalog.Result);

                await Delay.Frame(1);
                if (table.activeSelf)
                {
                    foreach (var specimen in objectTable.specimens)
                    {
                        Destroy(specimen);
                    }
                    objectTable.specimens = new List<GameObject>();
                    table.SetActive(false);
                }
                else
                {
                    table.SetActive(true);
                    table.transform.Find("BackGround/Text").gameObject.SetActive(true);

                    //CatalogDataHandler.Data.Load();
                    Debug.Log($"コンテントネーム１{contentName}");
                    Debug.Log($"{DLContentsHandler.Data.ContentsCatalogs[contentName].Labels[label].BeforeDL.Count}");
                    Debug.Log($"{DLContentsHandler.Data.ContentsCatalogs[contentName].Labels[label].AfterDL.Count}");

                    foreach (var a in DLContentsHandler.Data.ContentsCatalogs[contentName].Labels[label].AfterDL)
                    {
                        GameObject specimen = await Addressables.InstantiateAsync("SpecimenBody");
                        specimen.SetActive(false);
                        specimen.name = "Specimen";
                        specimen.transform.SetParent(table.transform);
                        specimen.AddComponent<Specimen>().createCollider = true;
                        objectTable.specimens.Add(specimen);

                        CatalogElementDischarger discharger = specimen.AddComponent<CatalogElementDischarger>();
                        discharger.contentName = contentName;
                        discharger.label = label;
                        discharger.address = a;
                        discharger.hitInfo = hitInfo;

                        Addressables.AddResourceLocator(resourceLocator);
                        GameObject sample = await Addressables.InstantiateAsync(a);
                        Addressables.RemoveResourceLocator(resourceLocator);
                        sample.AddComponent<MyMeshOptimizer_VRM>();
                        sample.SetActive(false);
                        sample.name = "Sample";
                        sample.transform.SetParent(specimen.transform);
                        sample.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

                        sample.SetActive(true);
                        specimen.SetActive(true);
                        SetLayer(specimen.transform, "UI");
                    }
                }
                SetLayer(table.transform, "UI");
                table.transform.Find("BackGround/Text").gameObject.SetActive(false);
            }
        });


        Controller.On_ClickMargin.Subscribe(_ =>
        {
            foreach (var specimen in table.GetComponent<ObjectTable>().specimens)
            {
                if (specimen.GetComponent<CatalogElementDischarger>().elementClicked)
                {
                    specimen.GetComponent<CatalogElementDischarger>().elementClicked = false;
                    return;
                }
            }
            if (tableController.Clicked.Value)
            {
                return;
            }

            foreach (var specimen in objectTable.specimens)
            {
                Destroy(specimen);
            }
            objectTable.specimens = new List<GameObject>();
            table.SetActive(false);
        });
    }


    static async UniTask<GameObject> LoadVRMAsync(string path)
    {
        if (path == null) return null;
        var instance = await VrmUtility.LoadAsync(path);

        // Rendererがオフになっていることがあるので全部オンにする
        SkinnedMeshRenderer[] components = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer component in components)
        {
            component.enabled = true;
        }
        return instance.gameObject;
    }

    void SetLayer(Transform parent, string layerName)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
            SetLayer(child, layerName);
        }
    }
}
