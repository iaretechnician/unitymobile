using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

namespace CoreTrivia
{
    public class LanguageDialog : MonoBehaviour
    {
        public RectTransform LanguageSelectorPrefab;

        [Space(10)]

        public GameObject LanguagesList;
        public GameObject Loader;
        public GameObject ErrorPanel;

        [Space(10)]

        public GameObject CloseButton;

        public Button RetryButton;
        public Button SelectLanguageButton;

        internal int SelectedLanguage;

        private ScrollRect LanguagesScrollView;
        private List<LanguageFormat> Languages = new List<LanguageFormat>();

        private void Awake()
        {
            RetryButton.onClick.AddListener(GetLanguageList);
            SelectLanguageButton.onClick.AddListener(SaveLanguageSelection);

            LanguagesScrollView = LanguagesList.GetComponentInChildren<ScrollRect>();
        }

        private void OnEnable()
        {
            GetLanguageList();
        }

        private void GetLanguageList()
        {
            Languages.Clear();
            SelectedLanguage = PlayerPrefs.GetInt(Globals.instance.SelectedLanguagePrefName, 0);

            Loader.SetActive(true);
            LanguagesList.SetActive(false);
            ErrorPanel.SetActive(false);
            CloseButtonState(false);

            StartCoroutine(GetLanguagesRequest());
        }

        private IEnumerator GetLanguagesRequest()
        {
            string RequestUri = Globals.instance.LanguagesURL();

            using (UnityWebRequest webRequest = UnityWebRequest.Get(RequestUri))
            {
                webRequest.timeout = Globals.instance.DefaultTimeout;

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                CloseButtonState(true);
                Loader.SetActive(false);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    ErrorPanel.SetActive(true);

                    Debug.Log(webRequest.error);

                    yield break;
                }

                string responseData = $"{{ \"Languages\": {webRequest.downloadHandler.text} }}";

                Languages = JsonUtility.FromJson<LanguageList>(responseData).Languages;

                foreach (Transform child in LanguagesScrollView.content)
                {
                    Destroy(child.gameObject);
                }

                Languages.ForEach(Language =>
                {
                    RectTransform LanguageSelectorObj = Instantiate(LanguageSelectorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), LanguagesScrollView.content.transform);
                    LanguageSelectorObj.localScale = new Vector3(1, 1, 1);

                    LanguageSelector LanguageSelectorScript = LanguageSelectorObj.GetComponent<LanguageSelector>();

                    LanguageSelectorScript.LanguageName = Language.name;
                    LanguageSelectorScript.LanguageID = Language.id;
                    LanguageSelectorScript.LanguageSelectionDialog = this;

                    LanguageSelectorScript.NameTextField.text = Language.name;
                });

                LanguagesScrollView.content.transform.localPosition = new Vector3(0, 0, 0);
                LanguagesList.SetActive(true);
            }
        }

        private void CloseButtonState(bool show = true)
        {
            CloseButton.SetActive(show);
        }

        public void SelectLanguage(int id)
        {
            SelectedLanguage = id;
            EventManager.TriggerEvent("update-language-selection");
        }

        public void SaveLanguageSelection()
        {
            if (SelectedLanguage == 0)
                return;

            MainController.instance.SaveLanguageSelection(SelectedLanguage);
        }

        private void OnDestroy()
        {
            RetryButton.onClick.RemoveAllListeners();
            SelectLanguageButton.onClick.RemoveAllListeners();
        }
    }
}