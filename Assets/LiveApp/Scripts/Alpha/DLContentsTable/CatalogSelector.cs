using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class CatalogSelector : MonoBehaviour
{
    [SerializeField] GameObject item;
    MyButton Controller => gameObject.AddComponent<MyButton>();


    void Start()
    {
        foreach (var contentsName in DLContentsHandler.Data.ContentsCatalogs.Keys)
        {
            GameObject element = Instantiate(item, transform.Find("Flexible Layout"));
            element.transform.Find("Label").GetComponent<TextMeshPro>().text = contentsName;
            element.SetActive(true);
        }
    }
}
