using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using System.Linq;

public class CatalogLabelSelector : MonoBehaviour
{
    [SerializeField] public string contentName;
    [SerializeField] List<ContentTableCreator> creator => GetComponentsInChildren<ContentTableCreator>().ToList();


    void Start()
    {
        foreach (var discharger in creator)
        {
            //Debug.Log($"{discharger.gameObject}");
            discharger.contentName = contentName;
            discharger.gameObject.SetActive(true);
            discharger.label = discharger.gameObject.transform.Find("Label").GetComponent<TextMeshPro>().text;
        };
    }
}
