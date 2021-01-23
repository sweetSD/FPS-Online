using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NetworkImage : MonoBehaviour
{
    [SerializeField] private string _url;
    public string Url
    {
        get => _url;
        set
        {
            _url = value;
            StartCoroutine(CO_LoadImageFromUrl(_url));
        }
    }

    [SerializeField] private Image _image;
    [SerializeField] private bool _useCache = true;

    private void Start()
    {
        if (_image == null) _image = GetComponent<Image>();
        if (_url.IsNotEmpty()) Url = _url;
    }

    private IEnumerator CO_LoadImageFromUrl(string url)
    {
        string directoryPath = Application.persistentDataPath + "/Cache/";
        string path = directoryPath + url.GetHashCode() + ".png";
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        if (File.Exists(path) && _useCache)
        {
            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            texture.Apply();
            _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.one * 0.5f);
        }
        else
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.LogError(uwr.error);
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.one * 0.5f);

                    if(_useCache)
                    {
                        var bytes = texture.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);
                    }
                }

            }
        }
    }
}
