using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Changes a mover's speed after it has spawned based on the curve information you input.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Components/Mover Speed Curve Control")]
    [RequireComponent(typeof(Mover))]
    public class MoverSpeedCurveControl : MonoBehaviour
    {
        [Tooltip("The curve that multiplies the mover's speed over time.")]
        [SerializeField]
        private AnimationCurve curve;

        private Mover mover;
        private float TimeOnEnable;

        private void Awake()
        {
            mover = GetComponent<Mover>();

            if(mover == null)
            {
                Debug.Log("MoverSpeedCurveControl.cs: Cannot find a mover to control, please " +
                    "make sure to apply mover speed curve control to game objects with a mover.");
            }
        }

        private void OnEnable()
        {
            TimeOnEnable = Time.time;
        }

        private void Update()
        {
            float timeSinceEnabled = Time.time - TimeOnEnable;
            float newSpeed = curve.Evaluate(timeSinceEnabled) * mover.Speed;
            mover.ModifyCurrentSpeed(newSpeed);
        }
    }
}