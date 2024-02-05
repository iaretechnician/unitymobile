using UnityEngine;
using DG.Tweening;

namespace CoreTrivia
{
    [RequireComponent(typeof(RectTransform))]
    public class BounceAnimation : MonoBehaviour
    {
        public float StartValue = 0.6f;

        [Space(10)]

        public float Duration = 1f;
        public float Delay = 0f;

        private RectTransform Rect;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            ResetScale();
        }

        private void ResetScale()
        {
            Rect.localScale = new Vector3(StartValue, StartValue, StartValue);
        }

        private void OnEnable()
        {
            Rect.DOScale(1, Duration).SetEase(Ease.OutBounce).SetDelay(Delay);
        }

        private void OnDisable()
        {
            Rect.DOKill();
            ResetScale();
        }
    }
}