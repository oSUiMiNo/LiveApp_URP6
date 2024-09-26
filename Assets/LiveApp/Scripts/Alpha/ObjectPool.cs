using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class ObjectPool : MonoBehaviour
{
    [SerializeField] protected List<GameObject> pool = new List<GameObject>();
    [SerializeField] protected GameObject Object;
    [SerializeField] protected int Quantity;
    [SerializeField] protected float HideTime;

    private enum State
    {
        Wait,
        Ready
    }
    private State state;
    public void Start()
    {
        Define_Object();
        Define_Quantity();
        Define_HideTime();
        Pool_Initialize();
    }

    protected abstract void Define_Object();

    protected abstract void Define_Quantity();

    protected abstract void Define_HideTime();

    public virtual void Pool_Initialize()
    {
        for (int i = 0; i < Quantity; i++)
        {
            GameObject Object_Initial = Object_Create();
            pool.Add(Object_Initial);
        }
    }


    public virtual GameObject Object_Discharge(Vector3 Position, Quaternion Rotation)
    {
        foreach (var Object_Existing in pool)  //戻り値はリターン文なので、返せるオブジェクトを1つ見つけて返した時点でループを抜ける。
        {
            if (Object_Existing.activeSelf == false)
            {
                Object_Existing.SetActive(true);
                Object_Existing.transform.position = Position;
                Object_Existing.transform.rotation = Rotation;

                return Object_Existing;
            }
        }

        GameObject Object_Additional = Object_Create();
        Object_Additional.SetActive(true);
        pool.Add(Object_Additional);
        Object_Additional.transform.position = Position;
        Object_Additional.transform.rotation = Rotation;

        return Object_Additional;
    }



    public virtual GameObject Object_Create()
    {
        GameObject Object_New = Instantiate(Object, this.transform);
        Object_New.name = Object.name + (pool.Count + 1);
        Object_New.SetActive(false);
        Object_New.transform.position = new Vector3(10, 30, 10);

        return Object_New;
    }



    public void Object_Hide(GameObject Object)
    {
        StartCoroutine(Hide(Object));
    }



    public void Object_HideAll()
    {
        foreach (var Object_Existing in pool)
        {
            //Object_Existing.SetActive(false);
            StartCoroutine(Hide(Object_Existing));
        }
    }


    private IEnumerator Hide(GameObject Object_ToHide)
    {
        yield return new WaitForSeconds(HideTime);
        Object_ToHide.SetActive(false);
    }
}