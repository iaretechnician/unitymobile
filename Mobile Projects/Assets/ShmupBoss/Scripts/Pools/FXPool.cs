using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Pool for all effects used in this pack, this includes player or enemy effects such as explosions 
    /// and any weapons or munition related effects.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Pools/FX Pool")]
    public class FXPool : PoolBase
    {
        public static FXPool Instance;

        /// <summary>
        /// How many clones are pooled of each effect.
        /// </summary>
        [Tooltip("How many clones are pooled of each effect.")]
        [SerializeField]
        private int poolLimit;

        private List<WeaponMunitionPlayer> playerMunitionWeapons;
        private List<WeaponMunitionEnemy> enemyMunitionWeapons;

        private List<WeaponParticlePlayer> playerParticleWeapons;
        private List<WeaponParticleEnemy> enemyParticleWeapons;

        protected override int Layer
        {
            get
            {
                return ProjectLayers.FX;
            }
        }

        private void OnValidate()
        {
            if (poolLimit <= 0)
            {
                poolLimit = 10;
            }
        }

        protected void Awake()
        {
            Instance = this;            
            Initializer.Instance.SubscribeToStage(Pool, 3);
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled FX -----").transform;
        }

        protected override void Pool()
        {
            CreateHierarchy();

            IndexPlayerFX();
            IndexFXFromEnemyPool();

            ListPlayerWeapons();
            ListEnemyWeapons();

            IndexPlayerWeaponsFX();
            IndexEnemyWeaponsFX();
        }

        #region Index Agents FX
        /// <summary>
        /// Indexes all effects used by the player FX spawner such as explosions.
        /// </summary>
        private void IndexPlayerFX()
        {
            if (Level.Instance == null)
            {
                return;
            }

            if(Level.Instance.PlayerComp == null)
            {
                return;
            }

            Player player = Level.Instance.PlayerComp;

            GameObject playerGO = player.gameObject;
            FxSpawner fxPlayer = playerGO.GetComponent<FxSpawner>();

            if(fxPlayer == null)
            {
                return;
            }

            GameObject[] fxGOs = new GameObject[fxPlayer.FXs.Length];

            for (int i = 0; i < fxPlayer.FXs.Length; i++)
            {
                fxGOs[i] = fxPlayer.FXs[i].FxGo;
            }

            if (fxGOs == null)
            {
                return;
            }

            if (fxGOs.Length == 0)
            {
                return;
            }

            foreach (GameObject fxGO in fxGOs)
            {
                string fxName = fxGO.name;
                CreateAndIndexCloneAndQueue(fxName, fxGO, poolLimit);
            }
        }

        /// <summary>
        /// Indexes all effects used by the enemy FX spawner after looking into a list of all enemies which 
        /// have been already stored in the enemy pool.
        /// </summary>
        private void IndexFXFromEnemyPool()
        {
            if (EnemyPool.Instance == null)
            {
                return;
            }

            if (EnemyPool.Instance.EnemyByName.Count == 0)
            {
                return;
            }

            foreach (var kvp in EnemyPool.Instance.EnemyByName)
            {
                Enemy enemy = kvp.Value;

                GameObject enemyGO = enemy.gameObject;
                FxSpawner fxEnemy = enemyGO.GetComponent<FxSpawner>();

                if (fxEnemy == null)
                {
                    continue;
                }

                GameObject[] fxGOs = new GameObject[fxEnemy.FXs.Length];

                for (int i = 0; i < fxEnemy.FXs.Length; i++)
                {
                    fxGOs[i] = fxEnemy.FXs[i].FxGo;
                }

                if(fxGOs == null)
                {
                    continue;
                }

                if(fxGOs.Length == 0)
                {
                    continue;
                }

                foreach (GameObject fxGO in fxGOs)
                {
                    string fxName = fxGO.name;
                    CreateAndIndexCloneAndQueue(fxName, fxGO, poolLimit);
                }
            }
        }
        #endregion

        #region List Weapons
        private void ListPlayerWeapons()
        {
            if (Level.Instance == null)
            {
                return;
            }

            if (Level.Instance.PlayerComp == null)
            {
                return;
            }

            Player player = Level.Instance.PlayerComp;

            ListPlayerMunitionWeapons(player);
            ListPlayerParticleWeapons(player);
        }

        private void ListPlayerMunitionWeapons(Player player)
        {
            playerMunitionWeapons = new List<WeaponMunitionPlayer>();

            foreach (WeaponMunitionPlayer weaponMunitionPlayer in player.MunitionWeapons)
            {
                playerMunitionWeapons.Add(weaponMunitionPlayer);
            }
        }

        private void ListPlayerParticleWeapons(Player player)
        {
            playerParticleWeapons = new List<WeaponParticlePlayer>();

            foreach (WeaponParticlePlayer weaponParticlePlayer in player.ParticleWeapons)
            {
                playerParticleWeapons.Add(weaponParticlePlayer);
            }
        }

        private void ListEnemyWeapons()
        {
            if (EnemyPool.Instance == null)
            {
                return;
            }

            if (EnemyPool.Instance.EnemyByName.Count == 0)
            {
                return;
            }

            enemyMunitionWeapons = new List<WeaponMunitionEnemy>();
            enemyParticleWeapons = new List<WeaponParticleEnemy>();

            foreach (var kvp in EnemyPool.Instance.EnemyByName)
            {
                ListEnemyMunitionWeapons(kvp);
                ListEnemyParticleWeapons(kvp);
            }
        }

        private void ListEnemyMunitionWeapons(KeyValuePair<string, Enemy> kvp)
        {
            for (int i = 0; i < kvp.Value.MunitionWeapons.Length; i++)
            {
                WeaponMunitionEnemy weaponMunitionEnemy = kvp.Value.MunitionWeapons[i] as WeaponMunitionEnemy;

                if (weaponMunitionEnemy == null)
                {
                    continue;
                }

                enemyMunitionWeapons.Add(weaponMunitionEnemy);
            }
        }

        private void ListEnemyParticleWeapons(KeyValuePair<string, Enemy> kvp)
        {
            foreach (WeaponParticleEnemy weaponParticleEnemy in kvp.Value.ParticleWeapons)
            {
                enemyParticleWeapons.Add(weaponParticleEnemy);
            }
        }
        #endregion

        #region Index Weapon FX
        private void IndexPlayerWeaponsFX()
        {
            IndexPlayerMunitionWeaponsFX();
            IndexPlayerParticleWeaponsFX();
        }

        private void IndexPlayerMunitionWeaponsFX()
        {
            if (playerMunitionWeapons == null || playerMunitionWeapons.Count == 0)
            {
                return;
            }

            foreach (WeaponMunitionPlayer weaponMunitionPlayer in playerMunitionWeapons)
            {
                IndexWeaponFireFX(weaponMunitionPlayer);
                IndexPlayerWeaponsMunitionHitFX(weaponMunitionPlayer);
            }
        }

        private void IndexPlayerParticleWeaponsFX()
        {
            if (playerParticleWeapons == null || playerParticleWeapons.Count == 0)
            {
                return;
            }

            foreach (WeaponParticlePlayer weaponParticlePlayer in playerParticleWeapons)
            {
                IndexWeaponFireFX(weaponParticlePlayer);
                IndexWeaponParticleHitFX(weaponParticlePlayer);
            }
        }

        private void IndexEnemyWeaponsFX()
        {
            IndexEnemyMunitionWeaponsFX();
            IndexEnemyParticleWeaponsFX();
        }

        private void IndexEnemyMunitionWeaponsFX()
        {
            if (enemyMunitionWeapons == null || enemyMunitionWeapons.Count == 0)
            {
                return;
            }

            foreach (WeaponMunitionEnemy weaponMunitionEnemy in enemyMunitionWeapons)
            {
                IndexWeaponFireFX(weaponMunitionEnemy);
                IndexEnemyWeaponsMunitionHitFX(weaponMunitionEnemy);
            }
        }

        private void IndexEnemyParticleWeaponsFX()
        {
            if (enemyParticleWeapons == null || enemyParticleWeapons.Count == 0)
            {
                return;
            }

            foreach (WeaponParticleEnemy weaponParticleEnemy in enemyParticleWeapons)
            {
                IndexWeaponFireFX(weaponParticleEnemy);
                IndexWeaponParticleHitFX(weaponParticleEnemy);
            }
        }

        private void IndexWeaponFireFX(Weapon weapon)
        {
            if(weapon == null)
            {
                return;
            }
            
            GameObject weaponGO = weapon.gameObject;

            WeaponFireFX weaponFireFX = weaponGO.GetComponent<WeaponFireFX>();

            if (weaponFireFX == null)
            {
                return;
            }

            GameObject weaponFireFxGO = weaponFireFX.FxGo;
            string weaponFireFxName = weaponFireFX.FxGo.name;

            CreateAndIndexCloneAndQueue(weaponFireFxName, weaponFireFxGO, poolLimit);
        }

        private void IndexWeaponParticleHitFX(WeaponParticle weaponParticle)
        {
            if(weaponParticle == null)
            {
                return;
            }
            
            GameObject weaponGO = weaponParticle.gameObject;
            ParticleHitFX particleHitFX = weaponGO.GetComponent<ParticleHitFX>();

            if (particleHitFX == null)
            {
                return;
            }

            GameObject particleHitFxGO = particleHitFX.FxGo;
            string particleHitFxName = particleHitFX.FxGo.name;

            CreateAndIndexCloneAndQueue(particleHitFxName, particleHitFxGO, poolLimit);
        }

        private void IndexPlayerWeaponsMunitionHitFX(WeaponMunitionPlayer weaponMunitionPlayer)
        {
            for (int i =0; i < weaponMunitionPlayer.Stages.Length; i++)
            {
                Munition munition = weaponMunitionPlayer.Stages[i].MunitionPrefab;

                GameObject munitionGO = munition.gameObject;

                MunitionHitFX munitionHitFX = munitionGO.GetComponent<MunitionHitFX>();

                if (munitionHitFX == null)
                {
                    continue;
                }

                GameObject munitionHitFxGo = munitionHitFX.FxGo;
                string munitionHitFxName = munitionHitFX.FxGo.name;

                CreateAndIndexCloneAndQueue(munitionHitFxName, munitionHitFxGo, poolLimit);
            }
        }

        private void IndexEnemyWeaponsMunitionHitFX(WeaponMunitionEnemy weaponMunitionEnemy)
        {
            Munition munition = weaponMunitionEnemy.CurrentStage.MunitionPrefab;

            GameObject munitionGO = munition.gameObject;

            MunitionHitFX munitionHitFX = munitionGO.GetComponent<MunitionHitFX>();

            if (munitionHitFX == null)
            {
                return;
            }

            GameObject munitionHitFxGo = munitionHitFX.FxGo;
            string munitionHitFxName = munitionHitFX.FxGo.name;

            CreateAndIndexCloneAndQueue(munitionHitFxName, munitionHitFxGo, poolLimit);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the pooled FX game objects queue by cloning the object as many times as the pool limit 
        /// specifies and also creates and indexes clones to be used if this pool ever needs to expand.<br></br>
        /// Each FX game object name needs to be uniqe or otherwise any objects with the same name will be 
        /// treated as identical ones.
        /// </summary>
        /// <param name="fxName">The name which will be used as a dictionary key for the pooled game objects.</param>
        /// <param name="fxGO">The FX game object which will be cloned and a pool created for it.</param>
        /// <param name="poolLimit">How many times to clone this game object.</param>
        private void CreateAndIndexCloneAndQueue(string fxName, GameObject fxGO, int poolLimit)
        {
            if (!GoCloneByName.ContainsKey(fxName))
            {
                GameObject fxGoClone = CreateClone(fxName, fxGO);
                GoCloneByName.Add(fxName, fxGoClone);

                CreateAndIndexQueue(fxName, fxGO, poolLimit);
            }
        }

        /// <summary>
        /// Creates the pooled FX game objects queue by cloning the object as many times as the pool limit 
        /// specifies.<br></br>
        /// Each FX game object name needs to be uniqe or otherwise any objects with the same name will be 
        /// treated as identical ones.
        /// </summary>
        /// <param name="fxName">The name which will be used as a dictionary key for the pooled game objects.</param>
        /// <param name="fxGO">The FX game object which will be cloned and a pool created for it.</param>
        /// <param name="poolLimit">How many times to clone this game object.</param>
        private void CreateAndIndexQueue(string fxName, GameObject fxGO, int poolLimit)
        {
            Queue<GameObject> uniqueFXGameObjectPool = new Queue<GameObject>();

            for (int j = 0; j < poolLimit; j++)
            {
                GameObject fxClone = CreateClone(fxName, fxGO);
                uniqueFXGameObjectPool.Enqueue(fxClone);
            }

            if (!GoQueueByName.ContainsKey(fxName))
            {
                GoQueueByName.Add(fxName, uniqueFXGameObjectPool);
            }
        }

        public override void Despawn(GameObject go)
        {
            base.Despawn(go);

            // This is needed incase the FX's FX eliminator comp had the option "is parented to treadmill checked"
            go.transform.parent = PoolHierarchy;
        }
        #endregion
    }
}