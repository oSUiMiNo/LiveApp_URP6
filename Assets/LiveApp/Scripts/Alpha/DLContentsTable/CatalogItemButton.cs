using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

public class CatalogItemButton : MonoBehaviour
{
    [SerializeField] GameObject labelSelectorObj;
    MyButton Controller => gameObject.AddComponent<MyButton>();


    void Start()
    {
        Controller.On_Click.Subscribe( _ =>
        {
            if (!labelSelectorObj.activeSelf)
            {
                labelSelectorObj.SetActive(true);
                CatalogLabelSelector labelSelector = labelSelectorObj.GetComponent<CatalogLabelSelector>();
                Debug.Log($"{labelSelector.contentName}");
                Debug.Log($"{transform.Find("Label").GetComponent<TextMeshPro>().text}");
                labelSelector.contentName = transform.Find("Label").GetComponent<TextMeshPro>().text;
            }
            else 
            {
                CatalogLabelSelector labelSelector = labelSelectorObj.GetComponent<CatalogLabelSelector>();
                labelSelector.contentName = string.Empty;
                labelSelectorObj.SetActive(false);
            }

            //Debug.Log($"{labelSelector.contentName}");
            //Debug.Log($"{transform.Find("Label").GetComponent<TextMeshPro>().text}");
        });
    }
}
