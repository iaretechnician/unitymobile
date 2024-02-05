using UnityEngine;
using DG.Tweening;

namespace CoreTrivia
{
    [RequireComponent(typeof(RectTransform))]
    public class PulseAnimation : MonoBehaviour
    {
        [Range(0.5f, 2f)]
        public float EndValue = 0.9f;

        public float Duration = 1f;

        public int loops = -1;

        public LoopType loopType = LoopType.Yoyo;

        private RectTransform Rect;
        private Vector3 InitialScale;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            InitialScale = Rect.localScale;
        }

        private void OnEnable()
        {
            Rect.DOScale(EndValue, Duration).SetLoops(loops, loopType);
        }

        private void OnDisable()
        {
            Rect.DOKill();
            Rect.localScale = InitialScale;
        }
    }
}