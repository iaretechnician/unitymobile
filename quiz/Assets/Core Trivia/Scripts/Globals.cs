using System.IO;
using UnityEngine;

namespace CoreTrivia
{
    public enum ShowPostQuestionButtons
    {
        Always,
        Never,
        IfExplanationExists
    }

    public class Globals : MonoBehaviour
    {
        internal static Globals instance;

        [Header("Server Config")]

        public string BaseURL;

        [Tooltip("Duration in seconds")]
        public int DefaultTimeout = 60;

        [Header("Sounds")]

        public AudioClip CorrectAnswerSound;
        public AudioClip WrongAnswerSound;

        [Space(5)]

        public AudioClip TickingSound;

        [Header("Images")]

        public Sprite CorrectAnswerImage;
        public Sprite WrongAnswerImage;

        [Header("Gameplay")]

        [Help("Points Per Question is used to calculate the score ONLY if the timer is disabled. Otherwise, Points Per Second is used.")]
        [Range(1, 1000)]
        public int PointsPerQuestion = 100;

        [Range(1, 100)]
        public int PointsPerSecond = 10;

        [Space(10)]

        public ShowPostQuestionButtons showPostQuestionButtons = ShowPostQuestionButtons.Always;

        [Tooltip("Time in seconds")]
        [Range(0, 10)]
        public float NextQuestionDelay = 3;

        [Header("Colors")]

        public Color GreenColor = Color.green;
        public Color RedColor = Color.red;

        internal string SelectedLanguagePrefName = "SelectedLanguage";

        internal string SoundPrefName = "SoundState";
        internal string VibrationPrefName = "VibrationState";

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
        }

        internal string LanguagesURL()
        {
            return Path.Combine(BaseURL, "api", "languages", "all").Replace(Path.DirectorySeparatorChar, '/');
        }

        internal string CategoriesURL()
        {
            int ActiveLanguageId = PlayerPrefs.GetInt(SelectedLanguagePrefName);

            return Path.Combine(BaseURL, "api", "categories", "all", ActiveLanguageId.ToString()).Replace(Path.DirectorySeparatorChar, '/');
        }

        internal string QuestionsURL(int categoryId, int questionLimit)
        {
            return Path.Combine(BaseURL, "api", "questions", "category", categoryId.ToString(), questionLimit.ToString()).Replace(Path.DirectorySeparatorChar, '/');
        }

        internal string IconURL(int id)
        {
            return Path.Combine(BaseURL, "resources", "icons", id.ToString()).Replace(Path.DirectorySeparatorChar, '/');
        }

        internal string ImageURL(string name)
        {
            return Path.Combine(BaseURL, "resources", "images", name).Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}
