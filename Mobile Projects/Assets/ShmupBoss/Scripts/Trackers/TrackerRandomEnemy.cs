using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Uses a random enemy as its target and finds the vector to it, 
    /// direction, distance and when it has reached it or not.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Trackers/Tracker Random Enemy")]
    public class TrackerRandomEnemy : Tracker
    {
        private bool isTargetLost;

        /// <summary>
        /// The randomly selected enemy as target.
        /// </summary>
        private Enemy pickedRandomEnemy;

        protected override void Update()
        {
            CheckIfTargetIsLost();

            if (isTargetLost)
            {
                UpdateTarget();
            }

            UpdateTargetCoordinates();
        }

        private void CheckIfTargetIsLost()
        {
            if (pickedRandomEnemy == null)
            {
                SetTargetLost();
            }
            else if (!pickedRandomEnemy.isActiveAndEnabled || pickedRandomEnemy.IsInvincible)
            {
                SetTargetLost();
            }
        }

        /// <summary>
        /// After acquiring the list of active enemies in the scene, 
        /// a random one from the list is selected as a target.
        /// </summary>
        private void UpdateTarget()
        {
            List<IInGamePool> allEnemies = InGamePoolManager.GetList<Enemy>();

            if (allEnemies == null || allEnemies.Count <= 0)
            {
                SetTargetLost();
                return;
            }

            pickedRandomEnemy = allEnemies[Random.Range(0, allEnemies.Count)] as Enemy;

            if (pickedRandomEnemy.IsInvincible)
            {
                SetTargetLost();
                return;
            }

            SetTargetFound();
        }

        private void SetTargetFound()
        {
            TargetTransform = pickedRandomEnemy.transform;

            IsTargetFound = true;
            isTargetLost = false;
        }

        private void SetTargetLost()
        {
            pickedRandomEnemy = null;
            TargetTransform = null;

            IsTargetFound = false;
            isTargetLost = true;
        }
    }
}