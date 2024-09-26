using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class SelectTableElement : MonoBehaviour
{

    GameObject objectTable;
    MyButton Controller => gameObject.AddComponent<MyButton>();

    void Awake()
    {
        //objectTable.transform.position = new Vector3(0.5f, 0.2f, 14);
    }

    async void Start()
    {
        objectTable = await Addressables.InstantiateAsync("ObjectTableBody");
        objectTable.SetActive(false);

        Controller.Clicked.Subscribe(value =>
        {
            if (value) objectTable.SetActive(!objectTable.activeSelf);
        });

        InputEventHandler.OnDown_0 += async () =>
        {
            GameObject specimen = await Addressables.InstantiateAsync("SpecimenBody");
            specimen.SetActive(false);
            specimen.name = "Specimen";
            specimen.transform.SetParent(objectTable.transform);
            specimen.AddComponent<Specimen>().createCollider = true;
            specimen.AddComponent<CatalogElementDischarger>().address = "Celestia_v.2.0.0_URP.prefab";
           
            GameObject sample = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sample.SetActive(false);
            sample.name = "Sample";
            sample.transform.SetParent(specimen.transform);
            
            sample.SetActive(true);
            specimen.SetActive(true);
        };
    }
}
