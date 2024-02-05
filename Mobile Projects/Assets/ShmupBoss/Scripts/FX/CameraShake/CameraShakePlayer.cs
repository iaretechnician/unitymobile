using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Component to shake the camera when a selected player event occurs.<br></br>
    /// For this component to work, you will need to have the camera tracking script
    /// applied in addition to the level camera script to your main camera.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Player/Camera Shake Player")]
    [RequireComponent(typeof(Player))]
    public class CameraShakePlayer : MonoBehaviour
    {
        /// <summary>
        /// The player player that will cause the level camera to shake when it occurs.
        /// </summary>
        [Tooltip("The player event that will cause the level camera to shake when it occurs.")]
        [SerializeField]
        private PlayerEvent playerEvent;

        private void Awake()
        {
            AgentCollidable player = GetComponent<Player>();
            player.Subscribe(ShakeCamera, AgentEventTranslator.FromPlayerEvent(playerEvent));
        }

        private void ShakeCamera(System.EventArgs args)
        {
            if (LevelCamera.Instance == null)
            {
                return;
            }

            LevelCamera.Instance.ShakeCamera();
        }
    }
}