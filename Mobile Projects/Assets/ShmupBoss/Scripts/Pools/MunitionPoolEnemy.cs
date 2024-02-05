using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Pools all munition used by the enemies, all munition weapons for the enemies are catagorized 
    /// and their munition indexed by their name.<br></br>
    /// Munition with identical names will be treated as the same munition, please use unique 
    /// names for each munition.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Pools/Munition Pool Enemy")]
    public class MunitionPoolEnemy : MunitionPool<MunitionPoolEnemy>
    {
        protected override int Layer
        {
            get
            {
                return ProjectLayers.EnemyMunition;
            }
        }


        /// <summary>
        /// Finds if the weapons can be found by accessig the enemies already pooled by the enemy pool.
        /// </summary>
        protected override bool canListWeapons
        {
            get
            {
                if (EnemyPool.Instance == null)
                {
                    return false;
                }

                if (EnemyPool.Instance.EnemyByName.Count == 0)
                {
                    return false;
                }

                return true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Initializer.Instance.SubscribeToStage(Pool, 2);
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled Enemies Munition -----").transform;
        }

        protected override void ListWeapons()
        {
            foreach(var kvp in EnemyPool.Instance.EnemyByName)
            {
                for(int i =0; i < kvp.Value.MunitionWeapons.Length; i++)
                {
                    WeaponMunition weaponMunition = kvp.Value.MunitionWeapons[i];

                    if(weaponMunition == null)
                    {
                        Debug.Log("MunitionPoolEnemy.cs: One of your enemies called: " + kvp.Key + " munition weapons slots is empty.");
                        
                        continue;
                    }

                    WeaponsMunition.Add(weaponMunition);
                }                
            }
        }

        protected override void GoThroughWeaponsLists()
        {
            foreach (WeaponMunition weaponMunition in WeaponsMunition)
            {
                if (weaponMunition is WeaponMunitionEnemy weaponMunitionEnemy)
                {
                    Munition munition = weaponMunitionEnemy.CurrentStage.MunitionPrefab;

                    if (munition == null)
                    {
                        Debug.Log("MunitionPools.cs: A munition weapon doesn't seem to have " +
                            "a munition plugged in and its munition can't be pooled.");

                        continue;
                    }

                    CreateAndIndexMunition(munition);
                }
            }
        }
    }
}