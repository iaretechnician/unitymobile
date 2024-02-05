using UnityEngine;
using UnityEngine.UI;

namespace CoreTrivia
{
    [RequireComponent(typeof(Button))]
    public class LanguageSelector : MonoBehaviour
    {
        public Text NameTextField;

        internal int LanguageID;
        internal string LanguageName;
        internal LanguageDialog LanguageSelectionDialog;

        private Button thisButton;

        private void Awake()
        {
            thisButton = GetComponent<Button>();
            thisButton.onClick.AddListener(delegate { LanguageSelectionDialog.SelectLanguage(LanguageID); });
        }

        private void OnEnable()
        {
            EventManager.StartListening("update-language-selection", UpdateText);

            UpdateText();
        }

        private void UpdateText()
        {
            if (LanguageID == LanguageSelectionDialog.SelectedLanguage)
            {
                if (NameTextField.text.Contains("✓"))
                    return;

                NameTextField.text += " ✓";
            }
            else
            {
                NameTextField.text = NameTextField.text.Replace("✓", "").Trim();
            }
        }

        private void OnDisable()
        {
            EventManager.StopListening("update-language-selection", UpdateText);
        }

        private void OnDestroy()
        {
            thisButton.onClick.RemoveAllListeners();
        }
    }
}