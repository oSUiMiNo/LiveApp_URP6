using UnityEngine;
using System;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.AddressableAssets.ResourceLocators;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json.Linq;

public class ServerController : MonoBehaviour
{
    private HttpListener listener;
    private Thread listenerThread;
    private UnityMainThreadDispatcher dispatcher;
    private const int HTTP_PORT = 8081;
    private volatile bool isRunning = true;
    private List<Thread> activeThreads = new List<Thread>();

    void Start()
    {
        UnityEngine.Debug.Log("HttpServer started");
        dispatcher = UnityMainThreadDispatcher.Instance();

        StartHttpServer();
    }

    void StartHttpServer()
    {
        UnityEngine.Debug.Log("Starting HTTP server...");
        if (IsPortInUse(HTTP_PORT))
        {
            UnityEngine.Debug.LogError($"HTTP port {HTTP_PORT} is already in use. Please close the application using this port and try again.");
            return;
        }

        listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{HTTP_PORT}/");
        
        try
        {
            UnityEngine.Debug.Log("Starting listener...");
            listener.Start();
            UnityEngine.Debug.Log($"HTTP Server is running on http://{GetLocalIP()}:{HTTP_PORT}");

            listenerThread = new Thread(ListenForRequests);
            listenerThread.Start(listener);
            activeThreads.Add(listenerThread);
            UnityEngine.Debug.Log("Listener thread started.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Unexpected error starting HTTP server: {e.Message}");
            UnityEngine.Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }

    void ListenForRequests(object listenerObj)
    {
        HttpListener currentListener = (HttpListener)listenerObj;
        UnityEngine.Debug.Log("ListenForRequests started.");
        while (isRunning && currentListener.IsListening)
        {
            try
            {
                UnityEngine.Debug.Log("Waiting for a connection...");
                HttpListenerContext context = currentListener.GetContext();
                UnityEngine.Debug.Log($"Received request for {context.Request.Url.PathAndQuery}");

                ProcessRequest(context);
            }
            catch (HttpListenerException e)
            {
                UnityEngine.Debug.LogError($"HttpListenerException: {e.Message}");
                UnityEngine.Debug.LogError($"Stack trace: {e.StackTrace}");
                break;
            }
            catch (ObjectDisposedException e)
            {
                UnityEngine.Debug.LogError($"ObjectDisposedException: {e.Message}");
                UnityEngine.Debug.LogError($"Stack trace: {e.StackTrace}");
                break;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Unexpected error in ListenForRequests: {e.Message}");
                UnityEngine.Debug.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        UnityEngine.Debug.Log("ListenForRequests ended.");
    }

    void ProcessRequest(HttpListenerContext context)
    {
        UnityEngine.Debug.Log("--- Begin Request Processing ---");
        UnityEngine.Debug.Log($"URL: {context.Request.Url.PathAndQuery}");
        UnityEngine.Debug.Log($"Method: {context.Request.HttpMethod}");
        UnityEngine.Debug.Log($"Content Type: {context.Request.ContentType}");
        UnityEngine.Debug.Log($"Content Length: {context.Request.ContentLength64}");

        foreach (string header in context.Request.Headers.AllKeys)
        {
            UnityEngine.Debug.Log($"Header: {header}: {context.Request.Headers[header]}");
        }

        string requestBody = "";
        try
        {
            UnityEngine.Debug.Log("Attempting to read request body");
            if (context.Request.HasEntityBody)
            {
                UnityEngine.Debug.Log("Request has entity body");
                using (var body = context.Request.InputStream)
                {
                    UnityEngine.Debug.Log("Opened InputStream");
                    int contentLength = (int)context.Request.ContentLength64;
                    byte[] buffer = new byte[contentLength];
                    int bytesRead = 0;
                    int totalBytesRead = 0;

                    while (totalBytesRead < contentLength)
                    {
                        bytesRead = body.Read(buffer, totalBytesRead, contentLength - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        totalBytesRead += bytesRead;
                        UnityEngine.Debug.Log($"Read {bytesRead} bytes. Total: {totalBytesRead}/{contentLength}");
                    }

                    requestBody = System.Text.Encoding.UTF8.GetString(buffer);
                }
                UnityEngine.Debug.Log($"Request Body: {requestBody}");
                UnityEngine.Debug.Log($"Read body length: {requestBody.Length}");

                if (!string.IsNullOrEmpty(requestBody))
                {
                    dispatcher.Enqueue(() => ScaleGameObject(requestBody));
                    UnityEngine.Debug.Log("Enqueued ScaleGameObject");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Request body is empty");
                }
            }
            else
            {
                UnityEngine.Debug.Log("Request has no entity body");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Error reading request: {e.Message}");
            UnityEngine.Debug.LogError($"Stack trace: {e.StackTrace}");
        }

        // Send a response
        string responseString = "{\"message\": \"Request processed\"}";
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = "application/json";
        context.Response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
        context.Response.Close();

        UnityEngine.Debug.Log($"Sent response: {responseString}");
        UnityEngine.Debug.Log("--- End Request Processing ---");
    }

    //[Serializable]
    //private class ScaleData
    //{
    //    public string scale;
    //}

    [Serializable]
    public class PurchaseData
    {
        public string itemId;
        public int quantity;
        public string timestamp;
    }

    private void ScaleGameObject(string jsonData)
    {
        UnityEngine.Debug.Log($"ScaleGameObject called with data: {jsonData}");
        PurchaseData data = JsonUtility.FromJson<PurchaseData>(jsonData);
        //JObject jObj = JObject.Parse(jsonData);
        //PurchaseData data = new PurchaseData()
        //{
        //    itemId = (string)jObj["itemId"],
        //    quantity = (int)jObj["quantity"],
        //    timestamp = (string)jObj["timestamp"]
        //};
        Debug.Log($"{data.itemId}");
        Discharge(data);
        //try
        //{
        //    Debug.Log($"1");
        //    PurchaseData data = JsonUtility.FromJson<PurchaseData>(jsonData);
        //    if (data == null)
        //    {
        //        Debug.Log($"2");
        //        UnityEngine.Debug.LogError($"Failed to parse JSON data: {jsonData}");
        //        return;
        //    }
        //    else 
        //    {
        //        Debug.Log($"3");
        //        Discharge(data);
        //    }
        //    Debug.Log($"4");
        //    Debug.Log($"Parsed JSON data: {JsonUtility.ToJson(data)}");
        //    //float scaleValue;
        //    //if (float.TryParse(data.scale, out scaleValue))
        //    //{
        //    //    Vector3 newScale = Vector3.one * scaleValue;
        //    //    transform.localScale = newScale;
        //    //    UnityEngine.Debug.Log($"GameObject scaled to {newScale}");
        //    //}
        //    //else
        //    //{
        //    //    UnityEngine.Debug.LogError($"Invalid scale value: {data.scale}");
        //    //}
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError($"Error parsing scale data: {e.Message}");
        //    Debug.LogError($"Stack trace: {e.StackTrace}");
        //}
    }

    async void Discharge(PurchaseData data)
    {
        if(data.itemId.Contains("Fireworks"))
        {
            for (int i = 0; i < data.quantity; i++)
            {
                GameObject.Find("CS").GetComponent<RuntimeInputCV>().NagesenHanabi();
            }
        }
        if (data.itemId.Contains("Clothes0"))
        {
            GameObject.Find("Avatar").GetComponent<AvatarRV>().Direction_SW(0);
        }
        if (data.itemId.Contains("Clothes1"))
        {
            GameObject.Find("Avatar").GetComponent<AvatarRV>().Direction_SW(1);
        }
        else
        for (int i = 0; i < data.quantity; i++)
        {
            Addressables.InstantiateAsync(data.itemId);
        }
    }


    void OnDisable()
    {
        StopServer();
    }

    void StopServer()
    {
        isRunning = false;
        if (listener != null)
        {
            listener.Stop();
            listener.Close();
        }

        foreach (Thread thread in activeThreads)
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Join(5000); // Wait up to 5 seconds for each thread to finish
            }
        }
        activeThreads.Clear();
    }

    private bool IsPortInUse(int port)
    {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

        return ipEndPoints.Any(endPoint => endPoint.Port == port);
    }

    static string GetLocalIP()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

        foreach (var network in networkInterfaces)
        {
            var properties = network.GetIPProperties();
            var addresses = properties.UnicastAddresses
                .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            foreach (var address in addresses)
            {
                return address.Address.ToString();
            }
        }

        return "127.0.0.1";
    }
}