using System;
using System.IO;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace CoreTrivia
{
    public class CategoryController : MonoBehaviour
    {
        public Text CategoryName;
        public GameObject Loader;
        public GameObject RetryButton;

        internal CategoryFormat CategoryDetails;

        private Button thisButton;
        private RawImage ButtonRawImage;

        private void Awake()
        {
            thisButton = GetComponent<Button>();
            ButtonRawImage = thisButton.GetComponent<RawImage>();

            thisButton.onClick.AddListener(LoadCategory);
        }

        private void Start()
        {
            LoadIcon();
        }

        public void LoadIcon()
        {
            ButtonRawImage.enabled = false;
            Loader.SetActive(false);
            RetryButton.SetActive(false);

            string CacheFilePath = Path.Combine(MainController.instance.CategoriesCacheFolder, CategoryDetails.id.ToString()).Replace(Path.DirectorySeparatorChar, '/');

            if (File.Exists(CacheFilePath) && IsFileFresh(CacheFilePath))
            {
                StartCoroutine(LoadCachedIcon(CacheFilePath));
            }
            else
            {
                StartCoroutine(DownloadIcon(CacheFilePath));
            }
        }

        private DateTime FormatTimestamp()
        {
            string FormattedTimestamp = CategoryDetails.modified.Replace("T", " ").Substring(0, CategoryDetails.modified.IndexOf("."));
            return DateTime.ParseExact(FormattedTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private bool IsFileFresh(string path)
        {
            if (!File.Exists(path))
                return false;

            DateTime LastWriteTime = File.GetLastWriteTimeUtc(path);
            return DateTime.Compare(LastWriteTime, FormatTimestamp()) > 0 ? true : false;
        }

        private IEnumerator LoadCachedIcon(string path)
        {
            using (UnityWebRequest CachedIconRequest = UnityWebRequestTexture.GetTexture("file:///" + path))
            {
                //Wait for the image to load
                yield return CachedIconRequest.SendWebRequest();

                if (CachedIconRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Image Request error - " + CachedIconRequest.error);
                    yield break;
                }

                Texture2D imgTex = DownloadHandlerTexture.GetContent(CachedIconRequest);

                //Load the texture to the image display
                ButtonRawImage.texture = imgTex;
                ButtonRawImage.enabled = true;
            }
        }

        private IEnumerator DownloadIcon(string path)
        {
            Loader.SetActive(true);

            string RequestUri = Globals.instance.IconURL(CategoryDetails.id);

            using (UnityWebRequest DownloadIconRequest = UnityWebRequestTexture.GetTexture(RequestUri))
            {
                DownloadIconRequest.timeout = Globals.instance.DefaultTimeout;

                yield return DownloadIconRequest.SendWebRequest();

                Loader.SetActive(false);

                if (DownloadIconRequest.result != UnityWebRequest.Result.Success)
                {
                    RetryButton.SetActive(true);

                    Debug.Log(DownloadIconRequest.error);

                    yield break;
                }

                Texture2D DownloadedTexture = DownloadHandlerTexture.GetContent(DownloadIconRequest);

                if (DownloadedTexture.width != 8 && DownloadedTexture.height != 8)
                {
                    File.WriteAllBytes(path, DownloadIconRequest.downloadHandler.data);

                    //Load the texture to the image display
                    StartCoroutine(LoadCachedIcon(path));

                    Destroy(DownloadedTexture);
                }
            }
        }

        private void LoadCategory()
        {
            CategoryFormat thisCategory = CategoryDetails;
            thisCategory.image = ButtonRawImage.texture;

            MainController.instance.LoadCategory(CategoryDetails);
        }

        private void OnDestroy()
        {
            thisButton.onClick.RemoveAllListeners();
        }
    }
}
