using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShmupBoss
{
    /// <summary>
    /// This will enable a virtual image button to find if it is still pressed when it's held down.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Input/Hold Button")]
    [RequireComponent(typeof(Image))]
    public class HoldButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public Sprite PressedImage;
        private Sprite originalImage;

        private Image image;

        public bool IsPressed
        {
            get;
            private set;
        }

        void Start()
        {
            image = GetComponent<Image>();
            originalImage = image.sprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            image.sprite = PressedImage;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            image.sprite = originalImage;
        }
    }
}