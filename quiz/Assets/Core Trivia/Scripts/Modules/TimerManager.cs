using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CoreTrivia
{
    public class TimerManager : MonoBehaviour
    {
        internal static TimerManager instance;

        public GameObject TimerUI;
        public Image TimerProgressBar;
        public Text TimerText;

        internal int TimeLeft;

        private int TotalTime;
        private Coroutine TimerCoroutine;

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

        internal void ToggleUI(bool active)
        {
            TimerUI.SetActive(active);
        }

        internal void ResetTimer(int timerAmount)
        {
            TimeLeft = TotalTime = timerAmount;

            TimerProgressBar.fillAmount = 1;
            TimerText.text = TimeLeft.ToString();

            TimerProgressBar.DOKill();
        }

        internal void StartTimer(bool immediate = false)
        {
            if (TimerUI.activeInHierarchy)
                TimerCoroutine = StartCoroutine(StartCountdown(immediate));
        }

        private IEnumerator StartCountdown(bool immediate = false)
        {
            if (TimeLeft <= 0)
                yield break;

            if (!immediate)
                yield return new WaitForSeconds(1f);

            TimerProgressBar.DOFillAmount(0, TimeLeft).SetEase(Ease.Linear);

            while (TimeLeft > 0)
            {
                if (TimeLeft <= 5)
                    SoundManager.instance.PlaySound(Globals.instance.TickingSound);

                yield return new WaitForSeconds(1f);

                TimeLeft--;

                TimerText.text = TimeLeft.ToString();
            }

            if (TimeLeft == 0)
                MainController.instance.HandleAnswer(false, 0, true);
        }

        internal void PauseTimer()
        {
            StopTimer(true);
        }

        internal void StopTimer(bool pause = false)
        {
            if (TimerCoroutine != null)
                StopCoroutine(TimerCoroutine);

            TimerProgressBar.DOKill();

            if (pause)
                TimerProgressBar.fillAmount = TimeLeft / (float)TotalTime;

            SoundManager.instance.StopAudioPlayer();
        }
    }
}