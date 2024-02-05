using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// A button which when pressed will open the indexed level.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/UI/Level Button")]
    public class LevelButton : MonoBehaviour
    {
        public int LevelIndex
        {
            get;
            set;
        }

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(GoToLevel);
        }

        public void GoToLevel()
        {
            GameManager.Instance.ToLevel(LevelIndex + GameManager.Instance.MinLevelSceneIndex);
        }
    }
}