using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the player and enemies munition pools. This class itself inherits from PoolBase.<br></br>
    /// Both the player and enemy munition pools are nearly identical with the exception of how to find and list 
    /// the weapons.
    /// </summary>
    /// <typeparam name="T">Ther player or enemy munition pool class types.</typeparam>
    public abstract class MunitionPool<T> : PoolBase where T : MonoBehaviour
    {
        public static T Instance;
        
        /// <summary>
        /// Categorizes all munition by their name to be able to find their damage value when hitting an 
        /// opposing faction without needed to get any component.
        /// </summary>
        public Dictionary<string, float> MunitionDamageByName = new Dictionary<string, float>();

        /// <summary>
        /// All munition weapons which will be pooled.
        /// </summary>
        protected List<WeaponMunition> WeaponsMunition = new List<WeaponMunition>();

        protected abstract bool canListWeapons
        {
            get;
        }

        protected virtual void Awake()
        {
            Instance = this as T;
        }

        protected override void Pool()
        {
            if (!canListWeapons)
            {
                return;
            }

            CreateHierarchy();
            ListWeapons();
            GoThroughWeaponsLists();
        }

        protected abstract void ListWeapons();

        /// <summary>
        /// Pools munition used by the weapons which have been already listed.
        /// </summary>
        protected abstract void GoThroughWeaponsLists();

        /// <summary>
        /// Creates munition game object queues (The munition pool), indexes their damage values and 
        /// creates and indexes clones if the pool ever needs to expand.
        /// </summary>
        /// <param name="munition">The munition which will be pooled.</param>
        protected void CreateAndIndexMunition(Munition munition)
        {
            GameObject munitionPrefabGO = munition.gameObject;
            string munitionName = munition.name;
            int poolLimit = munition.PoolLimit;
            float munitionDamage = munition.Damage;

            if (!GoCloneByName.ContainsKey(munitionName))
            {
                CreateAndIndexClone(munitionName, munitionPrefabGO);
                CreateAndIndexGoQueue(munitionName, munitionPrefabGO, poolLimit);
                IndexDamage(munitionName, munitionDamage);
            }
        }

        /// <summary>
        /// Creates a clone and categorizes it in the GoCloneByName dictionary to be retrieved 
        /// if the pool ever needs to expand. 
        /// </summary>
        /// <param name="munitionName">This name will be used an index key in the clones dictionary.</param>
        /// <param name="munitionPrefabGO">The game object of the munition which will be pooled.</param>
        private void CreateAndIndexClone(string munitionName, GameObject munitionPrefabGO)
        {
            GameObject munitionClone = CreateClone(munitionName, munitionPrefabGO);
            GoCloneByName.Add(munitionName, munitionClone);
        }

        /// <summary>
        /// Will pool munition game objects by creating them and indexing them in a dictionary of 
        /// game objects. Each munition will have its own queue of game objects which can be used 
        /// to spawn/despawn munitions.<br></br>
        /// Note: if two munition have the exact same name, they will be treated as the same, please 
        /// use different names for different munition.
        /// </summary>
        /// <param name="munitionName">This name will be used as a dictionary key</param>
        /// <param name="munitionPrefabGO">The game object of the munition which will be pooled.</param>
        /// <param name="poolLimit">How many instances of the munition will be pooled.</param>
        private void CreateAndIndexGoQueue(string munitionName, GameObject munitionPrefabGO, int poolLimit)
        {
            Queue<GameObject> munitionPrefabQueue = new Queue<GameObject>();

            for (int i = 0; i < poolLimit; i++)
            {
                GameObject munitionToPool = CreateClone(munitionName, munitionPrefabGO);
                munitionPrefabQueue.Enqueue(munitionToPool);
            }

            if (!GoQueueByName.ContainsKey(munitionName))
            {
                GoQueueByName.Add(munitionName, munitionPrefabQueue);
            }
        }

        private void IndexDamage(string munitionName, float munitionDamage)
        {
            if (!MunitionDamageByName.ContainsKey(munitionName))
            {
                MunitionDamageByName.Add(munitionName, munitionDamage);
            }
        }

        /// <summary>
        /// Returns the value of the damage caused by the munition.
        /// </summary>
        /// <param name="munitionName">The name of the munition which you wish to retrieve its damage.</param>
        /// <returns>The damage value of the munition with the name you input.</returns>
        public virtual float GetDamage(string munitionName)
        {
            try
            {
                float damage = MunitionDamageByName[munitionName];
                return damage;
            }
            catch
            {
                Debug.Log("MunitionPool.cs: Unable to find munition damage key.");

                return 0;
            }           
        }
    }
}