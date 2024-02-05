using UnityEngine;
using DG.Tweening;

namespace CoreTrivia
{
    [RequireComponent(typeof(RectTransform))]
    public class RotateAnimation : MonoBehaviour
    {
        public float Duration = 1f;
        public int loops = -1;
        public LoopType loopType = LoopType.Restart;

        private RectTransform Rect;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Rect.DOLocalRotate(new Vector3(0, 0, -360), Duration, RotateMode.FastBeyond360).SetLoops(loops, loopType).SetEase(Ease.Linear);
        }

        private void OnDisable()
        {
            Rect.DOKill();
            Rect.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}