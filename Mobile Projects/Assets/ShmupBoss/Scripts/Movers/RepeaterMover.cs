using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A mover that will move an object in one of eight directions then after a certain distance 
    /// has been reached, place the object in the same position it started from and repeat 
    /// the movement over and over again.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Background/Repeater Mover")]
    public class RepeaterMover : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        /// <summary>
        /// The direction the object will move in.
        /// </summary>
        [Tooltip("The direction the object will move in.")]
        [SerializeField]
        private EightDirection directionOption;

        /// <summary>
        /// The distance which after it has been crossed the object will be placed back in its starting postion.
        /// </summary>
        [Tooltip("The distance which after it has been crossed the object will be placed back in its " +
            "starting postion.")]
        [SerializeField]
        private float repeatDistance;

        private Vector3 direction;
        private Vector3 startPosition;
        private float distanceFromStartPosition;

        private void Awake()
        {
            startPosition = transform.position;

            Vector2 direction2 = Directions.EightDirectionToVector2(directionOption);
            direction = new Vector3(direction2.x, direction2.y, 0.0f);
        }

        private void Update()
        {
            distanceFromStartPosition += speed * Time.deltaTime;
            transform.position = startPosition + (direction * distanceFromStartPosition);

            if(distanceFromStartPosition > repeatDistance)
            {
                distanceFromStartPosition = 0.0f;
            }
        }
    }
}