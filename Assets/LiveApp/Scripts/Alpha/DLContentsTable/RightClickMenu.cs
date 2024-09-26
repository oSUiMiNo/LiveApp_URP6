using UnityEngine;
using UniRx;
using Unity.VisualScripting;

public class RightClickMenu : MonoBehaviourMyExtention
{
    MyButton Controller => CheckAddComponent<MyButton>(gameObject);

    Transform funiture;
    Transform delete;


    void Start()
    {
        funiture = transform.Find("Funiture");
        delete = transform.Find ("Delete");
        gameObject.SetActive(false);
        //ClickHandler.On_RightClick.Subscribe( hitInfo =>
        //{
        //    if (hitInfo.collider.gameObject != gameObject) return;
        //    OnClick_Room(hitInfo);
        //});
        //ClickHandler.On_ClickMargin.Subscribe(_ => OnClick_Margin());
    }


    public async void OnClick_Room(RaycastHit hitInfo)
    {
        await Delay.Frame(1);
        DischargeTableCreator_Funiture dischargeTableCreator_Funiture = funiture.GetComponent<DischargeTableCreator_Funiture>();
        dischargeTableCreator_Funiture.hitInfo = hitInfo;
        funiture.SetParent(gameObject.transform.Find("Flexible Layout"));
        funiture.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }


    public async void OnClick_Funiture(RaycastHit hitInfo)
    {
        await Delay.Frame(1);
        Deleter deleter = delete.GetComponent<Deleter>();
        deleter.target = hitInfo.collider.gameObject;
        delete.SetParent(gameObject.transform.Find("Flexible Layout"));
        delete.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }


    public void OnClick_Margin()
    {
        if (gameObject.activeSelf == false) return;
        funiture.SetParent(transform);
        funiture.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
