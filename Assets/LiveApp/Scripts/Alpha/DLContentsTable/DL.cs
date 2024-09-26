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

public enum QueryType
{
    ID,
    Address
}

public class DL : MonoBehaviour
{
    private const string NotionAccessToken = "secret_OIxSWO69mxnD9FNbmL2US0pcsLWCUmsaglBZBCWPWrC";
    private const string DatabaseID = "ee761ae157e346b88ef3fd58ba9146d3";

    [SerializeField] public QueryType queryType = QueryType.ID;
    [SerializeField] public string ID;

    [SerializeField] string BufferDir;
    [SerializeField] string ContentsDir;
    public Shiji shiji;


    async void Start()
    {
        ContentsDir = NagesenContentsHandler.Data.SaveFolderPath;
        BufferDir = @$"{ContentsDir}/Buffer";

        // 既に同じハッシュのデータが存在したら取り込み中止
        NagesenContentsHandler.Data.Load();
        foreach (var contentsCatalog in NagesenContentsHandler.Data.ContentsCatalogs.Values)
        {
            if (contentsCatalog.Hash == ID)
            {
                Debug.LogAssertion($"既に同じカタログがあります");
                return;
            }
        }


        Debug.Log($"ダウンロード開始");
        // 1. Shijiファイルを取得し、デシリアライズする
        shiji = await DeserializeShijiFile(ID);
        Debug.Log($"ダウンロード完了");


        Debug.Log(shiji.Hash.Content);
        Debug.Log(shiji.Addresses);

        //// 3. コンテンツを分割処理
        //await new ContentsDivider(BufferDir, ContentsDir, ID).Execute();
    }

    // Shijiファイルを取得しデシリアライズする処理
    async UniTask<Shiji> DeserializeShijiFile(string searchWord)
    {
        Debug.Log($"ダウンロード0");

        // クエリ検索してshijiファイルのURLを取得
        JToken queryResult = await CallNotionAPI_QueryFile(searchWord, QueryType.ID);

        if (queryResult == null || !queryResult.Any())
        {
            Debug.LogError("指定されたIDに該当するエントリが見つかりませんでした。");
            return null;
        }
        Debug.Log($"ダウンロード1");

        // ファイルのプロパティにアクセスして.shijiファイルを取得
        var firstElement = queryResult[0];
        var properties = firstElement["properties"] as JObject;
        if (properties == null)
        {
            Debug.LogError("プロパティが見つかりません。");
            return null;
        }

        Shiji shijiData = new Shiji();
        var shijiFile = properties["shijiファイル"];
        if (shijiFile != null && shijiFile["files"] is JArray shijiFiles && shijiFiles.Count > 0)
        {
            Debug.Log($"ダウンロード2");

            string shijiUrl = shijiFiles[0]["file"]["url"].ToString();
            string shijiFileName = shijiFiles[0]["name"].ToString();
            string shijiFilePath = await DownloadFile(shijiUrl, shijiFileName);
            Debug.Log($"ダウンロード5");

            // 取得した.shijiファイルを読み取り、デシリアライズする
            string shijiJson = File.ReadAllText(shijiFilePath);
            shijiData = JsonConvert.DeserializeObject<Shiji>(shijiJson);
            Debug.Log($"ダウンロード6");

            // デシリアライズしたデータを使用する処理を追加
            if (shijiData != null)
            {
                Debug.Log("Shijiファイルのデシリアライズに成功しました。");
                // 必要に応じて処理を追加
                // 例: shijiData.Hash, shijiData.Catalogなどにアクセス
            }
            else
            {
                Debug.LogError("Shijiファイルのデシリアライズに失敗しました。");
            }
        }
        else
        {
            Debug.LogError("Shijiファイルが見つかりません。");
            return null;
        }
        Debug.Log($"ダウンロード7");

        return shijiData;
    }

    // ファイルをダウンロードする共通関数
    async UniTask<string> DownloadFile(string fileUrl, string fileName)
    {
        Directory.CreateDirectory(BufferDir);
        string newFilePath = @$"{BufferDir}\{fileName}";

        using (UnityWebRequest request = new UnityWebRequest(fileUrl))
        {
            Debug.Log($"ダウンロード3 {newFilePath}");


            //request.downloadHandler = new DownloadHandlerBuffer();
            request.downloadHandler = new DownloadHandlerFile(newFilePath);
            request.chunkedTransfer = false;

            // リクエストの送信
            var operation = request.SendWebRequest();

            // ダウンロード進行状況をログに出力
            while (!operation.isDone)
            {
                Debug.Log($"ダウンロード進行中: {request.downloadProgress * 100}%");
                await UniTask.Yield();  // 非同期処理を続行させる
            }

            // ダウンロードが完了したら処理を続行
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                return null;
            }
            else
            {
                Debug.Log($"ダウンロード4");

                byte[] results = request.downloadHandler.data;
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(results, 0, results.Length);
                    Debug.Log($"ダウンロード4");
                }
                Debug.Log($"{fileName} が {newFilePath} に保存されました。");
                return newFilePath;
            }
        }
    }


    // CallNotionAPI_QueryFileは既存のまま使用
    async UniTask<JToken> CallNotionAPI_QueryFile(string searchWord, QueryType queryType)
    {
        // クエリを構築
        var queryPayload_ID = new
        {
            filter = new
            {
                property = "ID",
                title = new
                {
                    contains = searchWord
                }
            }
        };

        var queryPayload_Address = new
        {
            filter = new
            {
                property = "全アドレス",
                rich_text = new
                {
                    contains = searchWord
                }
            }
        };

        // クエリタイプに応じて適切なクエリを選択
        string jsonQuery = queryType == QueryType.ID
            ? JsonConvert.SerializeObject(queryPayload_ID)
            : JsonConvert.SerializeObject(queryPayload_Address);

        if (string.IsNullOrEmpty(jsonQuery)) return null;

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonQuery);

        using (UnityWebRequest request = new UnityWebRequest($"https://api.notion.com/v1/databases/{DatabaseID}/query", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {NotionAccessToken}");
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            request.SetRequestHeader("Notion-Version", "2022-02-22");

            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                return null;
            }

            string jsonStr = request.downloadHandler.text;

            // パース
            JObject responseObj = JObject.Parse(jsonStr);

            // テーブルのフィルタリングされた要素
            JArray elements = (JArray)responseObj["results"];
            if (elements == null || !elements.Any())
            {
                Debug.LogError("結果が見つかりませんでした。");
                return null;
            }

            return elements;
        }
    }
}

// Shijiファイルのデシリアライズ用クラス
public class Shiji
{
    public Name_Content Hash { get; set; }
    public Name_Content Catalog { get; set; }
    public string Addresses { get; set; }
    public List<Name_Content> Bundles { get; set; }
}

public class Name_Content
{
    public string FileName { get; set; }
    public string Content { get; set; }
}
