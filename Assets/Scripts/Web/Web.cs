using GLG;
using System.Collections;
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

        public void Send(string uri, string token, RequestKind requstKind, (string key, string value)[] data = null)
        {
            WWWForm form = new WWWForm();
            form.AddField("token", token);
            if (data != null)
            {
                foreach (var item in data)
                {
                    form.AddField(item.key, item.value);
                }
            }

            _routine = SendRequest(uri, requstKind, form);
            Kernel.CoroutinesObject.StartCoroutine(_routine);
        }

        public void Cancel()
        {
            if (_routine != null)
            {
                Kernel.CoroutinesObject.StopCoroutine(_routine);
            }
        }

        private IEnumerator SendRequest(string uri, RequestKind requestKind, WWWForm form)
        {
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
                    using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
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
                            ResultTexture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                        }
                    }
                    break;
                default:
                    break;
            }
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
