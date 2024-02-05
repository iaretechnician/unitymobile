using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Responisble for firing enemy weapons with options for delaying firing the weapons.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Shooters/Enemy Shooter")]
    [RequireComponent(typeof(Enemy))]
    public class EnemyShooter : MonoBehaviour
    {
        [Header("Munition Weapons Settings")]
        [Tooltip("Delay time before firing munition weapons.")]
        [SerializeField]
        private float firstMunitionShotDelayTime;

        /// <summary>
        /// When set to true and the munition weapon settings use the weapon rotation, 
        /// munition will be fired with the rotation of the enemy.
        /// </summary>
        [Tooltip("When checked and the munition weapon settings use the weapon rotation, " +
            "munition will be fired with the rotation of the enemy.")]
        [SerializeField]
        private bool isMunitionFollowingMovement;

        [Header("Particle Weapons Settings")]
        [Tooltip("Delay time before firing particle weapons.")]
        [SerializeField]
        private float firstParticleShotDelayTime;

        /// <summary>
        /// When set to true, particles will be fired with the rotation of the enemy.
        /// </summary>
        [Tooltip("When checked, particles will be fired with the rotation of the enemy.")]
        [SerializeField]
        private bool isParticleFollowingMovement;

        private Enemy enemy;
        private Mover mover;

        private bool canShootMunition;
        private bool canShootParticles;

        private Vector2 enemyDirection;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            mover = GetComponent<Mover>();
        }

        private void OnEnable()
        {
            if (Level.IsFinishedLoading)
            {
                StartCoroutine(StartShootingMunition());
                StartCoroutine(StartShootingParticles());
            }            
        }

        private void OnDisable()
        {
            canShootMunition = false;
            canShootParticles = false;
        }

        private IEnumerator StartShootingMunition()
        {
            yield return new WaitForSeconds(firstMunitionShotDelayTime);

            canShootMunition = true;

            yield break;
        }

        private IEnumerator StartShootingParticles()
        {
            yield return new WaitForSeconds(firstParticleShotDelayTime);

            canShootParticles = true;

            yield break;
        }

        private void Update()
        {
            if (!enemy.CanShoot)
            {
                return;
            }
            
            if (canShootMunition)
            {
                ShootMunitionWeapons();
            }

            if (canShootParticles)
            {
                ShootParticleWeapons();
            }
        }

        private void ShootMunitionWeapons()
        {
            for (int i = 0; i < enemy.MunitionWeapons.Length; i++)
            {
                if (enemy.MunitionWeapons[i] == null)
                {
                    continue;
                }

                if (!isMunitionFollowingMovement || mover == null)
                {
                    enemy.MunitionWeapons[i].FireWhenPossible(FacingDirections.NonPlayer);

                    continue;
                }

                enemyDirection = FindEnemyDirection();
                enemy.MunitionWeapons[i].FireWhenPossible(enemyDirection);
            }
        }

        private void ShootParticleWeapons()
        {
            for (int i = 0; i < enemy.ParticleWeapons.Length; i++)
            {
                if (enemy.ParticleWeapons[i] == null)
                {
                    continue;
                }

                if (!isParticleFollowingMovement || mover == null)
                {
                    enemy.ParticleWeapons[i].FireWhenPossible(FacingDirections.NonPlayer);

                    continue;
                }

                enemyDirection = FindEnemyDirection();
                enemy.ParticleWeapons[i].FireWhenPossible(enemyDirection);
            }
        }

        private Vector2 FindEnemyDirection()
        {
            if(mover.CurrentDirection == Vector2.zero)
            {
                return FacingDirections.NonPlayer;
            }

            return mover.CurrentDirection.normalized;
        }
    }
}