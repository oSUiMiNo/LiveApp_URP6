using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.AddressableAssets.ResourceLocators;

public class ContentDownloader : MonoBehaviour
{
    [SerializeField] GameObject catalogTable;

    MyButton Controller => gameObject.AddComponent<MyButton>();

    [SerializeField] public string contentName;
    [SerializeField] public string address;
    IResourceLocator resourceLocator;


    void Start()
    {
        Controller.On_Click.Subscribe(async _ =>
        {
            Debug.Log($"ダウンローダー");
            if (!catalogTable.activeSelf)
            {
                Debug.Log($"ダウンローダー1");
                catalogTable.SetActive(true);
            }
            else
            {
                Debug.Log($"ダウンローダー2");
                foreach (var a in catalogTable.GetComponentsInChildren<CatalogItemButton>())
                {
                    Destroy(a.gameObject);
                }
                catalogTable.SetActive(false);
            }

        });

    }
}
