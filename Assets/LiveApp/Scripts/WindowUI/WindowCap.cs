using UnityEngine;
using Cysharp.Threading.Tasks;

public class WindowCap : MonoBehaviour
{
    bool avtive = false;

    async void Start()
    {
        await Delay.Second(2);

        SetLayerRecursively(transform.Find("Window").gameObject, "Default");
        SetLayerRecursively(transform.Find("Window/Canvas").gameObject, "ProhibitWebCam");
        SetLayerRecursively(GameObject.Find("Cam_UI").transform.Find("AltTabWIndows").gameObject, "UI");
    }

 
    /// <summary>
    /// �������g���܂ނ��ׂĂ̎q�I�u�W�F�N�g�̃��C���[��ݒ肵�܂�
    /// </summary>
    public static void SetLayerRecursively(GameObject self, string layerName )
    {
        self.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform n in self.transform)
        {
            SetLayerRecursively(n.gameObject, layerName);
        }
    }
}
