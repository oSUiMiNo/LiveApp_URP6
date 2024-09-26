using Flexalon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectTable : MonoBehaviour
{
    [SerializeField] public Vector3 elemetSize = new Vector3(0.5f, 0.5f, 0.07f);
    [SerializeField] float rowGap = 0.5f;
    [SerializeField] float colGap = 0.5f;

    [SerializeField] public List<GameObject> specimens = new List<GameObject>();
    [SerializeField] public FlexalonGridLayout flexalonLayout;
    [SerializeField] public FlexalonObject flexalonObject;
    [SerializeField] public Transform backGround;

    void Awake()
    {
        gameObject.name = "ObjectTable";

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);

            //if (child.GetComponent<Specimen>()) specimens.Add(child.gameObject);
            //else
            if (child.name == "Grid Layout")
            {
                flexalonLayout = child.GetComponent<FlexalonGridLayout>();
                flexalonObject = child.GetComponent<FlexalonObject>();
            }
            else
            if (child.name == "BackGround") backGround = child;
        }


        //foreach(var a in specimens) a.GetComponent<ChildObjBounds>().size = elemetSize;

        flexalonLayout.RowSpacing = rowGap;
        flexalonLayout.ColumnSpacing = colGap;
        flexalonObject.Scale = elemetSize;

        float backGroundX = (colGap * elemetSize.x * flexalonLayout.Columns + elemetSize.x * flexalonLayout.Columns);
        float backGroundY = (rowGap * elemetSize.y * flexalonLayout.Rows + elemetSize.y * flexalonLayout.Rows);
        //Debug.Log($"{backGroundX} {backGroundY}");
        backGround.localScale = new Vector3(backGroundX, backGroundY, 1);
        //backGround.localScale = Vector3.one * 1.2f;
        backGround.localPosition = new Vector3(0, 0, (elemetSize.z / 2) + 0.04f);
    }

    //private void Start()
    //{
     
    //}
}
