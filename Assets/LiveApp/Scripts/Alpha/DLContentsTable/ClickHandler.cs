using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class ClickHandler : SingletonCompo<ClickHandler>
{
    public GameObject Room;
    List<GameObject> rightClickTargets = new List<GameObject>();

    public static Subject<RaycastHit> On_RightClick = new Subject<RaycastHit>();
    public static Subject<RaycastHit> On_LeftClick = new Subject<RaycastHit>();
    public static Subject<RaycastHit> On_DoubleClick = new Subject<RaycastHit>();
    public static Subject<Unit> On_ClickMargin = new Subject<Unit>();

    [SerializeField] RightClickMenu rightClickMenu;




    protected sealed override void Start()
    {
        AddColli(Room.transform);

        InputEventHandler.OnDown_MouseRight += () =>
        {
            (bool, RaycastHit) hit = ClickColli();
            if (hit.Item1)
            {
                On_RightClick.OnNext(hit.Item2);
            }
            else
            {
                On_ClickMargin.OnNext(Unit.Default);
            }
        };

        InputEventHandler.OnDown_MouseLeft += () =>
        {
            (bool, RaycastHit) hit = ClickColli();
            if (hit.Item1)
            {
                On_LeftClick.OnNext(hit.Item2);
            }
            else
            {
                On_ClickMargin.OnNext(Unit.Default);
            }
        };

        InputEventHandler.On_MouseDouble += () =>
        {
            (bool, RaycastHit) hit = ClickColli();
            if (hit.Item1)
            {
                On_DoubleClick.OnNext(hit.Item2);
            }
            else
            {
                On_ClickMargin.OnNext(Unit.Default);
            }
        };


        On_RightClick.Subscribe(hitInfo => 
        {
            switch(hitInfo.transform.tag)
            {
                case "部屋":
                    rightClickMenu.OnClick_Room(hitInfo);
                    break;
                case "家具":
                    rightClickMenu.OnClick_Funiture(hitInfo);
                    break;
                default:
                    rightClickMenu.OnClick_Margin();
                    break;
            }
        });
        On_LeftClick.Subscribe(hitInfo =>
        {
            switch (hitInfo.transform.tag)
            {
                case "":
                    break;
                default:
                    break;
            }
        });
        On_DoubleClick.Subscribe(hitInfo =>
        {
            switch (hitInfo.transform.tag)
            {
                case "部屋":
                    // 選択して編集可能状態に
                    break;
                case "アバター":
                    // 選択して編集可能状態に
                    break;
                case "家具":
                    // 選択して編集可能状態に
                    break;
                default:
                    break;
            }
        });
    }



    void AddColli(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Collider colli = child.GetComponent<Collider>();
            if (colli == null)
            {
                AddColli(child);
                continue;
            }
            rightClickTargets.Add(colli.gameObject);
        }
    }


    (bool, RaycastHit) ClickColli()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        float maxDistance = 10000f;

        bool isHit = Physics.Raycast(ray, out hitInfo, maxDistance);

        if (!isHit) return (false, hitInfo);
        
        DischargeMarker(hitInfo);
        return (true, hitInfo);
    }


    public GameObject DischargeMarker(RaycastHit hitInfo)
    {
        Pool_ClickMarker markerPool = GameObject.Find("Pool_UI").GetComponent<Pool_ClickMarker>();
        Quaternion normalRotation = Quaternion.LookRotation(hitInfo.normal);
        GameObject impactOverlay = markerPool.Object_Discharge(hitInfo.point + hitInfo.normal * 0.02f, normalRotation);
        markerPool.Object_Hide(impactOverlay);
        return impactOverlay;
    }
}
