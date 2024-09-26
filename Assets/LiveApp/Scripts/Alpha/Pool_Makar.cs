using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_ClickMarker : ObjectPool
{
    protected override void Define_Object()
    {
        Object = transform.Find("ClickMarker").gameObject;
    }
    protected override void Define_Quantity()
    {
        Quantity = 3;
    }
    protected override void Define_HideTime()
    {
        HideTime = 6f;
    }


    //public GameObject Create_ImpactPointOverlay(RaycastHit hitInfo)
    //{
    //    Pool_ClickMarker pool_ImpactOverlay = GetComponent<Pool_ClickMarker>();
    //    Quaternion normalRotation = Quaternion.LookRotation(hitInfo.normal);
    //    GameObject impactOverlay = pool_ImpactOverlay.Object_Discharge(hitInfo.point + hitInfo.normal * 0.02f, normalRotation);
    //    pool_ImpactOverlay.Object_Hide(impactOverlay);
    //    return impactOverlay;
    //}



    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hitInfo = new RaycastHit();
    //        float maxDistance = 10000f;

    //        bool isHit = Physics.Raycast(ray, out hitInfo, maxDistance);

    //        if (isHit)
    //        {
    //            Create_ImpactPointOverlay(hitInfo);
    //        }
    //    }
    //}
}