using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CoreTrivia
{
    [RequireComponent(typeof(Image))]
    public class FadeInAnimation : MonoBehaviour
    {
        [Range(0, 1)]
        public float StartOpacity = 0f;

        [Range(0, 1)]
        public float EndOpacity = 1f;

        [Space(10)]

        public float Duration = 1f;
        public float Delay = 0f;

        private Image thisImage;
        private Color InitialColor;

        private void Awake()
        {
            thisImage = GetComponent<Image>();
            InitialColor = thisImage.color;

            ResetColor();
        }

        private void ResetColor()
        {
            thisImage.color = InitialColor;
            thisImage.color = new Color(InitialColor.r, InitialColor.g, InitialColor.b, StartOpacity);
        }

        private void OnEnable()
        {
            thisImage.DOFade(EndOpacity, Duration).SetDelay(Delay);
        }

        private void OnDisable()
        {
            thisImage.DOKill();
            ResetColor();
        }
    }
}
