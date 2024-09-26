using UnityEngine;
using UniRx;

public class Deleter : MonoBehaviourMyExtention
{
    public GameObject target;
    MyButton Controller => CheckAddComponent<MyButton>(gameObject);


    void Start()
    {
        Controller.On_Click.Subscribe(_ => 
        {
            Destroy(target);
        });
    }
}
