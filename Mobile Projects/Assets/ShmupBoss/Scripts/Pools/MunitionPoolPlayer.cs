using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Pools all munition used by the player, all munition weapons for the player are catagorized 
    /// and their munition indexed by their name.<br></br>
    /// Munition with identical names will be treated as the same munition, please use unique 
    /// names for each munition.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Pools/Munition Pool Player")]
    public class MunitionPoolPlayer : MunitionPool<MunitionPoolPlayer>
    {
        protected override int Layer
        {
            get
            {
                return ProjectLayers.PlayerMunition;
            }
        }

        /// <summary>
        /// Returns if the player can be found inside the level instance and thus its weapons indexed or not.
        /// </summary>
        protected override bool canListWeapons
        {
            get
            {
                if (Level.Instance == null)
                {
                    return false;
                }

                if (Level.Instance.PlayerComp == null)
                {
                    return false;
                }

                return true;
            }
        }

        private Player player;

        protected override void Awake()
        {
            base.Awake();
            Initializer.Instance.SubscribeToStage(Pool, 2);
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled Player Munition -----").transform;
        }

        protected override void ListWeapons()
        {
            player = Level.Instance.PlayerComp;

            for (int i = 0; i < player.MunitionWeapons.Length; i++)
            {
                WeaponMunition weaponMunition = player.MunitionWeapons[i];

                if (weaponMunition == null)
                {
                    Debug.Log("MunitionPoolPlayer.cs: player munition weapons slot number: " + i + " is empty.");
                    continue;
                }

                WeaponsMunition.Add(weaponMunition);
            }
        }

        protected override void GoThroughWeaponsLists()
        {
            foreach (WeaponMunition weaponMunition in WeaponsMunition)
            {
                if (weaponMunition is WeaponMunitionPlayer weaponMunitionPlayer)
                {
                    foreach (WeaponMunitionStage stage in weaponMunitionPlayer.Stages)
                    {
                        Munition munition = stage.MunitionPrefab;

                        if (munition == null)
                        {
                            Debug.Log("MunitionPool.cs: A munition weapon doesn't seem to have a munition plugged in.");
                            continue;
                        }

                        CreateAndIndexMunition(munition);
                    }
                }
            }
        }
    }
}