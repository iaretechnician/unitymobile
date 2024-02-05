using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// An optional field which you can use to destroy weapon particles when they hit the boundries as an alternative 
    /// to using the distance settings in the particle weapon.<br></br>
    /// Please note that if you are using the particle hit fx, the FX will be spawned when the particle exits this field, 
    /// you should avoid using this field if you intend to use a particle hit FX or you could make the offset large enough so 
    /// that the user cannot see the fx.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Fields/Particle Destruction Field")]
    public class ParticleDestructionField : GameField
    {
        /// <summary>
        /// This field uses a group of colliders instead of one due to how a particle system works. 
        /// I just added the collider thickness so that you can more easily see the colliders which are 
        /// being created.
        /// </summary>
        private const float collidersThickness = 1.0f;
        
        protected override Color gizmosColor
        {
            get
            {
                return Color.black;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            gameObject.layer = ProjectLayers.ParticleDestroyer;
        }

        protected override void Initialize()
        {
            FindFieldBoundries();
            AddBoxColliders();
        }

        /// <summary>
        /// This approach is needed instead of the single box collider of the GameField base class because 
        /// particles are destroyed on hitting this layer instead of being despawned on an exit trigger like 
        /// the rest of the fields.
        /// </summary>
        private void AddBoxColliders()
        {
            BoxCollider2D colliderUp = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D colliderDown = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D colliderRight = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D colliderLeft = gameObject.AddComponent<BoxCollider2D>();

            colliderUp.size = new Vector2(boundries.width + collidersThickness, collidersThickness);
            colliderDown.size = new Vector2(boundries.width + collidersThickness, collidersThickness);
            colliderRight.size = new Vector2(collidersThickness, boundries.height + collidersThickness);
            colliderLeft.size = new Vector2(collidersThickness, boundries.height + collidersThickness);

            if (LevelCamera.Instance == null)
            {
                return;
            }

            if (fieldSource == FieldSource.Viewfield)
            {
                SetColliderCameraOffset1(colliderUp);
                SetColliderCameraOffset2(colliderDown);
                SetColliderCameraOffset3(colliderRight);
                SetColliderCameraOffset4(colliderLeft);
            }
        }

        private void SetColliderCameraOffset1(BoxCollider2D collider)
        {
            collider.offset = LevelCamera.Instance.GetCameraShiftedPosition() + new Vector2(0.0f, (boundries.height * 0.5f) + (collidersThickness * 0.5f));
        }

        private void SetColliderCameraOffset2(BoxCollider2D collider)
        {
            collider.offset = LevelCamera.Instance.GetCameraShiftedPosition() + new Vector2(0.0f, -(boundries.height * 0.5f) - (collidersThickness * 0.5f));
        }

        private void SetColliderCameraOffset3(BoxCollider2D collider)
        {
            collider.offset = LevelCamera.Instance.GetCameraShiftedPosition() + new Vector2((boundries.width * 0.5f) + (collidersThickness * 0.5f), 0.0f);
        }

        private void SetColliderCameraOffset4(BoxCollider2D collider)
        {
            collider.offset = LevelCamera.Instance.GetCameraShiftedPosition() + new Vector2(-(boundries.width * 0.5f) - (collidersThickness * 0.5f), 0.0f);
        }

#if UNITY_EDITOR
        protected override Vector2 GetGizmoLabelPosition()
        {
            return new Vector2(boundries.center.x + ProjectConstants.LabelMarginDistance, boundries.yMin);
        }
#endif
    }
}