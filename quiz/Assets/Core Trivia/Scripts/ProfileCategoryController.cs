using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace CoreTrivia
{
    public class ProfileCategoryController : MonoBehaviour
    {
        public Text CategoryName;
        public Text ScoreText;

        [Space(5)]

        public GameObject Loader;
        public GameObject RetryButton;

        internal int CategoryId;

        private RawImage thisImage;

        private void Awake()
        {
            thisImage = GetComponent<RawImage>();
        }

        private void Start()
        {
            LoadIcon();
        }
        private void OnEnable()
        {
            ScoreText.text = PlayerPrefs.GetInt(CategoryId.ToString(), 0).ToString();
        }

        public void LoadIcon()
        {
            thisImage.enabled = false;
            Loader.SetActive(false);
            RetryButton.SetActive(false);

            string CacheFilePath = Path.Combine(MainController.instance.CategoriesCacheFolder, CategoryId.ToString()).Replace(Path.DirectorySeparatorChar, '/');

            if (File.Exists(CacheFilePath))
            {
                StartCoroutine(LoadCachedIcon(CacheFilePath));
            }
            else
            {
                StartCoroutine(DownloadIcon(CacheFilePath));
            }
        }

        private IEnumerator LoadCachedIcon(string path)
        {
            if (!File.Exists(path))
                yield break;

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
                thisImage.texture = imgTex;
                thisImage.enabled = true;
            }
        }

        private IEnumerator DownloadIcon(string path)
        {
            Loader.SetActive(true);

            string RequestUri = Globals.instance.IconURL(CategoryId);

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
    }
}
