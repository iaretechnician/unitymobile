using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Component to shake the camera when a selected enemy event occurs.<br></br>
    /// For this component to work, you will need to have the camera tracking script
    /// applied in addition to the level camera script to your main camera.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Enemy/Camera Shake Enemy")]
    [RequireComponent(typeof(Enemy))]
    public class CameraShakeEnemy : MonoBehaviour
    {
        /// <summary>
        /// The enemy event that will cause the level camera to shake when it occurs.
        /// </summary>
        [Tooltip("The enemy event that will cause the level camera to shake when it occurs.")]
        [SerializeField]
        private EnemyEvent enemyEvent;

        private void Awake()
        {
            AgentCollidable enemy = GetComponent<Enemy>();
            enemy.Subscribe(ShakeCamera, AgentEventTranslator.FromEnemyEvent(enemyEvent));
        }

        private void ShakeCamera(System.EventArgs args)
        {       
            if(LevelCamera.Instance == null)
            {
                return;
            }

            LevelCamera.Instance.ShakeCamera();
        }
    }
}