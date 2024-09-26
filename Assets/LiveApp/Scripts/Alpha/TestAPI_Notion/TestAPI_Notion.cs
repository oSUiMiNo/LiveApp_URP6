using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

// このNotionページにDB置いてある。
//https://www.notion.so/6795dd70753a495897cb85e0abce95fe

public class TestAPI_Notion : MonoBehaviour
{
    private const string NotionAccessToken = "secret_OIxSWO69mxnD9FNbmL2US0pcsLWCUmsaglBZBCWPWrC"; //新しい方

    void Start()
    {
        //InputEventHandler.OnDown_E += UseAPI_Add_Test;
        //UseAPI_Add_Test();
    }




    // パースエラー出て使えん
    async void UseAPI_Add_Test()
    {
        string DatabaseID = "cf2343f8bb934f51b3cddd99640fc790";

        EditableJSON eJson_Payload = new EditableJSON($@"{Application.dataPath}\Payload.json");
        eJson_Payload.Obj0["parent"]["database_id"] = DatabaseID;

        string payload = eJson_Payload.Json;
        //string payload = JsonConvert.SerializeObject(eJson_Payload.Obj, Formatting.Indented);


        DebugView.Log($"{payload}");


        UnityWebRequest request = UnityWebRequest.PostWwwForm($"https://api.notion.com/v1/pages", payload);
        request.SetRequestHeader("Authorization", $"Bearer {NotionAccessToken}");
        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Notion-Version", "2022-02-22");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();


        await request.SendWebRequest();



        switch (request.result)
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log("リクエスト中");
                break;

            case UnityWebRequest.Result.Success:
                Debug.Log("リクエスト成功");
                break;

            case UnityWebRequest.Result.ConnectionError:
                Debug.Log(
                    @"サーバとの通信に失敗。
                        リクエストが接続できなかった、
                        セキュリティで保護されたチャネルを確立できなかったなど。");
                Debug.LogError(request.error);
                break;

            case UnityWebRequest.Result.ProtocolError:
                Debug.Log(
                    @"サーバがエラー応答を返した。
                        サーバとの通信には成功したが、
                        接続プロトコルで定義されているエラーを受け取った。");
                Debug.LogError(request.error);
                break;

            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log(
                    @"データの処理中にエラーが発生。
                        リクエストはサーバとの通信に成功したが、
                        受信したデータの処理中にエラーが発生。
                        データが破損しているか、正しい形式ではないなど。");
                Debug.LogError(request.error);
                break;

            default: throw new ArgumentOutOfRangeException();
        }
    }
}