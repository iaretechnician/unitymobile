using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This field determines where agents (player and enemies) and effects are spawned and the movement 
    /// bounds of the player and the AI mover.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Fields/Play Field")]
    public class PlayField : GameField
    {
        public static PlayField Instance;

        /// <summary>
        /// Returns the rect that will set the collider for the playfield, this is essentially the playfield.
        /// </summary>
        public Rect Boundries
        {
            get
            {
                return boundries;
            }
        }

        protected override Color gizmosColor
        {
            get
            {
                return Color.yellow;
            }
        }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void Initialize()
        {
            FindFieldBoundries();
            AddRigidBody2D();
            AddBoxCollider2D();
        }

#if UNITY_EDITOR
        protected override Vector2 GetGizmoLabelPosition()
        {
            return new Vector2(boundries.xMin + ProjectConstants.LabelMarginDistance, boundries.yMax);
        }
#endif
    }
}