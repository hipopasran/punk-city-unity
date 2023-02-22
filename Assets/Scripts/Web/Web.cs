using GLG;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class Web
{
    public enum RequestKind { Get, Post, Delete, GetTexture }
    public class Request
    {
        private IEnumerator _routine;

        public bool IsComplete { get; private set; }
        public bool IsError { get; private set; }
        public string Result { get; private set; }
        public Texture2D ResultTexture { get; private set; }

        public void Send(string uri, string token, RequestKind requestKind, (string key, string value)[] queryParams = null, (string key, string value)[] data = null)
        {
            Debug.Log($"[Request] Try to send request. URI={uri} | token={token} | requestKind={requestKind}");
            Debug.Log("[Request] Query params:");
            string query = "?";
            List<(string key, string value)> _query = new List<(string key, string value)>();
            _query.Add(("token", token));
            if (queryParams != null && queryParams.Length > 0)
            {
                _query.AddRange(queryParams);
            }
            if (_query != null)
            {
                foreach (var item in _query)
                {
                    Debug.Log($"{item.key}={item.value}");
                    if (query.Length > 1)
                    {
                        query += '&';
                    }
                    query += $"{item.key}={item.value}";
                }
            }

            Debug.Log("[Request] Data:");
            WWWForm form = new WWWForm();
            if (data != null)
            {
                foreach (var item in data)
                {
                    form.AddField(item.key, item.value);
                    Debug.Log($"{item.key}={item.value}");
                }
            }

            _routine = SendRequest(uri, requestKind, query, form);
            Kernel.CoroutinesObject.StartCoroutine(_routine);
        }

        public void Cancel()
        {
            if (_routine != null)
            {
                Kernel.CoroutinesObject.StopCoroutine(_routine);
            }
        }

        private IEnumerator SendRequest(string uri, RequestKind requestKind, string query, WWWForm data)
        {
            uri += query;
            Debug.Log("[SendRequest] URI: " + uri);
            switch (requestKind)
            {
                case RequestKind.Get:
                    using (UnityWebRequest www = UnityWebRequest.Get(uri))
                    {
                        www.downloadHandler = new DownloadHandlerBuffer();
                        yield return www.SendWebRequest();
                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            IsError = true;
                            Result = www.error;
                        }
                        else
                        {
                            IsError = false;
                            Result = Encoding.Default.GetString(www.downloadHandler.data);
                        }
                    }
                    break;
                case RequestKind.Post:
                    using (UnityWebRequest www = UnityWebRequest.Post(uri, data))
                    {
                        www.downloadHandler = new DownloadHandlerBuffer();
                        yield return www.SendWebRequest();
                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            IsError = true;
                            Result = www.error;
                        }
                        else
                        {
                            IsError = false;
                            Result = Encoding.Default.GetString(www.downloadHandler.data);
                        }
                    }
                    break;
                case RequestKind.Delete:
                    using (UnityWebRequest www = UnityWebRequest.Delete(uri))
                    {
                        www.downloadHandler = new DownloadHandlerBuffer();
                        yield return www.SendWebRequest();
                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            IsError = true;
                            Result = www.error;
                        }
                        else
                        {
                            IsError = false;
                            Result = Encoding.Default.GetString(www.downloadHandler.data);
                        }
                    }
                    break;
                case RequestKind.GetTexture:
                    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
                    {
                        yield return www.SendWebRequest();
                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            IsError = true;
                            Result = www.error;
                        }
                        else
                        {
                            IsError = false;
                            ResultTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        }
                    }
                    break;
                default:
                    break;
            }
            Debug.Log("[Request] Result: " + Result);
            Debug.Log("------------------------------------------------------------------");
            IsComplete = true;
            yield break;
        }
    }


    private static string _url;
    private static string _token;

    public static void Initialize(string url, string token)
    {
        _url = url;
        _token = token;
    }
    public static void Initialize(string url)
    {
        _url = url;
    }

    public static Request SendRequest(string urn, RequestKind requstKind, (string key, string value)[] data = null)
    {
        Request result = new Request();
        string uri = _url;
        if (!urn.StartsWith('/'))
        {
            uri += '/';
        }
        uri += urn;
        result.Send(uri, _token, requstKind, data);
        return result;
    }

    public static string GetToken()
    {
        return "DlbxqqScj-z5sYLlFwLIJg";
    }

    public static T Parse<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
    public static Sprite ToSprite(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
    }
}
