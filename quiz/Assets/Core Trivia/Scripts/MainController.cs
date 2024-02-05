#if UNITY_EDITOR
using UnityEditor;
#endif

using DG.Tweening;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace CoreTrivia
{
    public class MainController : MonoBehaviour
    {
        internal static MainController instance;

        [Header("Pages")]

        public GameObject Homepage;
        public GameObject CategoriesPage;
        public GameObject GamePage;
        public GameObject PausePage;
        public GameObject GameOverPage;

        [Header("Homepage")]

        public GameObject LanguageDialog;

        [Space(10)]

        public GameObject SettingsPage;
        public GameObject ProfilePage;

        [Space(5)]

        public Text ToolbarTitle;

        [Space(5)]

        public GameObject ProfileButton;
        public GameObject BackButton;
        public GameObject SettingsButton;

        [Header("Profile Page")]

        public RectTransform ProfileCategoryPrefab;

        [Space(10)]

        public ScrollRect ProfileCategoriesList;

        [Header("Settings Page")]

        public Button SoundButton;
        public Button VibrationButton;
        public Button CacheButton;

        private Text CacheButtonText;

        private bool VibrationEnabled;

        [Header("Categories Page")]

        public RectTransform CategoryButtonPrefab;

        [Space(10)]

        public ScrollRect CategoriesList;
        public GameObject CategoryLoader;
        public GameObject ErrorPanel;

        [Space(5)]

        public Text CategoryLoaderText;
        public Button ErrorPanelButton;
        public Text ErrorPanelText;

        private List<CategoryFormat> Categories = new List<CategoryFormat>();

        private Coroutine GetCategoriesCoroutine;

        [Header("Game Page")]

        public GameObject LivesUI;
        public Text LivesText;

        [Space(10)]

        public ScrollRect QuestionScrollView;
        public RawImage QuestionImage;
        public GameObject ImageLoader;
        public GameObject ImageErrorPanel;

        [Space(5)]

        public CanvasGroup AnswerGroup;
        public GameObject TofAnswersUI;
        public GameObject MultiAnswersUI;

        [Space(5)]

        public GameObject PostQuestionButtons;
        public GameObject ExplanationButton;

        [Space(5)]

        public Text TofAnswerHeader;
        public GameObject TrueButton;
        public GameObject FalseButton;

        [Space(5)]

        public GameObject ImageLightbox;
        public RawImage LightboxImageHolder;

        [Space(10)]

        public GameObject ExplanantionPanel;
        public ScrollRect ExplanantionScrollView;

        private Text QuestionText, ExplanationText;

        private AspectRatioFitter ImageAspectRatioFitter;
        private AspectRatioFitter LightboxImageAspectRatioFitter;

        private List<Button> MultiAnswerButtons = new List<Button>();
        private List<Text> MultiAnswerButtonText = new List<Text>();

        private int LivesLeft, CurrentScore;

        private CategoryFormat CurrentCategory;

        private QuestionFormat CurrentQuestion;

        private List<QuestionFormat> Questions = new List<QuestionFormat>();

        [Header("GameOver Page")]

        public RawImage CategoryImage;
        public Text CategoryName;

        [Space(5)]

        public Text ScoreText;

        // Other Variables

        internal string CategoriesCacheFolder;
        internal string ImagesCacheFolder;

#if UNITY_EDITOR
        [ContextMenu("Open Cache Location", false)]
        public void OpenCacheLocation()
        {
            Application.OpenURL(Application.temporaryCachePath);
        }

        [ContextMenu("Delete ALL PlayerPrefs", false)]
        public void ClearAllPrefs()
        {
            bool result = EditorUtility.DisplayDialog("Delete ALL PlayerPrefs", "Do you want to DELETE ALL PlayerPrefs? You cannot undo this.", "Delete", "Cancel");

            if (result)
                PlayerPrefs.DeleteAll();
        }
#endif

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }

            SetupConstants();

            CreateCacheFolders();

            ToggleLanguageDialog(false);
        }

        private void Start()
        {
            OpenHomepage();

            ToggleSound(true);

            ToggleVibration(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (LanguageDialog.activeInHierarchy)
                {
                    ToggleLanguageDialog(false);
                    return;
                }

                if (ProfilePage.activeInHierarchy || SettingsPage.activeInHierarchy || CategoriesPage.activeInHierarchy)
                {
                    OpenHomepage();
                    return;
                }

                if (ImageLightbox.activeInHierarchy)
                {
                    ToggleLightbox(false);
                    return;
                }

                if (PausePage.activeInHierarchy)
                {
                    ResumeGame();
                    return;
                }

                if (Homepage.activeInHierarchy)
                {
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                }
            }
        }

        private void SetupConstants()
        {
            CacheButtonText = CacheButton.GetComponentInChildren<Text>();

            QuestionText = QuestionScrollView.content.GetComponent<Text>();
            ImageAspectRatioFitter = QuestionImage.GetComponent<AspectRatioFitter>();

            ExplanationText = ExplanantionScrollView.content.GetComponent<Text>();

            foreach (Transform child in MultiAnswersUI.transform)
            {
                MultiAnswerButtons.Add(child.GetComponent<Button>());
            }

            foreach (Button btn in MultiAnswerButtons)
            {
                MultiAnswerButtonText.Add(btn.GetComponentInChildren<Text>());
            }

            LightboxImageAspectRatioFitter = LightboxImageHolder.GetComponent<AspectRatioFitter>();
        }

        private void CreateCacheFolders()
        {
            CategoriesCacheFolder = Path.Combine(Application.temporaryCachePath, "Categories").Replace(Path.DirectorySeparatorChar, '/');
            ImagesCacheFolder = Path.Combine(Application.temporaryCachePath, "Images").Replace(Path.DirectorySeparatorChar, '/');

            Directory.CreateDirectory(CategoriesCacheFolder);
            Directory.CreateDirectory(ImagesCacheFolder);
        }

        public void OpenHomepage()
        {
            Homepage.SetActive(true);

            CategoriesPage.SetActive(false);
            GamePage.SetActive(false);
            PausePage.SetActive(false);
            GameOverPage.SetActive(false);

            ProfilePage.SetActive(false);
            SettingsPage.SetActive(false);

            ProfileButton.SetActive(Categories.Count > 0);
            SettingsButton.SetActive(true);

            BackButton.SetActive(false);

            ToolbarTitle.text = "Home";

            // Stop the Get Categories request
            if (GetCategoriesCoroutine != null)
            {
                StopCoroutine(GetCategoriesCoroutine);
            }
        }

        public void OpenProfilePage()
        {
            ProfilePage.SetActive(true);

            ProfileButton.SetActive(false);
            SettingsButton.SetActive(false);

            BackButton.SetActive(true);

            ToolbarTitle.text = "Profile";
        }

        public void OpenSettingsPage()
        {
            SettingsPage.SetActive(true);

            ProfileButton.SetActive(false);
            SettingsButton.SetActive(false);

            BackButton.SetActive(true);

            ToolbarTitle.text = "Settings";
        }

        public void ToggleSound(bool init = false)
        {
            int SoundState = PlayerPrefs.GetInt(Globals.instance.SoundPrefName, 1);

            if (!init)
            {
                SoundState = SoundState == 1 ? 0 : 1;
                PlayerPrefs.SetInt(Globals.instance.SoundPrefName, SoundState);
            }

            if (SoundState == 0)
            {
                SoundManager.instance.SoundEnabled = false;
                SoundButton.image.color = Globals.instance.RedColor;
            }
            else if (SoundState == 1)
            {
                SoundManager.instance.SoundEnabled = true;
                SoundButton.image.color = Globals.instance.GreenColor;
            }
        }

        public void ToggleVibration(bool init = false)
        {
            int VibrationState = PlayerPrefs.GetInt(Globals.instance.VibrationPrefName, 1);

            if (!init)
            {
                VibrationState = VibrationState == 1 ? 0 : 1;
                PlayerPrefs.SetInt(Globals.instance.VibrationPrefName, VibrationState);
            }

            if (VibrationState == 0)
            {
                VibrationEnabled = false;
                VibrationButton.image.color = Globals.instance.RedColor;
            }
            else if (VibrationState == 1)
            {
                VibrationEnabled = true;
                VibrationButton.image.color = Globals.instance.GreenColor;
            }
        }

        public void DeleteCache()
        {
            DirectoryInfo DirInfo = new DirectoryInfo(Application.temporaryCachePath);

            foreach (FileInfo file in DirInfo.GetFiles("*", SearchOption.AllDirectories))
                file.Delete();

            CreateCacheFolders();

            StartCoroutine(DeletionComplete());
        }

        private IEnumerator DeletionComplete()
        {
            string OriginalText = CacheButtonText.text;

            CacheButton.interactable = false;
            CacheButtonText.text = "Done";

            yield return new WaitForSeconds(3f);

            CacheButton.interactable = true;
            CacheButtonText.text = OriginalText;
        }

        public void StartGame()
        {
            if (!PlayerPrefs.HasKey(Globals.instance.SelectedLanguagePrefName))
            {
                ToggleLanguageDialog(true);
                return;
            }

            OpenCategoriesPage();
        }

        public void ToggleLanguageDialog(bool active = true)
        {
            LanguageDialog.SetActive(active);
        }

        internal void SaveLanguageSelection(int id)
        {
            PlayerPrefs.SetInt(Globals.instance.SelectedLanguagePrefName, id);
            ToggleLanguageDialog(false);
        }

        private void OpenCategoriesPage(bool initiate = true)
        {
            Homepage.SetActive(false);
            CategoriesPage.SetActive(true);

            if (initiate)
                GetCategoriesList();
        }

        public void GetCategoriesList()
        {
            Categories.Clear();

            CategoryLoaderText.text = "Fetching Categories";

            CategoryLoader.SetActive(true);
            CategoriesList.gameObject.SetActive(false);
            ErrorPanel.SetActive(false);

            GetCategoriesCoroutine = StartCoroutine(GetCategroriesRequest());
        }

        private IEnumerator GetCategroriesRequest()
        {
            string RequestUri = Globals.instance.CategoriesURL();

            using (UnityWebRequest webRequest = UnityWebRequest.Get(RequestUri))
            {
                webRequest.timeout = Globals.instance.DefaultTimeout;

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                CategoryLoader.SetActive(false);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    ShowCategoryErrorPanel("Unable to fetch categories");

                    Debug.Log(webRequest.error);

                    yield break;
                }

                string responseData = $"{{ \"Categories\": { webRequest.downloadHandler.text } }}";
                Categories = JsonUtility.FromJson<CategoryList>(responseData).Categories;

                foreach (Transform child in CategoriesList.content)
                {
                    Destroy(child.gameObject);
                }

                foreach (Transform child in ProfileCategoriesList.content)
                {
                    Destroy(child.gameObject);
                }

                Categories.ForEach(Category =>
                {
                    //Setup Category Page

                    RectTransform CategoryHolder = Instantiate(CategoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), CategoriesList.content.transform);
                    CategoryHolder.localScale = new Vector3(1, 1, 1);

                    CategoryController CategoryHolderScript = CategoryHolder.GetComponent<CategoryController>();

                    CategoryHolderScript.CategoryName.text = Category.name;
                    CategoryHolderScript.CategoryDetails = Category;

                    //Setup Profile Page

                    RectTransform ProfileCategoryHolder = Instantiate(ProfileCategoryPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), ProfileCategoriesList.content.transform);
                    ProfileCategoryHolder.localScale = new Vector3(1, 1, 1);

                    ProfileCategoryController ProfileCategoryHolderScript = ProfileCategoryHolder.GetComponent<ProfileCategoryController>();

                    ProfileCategoryHolderScript.CategoryName.text = Category.name;
                    ProfileCategoryHolderScript.CategoryId = Category.id;
                });

                CategoriesList.content.transform.localPosition = new Vector3(0, 0, 0);
                CategoriesList.gameObject.SetActive(true);
            }
        }

        internal void LoadCategory(CategoryFormat CategoryDetails)
        {
            CurrentCategory = CategoryDetails;

            CategoryLoaderText.text = "Loading Category";

            CategoryLoader.SetActive(true);
            CategoriesList.gameObject.SetActive(false);
            ErrorPanel.SetActive(false);

            Questions.Clear();

            StartCoroutine(GetQuestionsRequest());
        }

        private IEnumerator GetQuestionsRequest()
        {
            string RequestUri = Globals.instance.QuestionsURL(CurrentCategory.id, CurrentCategory.questionLimit);

            using (UnityWebRequest webRequest = UnityWebRequest.Get(RequestUri))
            {
                webRequest.timeout = Globals.instance.DefaultTimeout;

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                CategoryLoader.SetActive(false);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    ShowCategoryErrorPanel("Unable to load category", false);

                    Debug.Log(webRequest.error);

                    yield break;
                }

                string responseData = $"{{ \"Questions\": { webRequest.downloadHandler.text } }}";
                Questions = JsonUtility.FromJson<QuestionList>(responseData).Questions;

                ShuffleQuestions();
                SetupGame();
            }
        }

        private void ShuffleQuestions()
        {
            for (int index = 0; index < Questions.Count; index++)
            {
                QuestionFormat tempNumber = Questions[index];

                int randomIndex = Random.Range(index, Questions.Count);

                Questions[index] = Questions[randomIndex];

                Questions[randomIndex] = tempNumber;
            }
        }

        private void ShowCategoryErrorPanel(string text, bool retryCategories = true)
        {
            ErrorPanelText.text = text;

            ErrorPanelButton.onClick.RemoveAllListeners();

            if (retryCategories)
                ErrorPanelButton.onClick.AddListener(GetCategoriesList);
            else
                ErrorPanelButton.onClick.AddListener(delegate { LoadCategory(CurrentCategory); });

            ErrorPanel.SetActive(true);
        }

        private void SetupGame()
        {
            ImageLightbox.SetActive(false);

            TimerManager.instance.StopTimer();

            TimerManager.instance.ToggleUI(CurrentCategory.enableTimer);

            LivesUI.SetActive(CurrentCategory.enableLives);

            if (CurrentCategory.enableLives)
            {
                LivesLeft = CurrentCategory.livesAmount;
                UpdateLives(false);
            }

            CurrentScore = 0;

            CategoriesPage.SetActive(false);

            GamePage.SetActive(true);

            SetupQuestion();
        }

        private void SetupQuestion()
        {
            CurrentQuestion = Questions[0];

            QuestionText.text = CurrentQuestion.question.Replace("<br>", "\n").Replace("<br />", "\n");

            ExplanationText.text = "\n" + CurrentQuestion.explanation.Trim().Replace("<br>", "\n").Replace("<br />", "\n") + "\n";

            if (CurrentCategory.enableTimer)
                TimerManager.instance.ResetTimer(CurrentCategory.timerAmount);

            StartCoroutine(ResetQuestionView());

            QuestionImage.texture = null;

            ImageLightbox.SetActive(false);

            TofAnswersUI.SetActive(false);

            MultiAnswersUI.SetActive(false);

            TogglePostQuestionButtons(false);

            ToggleExplanationPanel(false);

            if (CurrentQuestion.image.filename == null)
            {
                QuestionImage.gameObject.SetActive(false);

                SetupAnswers();
            }
            else
            {
                QuestionImage.gameObject.SetActive(true);

                QuestionImage.enabled = false;

                ImageLoader.SetActive(false);

                LoadImage();
            }
        }

        private IEnumerator ResetQuestionView()
        {
            yield return new WaitForEndOfFrame();

            QuestionScrollView.verticalNormalizedPosition = 1;
        }

        private void SetupAnswers()
        {
            if (CurrentQuestion.type == "tof")
            {
                TofAnswersUI.SetActive(true);

                TrueButton.SetActive(true);
                FalseButton.SetActive(true);

                TofAnswerHeader.gameObject.SetActive(false);
            }
            else
            {
                ShuffleAnswers();

                foreach (Button Btn in MultiAnswerButtons)
                {
                    Btn.gameObject.SetActive(false);
                }

                for (int index = 0; index < CurrentQuestion.answers.Count; index++)
                {
                    MultiAnswerButtons[index].gameObject.SetActive(true);
                    MultiAnswerButtonText[index].text = CurrentQuestion.answers[index].answer;
                }

                MultiAnswersUI.SetActive(true);
            }

            ToggleAnswersInteractability(true);

            TimerManager.instance.StartTimer();
        }

        private void ShuffleAnswers()
        {
            for (int index = 0; index < CurrentQuestion.answers.Count; index++)
            {
                AnswerFormat tempNumber = CurrentQuestion.answers[index];

                int randomIndex = Random.Range(index, CurrentQuestion.answers.Count);

                CurrentQuestion.answers[index] = CurrentQuestion.answers[randomIndex];

                CurrentQuestion.answers[randomIndex] = tempNumber;
            }
        }

        private void ToggleAnswersInteractability(bool activate = true)
        {
            AnswerGroup.blocksRaycasts = activate;
        }

        public void LoadImage()
        {
            ImageErrorPanel.SetActive(false);

            string ImageCachePath = Path.Combine(ImagesCacheFolder, CurrentQuestion.image.filename).Replace(Path.DirectorySeparatorChar, '/');

            if (File.Exists(ImageCachePath))
            {
                StartCoroutine(LoadCachedImage(ImageCachePath));
            }
            else
            {
                StartCoroutine(DownloadAndCacheImage(ImageCachePath));
            }
        }

        private IEnumerator LoadCachedImage(string path)
        {
            using (UnityWebRequest CachedImageRequest = UnityWebRequestTexture.GetTexture("file:///" + path))
            {
                //Wait for the image to load
                yield return CachedImageRequest.SendWebRequest();

                if (CachedImageRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Image Request error - " + CachedImageRequest.error);
                    yield break;
                }

                Texture2D imgTex = DownloadHandlerTexture.GetContent(CachedImageRequest);

                //Load the texture to the image display
                QuestionImage.texture = LightboxImageHolder.texture = imgTex;

                ImageAspectRatioFitter.aspectRatio = LightboxImageAspectRatioFitter.aspectRatio = (float)imgTex.width / imgTex.height;

                QuestionImage.enabled = true;

                SetupAnswers();
            }
        }

        private IEnumerator DownloadAndCacheImage(string savePath)
        {
            ImageLoader.SetActive(true);

            string RequestUri = Globals.instance.ImageURL(CurrentQuestion.image.filename);

            using (UnityWebRequest DownloadImageRequest = UnityWebRequestTexture.GetTexture(RequestUri))
            {
                DownloadImageRequest.timeout = Globals.instance.DefaultTimeout;

                // Request and wait for the desired page.
                yield return DownloadImageRequest.SendWebRequest();

                ImageLoader.SetActive(false);

                if (DownloadImageRequest.result != UnityWebRequest.Result.Success)
                {
                    ImageErrorPanel.SetActive(true);

                    Debug.Log(DownloadImageRequest.error);
                    yield break;
                }

                Texture2D DownloadedTexture = DownloadHandlerTexture.GetContent(DownloadImageRequest);

                if (DownloadedTexture.width != 8 && DownloadedTexture.height != 8)
                {
                    yield return new WaitForEndOfFrame();

                    File.WriteAllBytes(savePath, DownloadImageRequest.downloadHandler.data);

                    //Load the texture to the image display
                    StartCoroutine(LoadCachedImage(savePath));

                    Destroy(DownloadedTexture);
                }
            }
        }

        public void ToggleLightbox(bool activate = true)
        {
            if (QuestionImage.texture == null && activate)
                return;

            ImageLightbox.SetActive(activate);
        }

        public void ToFAnswer(bool isTrue)
        {
            HandleAnswer(CurrentQuestion.tofAnswer == isTrue, isTrue ? 0 : 1);
        }

        public void MultipleChoiceAnswer(int selected)
        {
            HandleAnswer(CurrentQuestion.answers[selected].correct, selected);
        }

        internal void HandleAnswer(bool correct, int selected, bool noResponse = false)
        {
            TimerManager.instance.StopTimer();
            SoundManager.instance.StopAudioPlayer();

            ToggleAnswersInteractability(false);

            QuestionFormat AnsweredQuestion = CurrentQuestion;

            if (correct)
            {
                SoundManager.instance.PlaySound(Globals.instance.CorrectAnswerSound);

                if (CurrentQuestion.type == "tof")
                {
                    TofAnswerHeader.text = "You are Correct!";
                    TofAnswerHeader.gameObject.SetActive(true);

                    TrueButton.SetActive(selected == 0);
                    FalseButton.SetActive(selected == 1);
                }

                if (CurrentCategory.enableTimer)
                    UpdateScore(Globals.instance.PointsPerSecond * TimerManager.instance.TimeLeft);
                else
                    UpdateScore(Globals.instance.PointsPerQuestion);
            }
            else
            {
                LivesLeft--;
                UpdateLives();

                SoundManager.instance.PlaySound(Globals.instance.WrongAnswerSound);

                if (VibrationEnabled)
                {
#if UNITY_ANDROID || UNITY_IOS
                    Handheld.Vibrate();
#endif
                }

                if (CurrentQuestion.type == "tof")
                {
                    TofAnswerHeader.text = "The Correct Answer is";
                    TofAnswerHeader.gameObject.SetActive(true);

                    TrueButton.SetActive(CurrentQuestion.tofAnswer == true);
                    FalseButton.SetActive(CurrentQuestion.tofAnswer == false);
                }
                else
                {
                    if (!noResponse)
                        MultiAnswerButtons[selected].image.color = Globals.instance.RedColor;
                }
            }

            for (int x = 0; x < CurrentQuestion.answers.Count; x++)
                if (CurrentQuestion.answers[x].correct)
                    MultiAnswerButtons[x].image.color = Globals.instance.GreenColor;

            StartCoroutine(PostQuestion());
        }

        private void UpdateScore(int score)
        {
            CurrentScore += score;
        }

        private void UpdateLives(bool animate = true)
        {
            LivesText.text = LivesLeft.ToString();

            if (animate)
            {
                LivesText.rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                LivesText.rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
            }
        }

        private IEnumerator PostQuestion()
        {
            yield return new WaitForSeconds(Globals.instance.NextQuestionDelay);

            if (Globals.instance.showPostQuestionButtons == ShowPostQuestionButtons.Never)
                StartCoroutine(NextQuestion());
            else if (Globals.instance.showPostQuestionButtons == ShowPostQuestionButtons.IfExplanationExists && string.IsNullOrEmpty(CurrentQuestion.explanation))
                StartCoroutine(NextQuestion());
            else
                TogglePostQuestionButtons(true);
        }

        public void GoToNextQuestion()
        {
            StartCoroutine(NextQuestion());
        }

        private IEnumerator NextQuestion()
        {
            if (LivesLeft <= 0)
            {
                GameOver();
                yield break;
            }

            Questions.RemoveAt(0);

            if (Questions.Count > 0)
                SetupQuestion();
            else
                GameOver();
        }

        private void TogglePostQuestionButtons(bool active)
        {
            PostQuestionButtons.SetActive(active);

            ExplanationButton.SetActive(!string.IsNullOrEmpty(CurrentQuestion.explanation));

            if (active)
            {
                TofAnswersUI.SetActive(false);
                MultiAnswersUI.SetActive(false);
            }
        }

        public void ToggleExplanationPanel(bool active)
        {
            ExplanantionPanel.SetActive(active);

            if (active)
                StartCoroutine(ResetExplanationView());
        }

        private IEnumerator ResetExplanationView()
        {
            yield return new WaitForEndOfFrame();

            ExplanantionScrollView.verticalNormalizedPosition = 1;
        }

        private void GameOver()
        {
            GameOverPage.SetActive(true);

            GamePage.SetActive(false);

            CategoryImage.texture = CurrentCategory.image;
            CategoryName.text = CurrentCategory.name;

            ScoreText.text = "Score: " + CurrentScore;

            int highscrore = PlayerPrefs.GetInt(CurrentCategory.id.ToString(), 0);

            if (CurrentScore > highscrore)
                PlayerPrefs.SetInt(CurrentCategory.id.ToString(), CurrentScore);
        }

        public void TogglePauseMenu(bool activate)
        {
            PausePage.SetActive(activate);

            if (activate)
                TimerManager.instance.PauseTimer();
        }

        public void ResumeGame()
        {
            PausePage.SetActive(false);
            TimerManager.instance.StartTimer(true);
        }

        public void RestartGame()
        {
            OpenHomepage();
            OpenCategoriesPage(false);
            LoadCategory(CurrentCategory);
        }

        public void QuitGame()
        {
            OpenHomepage();
        }

        private void OnDestroy()
        {
            ErrorPanelButton.onClick.RemoveAllListeners();
        }
    }
}