using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A basic projectile which uses a rigibody 2D to move in a predetermined direction.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Munition/Bullet")]
    public class Bullet : Munition
    {
        [SerializeField]
        protected float speed;
        public float Speed
        {
            get
            {
                if (gameObject.layer == ProjectLayers.PlayerMunition)
                {
                    return speed * CurrentMultiplier.PlayerMunitionSpeedMultiplier;
                }
                else
                {
                    return speed * CurrentMultiplier.EnemiesMunitionSpeedMultiplier;
                }     
            }
        }

        private Rigidbody2D rb2D;

        protected override void OnValidate()
        {
            base.OnValidate();

            if(speed <= 0) 
            {
                speed = 5.0f;
            }
        }

        private void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();

            if(rb2D == null)
            {
                rb2D = gameObject.AddComponent<Rigidbody2D>();
                rb2D.isKinematic = true;
            }
        }

        public override void Initialize(Vector3 weaponPosition, Quaternion firingRotation, Vector2 agentDirection)
        {
            if (rb2D == null)
            {
                return;
            }

            transform.position = weaponPosition;

            if (!isIndependentOfWeaponRotation)
            {
                Vector3 firingRotationInAngles = firingRotation.eulerAngles;
                Vector3 firing2DRotation = new Vector3(0.0f, 0.0f, firingRotationInAngles.z);

                Quaternion bulletRotation = Quaternion.Euler(firing2DRotation);

                transform.rotation = bulletRotation;

                // The 90 degree rotation angle substraction is due to the convention used for the vector to degree.
                float rotationAngle = Math2D.VectorToDegree(agentDirection) - 90.0f;

                transform.RotateAround(transform.position, Vector3.forward, rotationAngle);
            }

            rb2D.velocity = (firingRotation.normalized * agentDirection.normalized).normalized * Speed;
        }
    }
}