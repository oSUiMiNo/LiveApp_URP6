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

        // ���ɓ����n�b�V���̃f�[�^�����݂������荞�ݒ��~
        NagesenContentsHandler.Data.Load();
        foreach (var contentsCatalog in NagesenContentsHandler.Data.ContentsCatalogs.Values)
        {
            if (contentsCatalog.Hash == ID)
            {
                Debug.LogAssertion($"���ɓ����J�^���O������܂�");
                return;
            }
        }


        Debug.Log($"�_�E�����[�h�J�n");
        // 1. Shiji�t�@�C�����擾���A�f�V���A���C�Y����
        shiji = await DeserializeShijiFile(ID);
        Debug.Log($"�_�E�����[�h����");


        Debug.Log(shiji.Hash.Content);
        Debug.Log(shiji.Addresses);

        //// 3. �R���e���c�𕪊�����
        //await new ContentsDivider(BufferDir, ContentsDir, ID).Execute();
    }

    // Shiji�t�@�C�����擾���f�V���A���C�Y���鏈��
    async UniTask<Shiji> DeserializeShijiFile(string searchWord)
    {
        Debug.Log($"�_�E�����[�h0");

        // �N�G����������shiji�t�@�C����URL���擾
        JToken queryResult = await CallNotionAPI_QueryFile(searchWord, QueryType.ID);

        if (queryResult == null || !queryResult.Any())
        {
            Debug.LogError("�w�肳�ꂽID�ɊY������G���g����������܂���ł����B");
            return null;
        }
        Debug.Log($"�_�E�����[�h1");

        // �t�@�C���̃v���p�e�B�ɃA�N�Z�X����.shiji�t�@�C�����擾
        var firstElement = queryResult[0];
        var properties = firstElement["properties"] as JObject;
        if (properties == null)
        {
            Debug.LogError("�v���p�e�B��������܂���B");
            return null;
        }

        Shiji shijiData = new Shiji();
        var shijiFile = properties["shiji�t�@�C��"];
        if (shijiFile != null && shijiFile["files"] is JArray shijiFiles && shijiFiles.Count > 0)
        {
            Debug.Log($"�_�E�����[�h2");

            string shijiUrl = shijiFiles[0]["file"]["url"].ToString();
            string shijiFileName = shijiFiles[0]["name"].ToString();
            string shijiFilePath = await DownloadFile(shijiUrl, shijiFileName);
            Debug.Log($"�_�E�����[�h5");

            // �擾����.shiji�t�@�C����ǂݎ��A�f�V���A���C�Y����
            string shijiJson = File.ReadAllText(shijiFilePath);
            shijiData = JsonConvert.DeserializeObject<Shiji>(shijiJson);
            Debug.Log($"�_�E�����[�h6");

            // �f�V���A���C�Y�����f�[�^���g�p���鏈����ǉ�
            if (shijiData != null)
            {
                Debug.Log("Shiji�t�@�C���̃f�V���A���C�Y�ɐ������܂����B");
                // �K�v�ɉ����ď�����ǉ�
                // ��: shijiData.Hash, shijiData.Catalog�ȂǂɃA�N�Z�X
            }
            else
            {
                Debug.LogError("Shiji�t�@�C���̃f�V���A���C�Y�Ɏ��s���܂����B");
            }
        }
        else
        {
            Debug.LogError("Shiji�t�@�C����������܂���B");
            return null;
        }
        Debug.Log($"�_�E�����[�h7");

        return shijiData;
    }

    // �t�@�C�����_�E�����[�h���鋤�ʊ֐�
    async UniTask<string> DownloadFile(string fileUrl, string fileName)
    {
        Directory.CreateDirectory(BufferDir);
        string newFilePath = @$"{BufferDir}\{fileName}";

        using (UnityWebRequest request = new UnityWebRequest(fileUrl))
        {
            Debug.Log($"�_�E�����[�h3 {newFilePath}");


            //request.downloadHandler = new DownloadHandlerBuffer();
            request.downloadHandler = new DownloadHandlerFile(newFilePath);
            request.chunkedTransfer = false;

            // ���N�G�X�g�̑��M
            var operation = request.SendWebRequest();

            // �_�E�����[�h�i�s�󋵂����O�ɏo��
            while (!operation.isDone)
            {
                Debug.Log($"�_�E�����[�h�i�s��: {request.downloadProgress * 100}%");
                await UniTask.Yield();  // �񓯊������𑱍s������
            }

            // �_�E�����[�h�����������珈���𑱍s
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                return null;
            }
            else
            {
                Debug.Log($"�_�E�����[�h4");

                byte[] results = request.downloadHandler.data;
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(results, 0, results.Length);
                    Debug.Log($"�_�E�����[�h4");
                }
                Debug.Log($"{fileName} �� {newFilePath} �ɕۑ�����܂����B");
                return newFilePath;
            }
        }
    }


    // CallNotionAPI_QueryFile�͊����̂܂܎g�p
    async UniTask<JToken> CallNotionAPI_QueryFile(string searchWord, QueryType queryType)
    {
        // �N�G�����\�z
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
                property = "�S�A�h���X",
                rich_text = new
                {
                    contains = searchWord
                }
            }
        };

        // �N�G���^�C�v�ɉ����ēK�؂ȃN�G����I��
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

            // �p�[�X
            JObject responseObj = JObject.Parse(jsonStr);

            // �e�[�u���̃t�B���^�����O���ꂽ�v�f
            JArray elements = (JArray)responseObj["results"];
            if (elements == null || !elements.Any())
            {
                Debug.LogError("���ʂ�������܂���ł����B");
                return null;
            }

            return elements;
        }
    }
}

// Shiji�t�@�C���̃f�V���A���C�Y�p�N���X
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
