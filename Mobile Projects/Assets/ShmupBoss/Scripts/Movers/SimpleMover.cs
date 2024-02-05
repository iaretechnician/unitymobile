using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Moves enemies in a single direction that can either be straight, diagonal or with a curved bend.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Simple Mover")]
    public class SimpleMover : MoverRotatableOnZ, ISideSpawnable
    {
        /// <summary>
        /// This value will give a diagonal rotation to the direction of movement.<br></br> 
        /// </summary>
        [Tooltip("This value will give a diagonal rotation to the direction of movement. Please be " +
            "conservative in the values you use especially if in conjunction with the bend value.")]
        [SerializeField]
        [Range(-1.0f, 1.0f)]
        private float diagonal;

        /// <summary>
        /// Will give a curved bend effect to the direction of movement.
        /// </summary>
        [Tooltip("Will give a curved bend effect to the direction of movement. Please be conservative in the " +
            "values you use especially if in conjunction with the diagonal value.")]
        [SerializeField]
        [Range(-1.0f, 1.0f)]
        private float bend;

        private float bendAmount;

        public FourDirection InitialDirection
        {
            get;
            set;
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool isDrawingGizmos;

        [SerializeField]
        private float pathLength;
#endif
        protected override void OnDisable()
        {
            base.OnDisable();

            bendAmount = 0.0f;
        }

        protected override void FindCurrentDirection()
        {
            bendAmount += bend * Time.deltaTime;

            float totalRotation = bendAmount + diagonal;

            switch (InitialDirection)
            {
                case FourDirection.Down:
                    CurrentDirection = new Vector2(totalRotation, -1.0f).normalized;
                    break;

                case FourDirection.Up:
                    CurrentDirection = new Vector2(totalRotation, 1.0f).normalized;
                    break;

                case FourDirection.Right:
                    CurrentDirection = new Vector2(1.0f, totalRotation).normalized;
                    break;

                case FourDirection.Left:
                    CurrentDirection = new Vector2(-1.0f, totalRotation).normalized;
                    break;

                default:
                    CurrentDirection = Vector2.zero;
                    break;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draws the line representing the simulated direction of movement that this mover will take.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!isDrawingGizmos)
            {
                return;
            }
            
            Gizmos.color = Color.yellow;

            int deltaNumber = Mathf.FloorToInt(pathLength / ProjectConstants.LineDeltaLength) - 1;

            float bend = 0;
            float totalRotation;

            Vector2 prePoint = gameObject.transform.position;
            Vector2 postPoint = gameObject.transform.position;

            for (int i = 0; i < deltaNumber; i++)
            {
                Vector2 simulatedDirection = Vector2.zero;

                bend += this.bend * ProjectConstants.LineDeltaLength;
                totalRotation = bend + diagonal;

                if (Level.IsVertical)
                {
                    simulatedDirection = new Vector2(totalRotation, -1.0f);
                }
                else if(Level.IsHorizontal)
                {
                    simulatedDirection = new Vector2(-1.0f, totalRotation);
                }

                simulatedDirection.Normalize();

                simulatedDirection *= ProjectConstants.LineDeltaLength * Speed;

                postPoint += simulatedDirection;

                Gizmos.DrawLine(Math2D.Vector2ToVector3(prePoint, transform.position.z),
                    Math2D.Vector2ToVector3(postPoint, transform.position.z));

                prePoint = postPoint;
            }
        }
#endif
    }
}