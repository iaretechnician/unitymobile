using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the different pool managers in this pack, this includes: enemy pool, FX pool, 
    /// pickup pool, punition pools and backgorund related pools.<br></br>
    /// This base makes sure that every pool manager has a queue of game objects catagorized by name 
    /// which is spawned/despawned as needed and a dictionary of clones to duplicate if needed to 
    /// expand any pool.<br></br>
    /// Please note that all pools are categorized by name which is not ideal, because it means 
    /// that if two objects in the same pool has the same name; they will be treated as the same 
    /// object. Unique names must always be used for the same pool.
    /// </summary>
    public abstract class PoolBase : MonoBehaviour
    {
        /// <summary>
        /// This dictionary holds queues of the pooled game objects. 
        /// Use it to pull/push game objects from queues to spawn/despawn game objects.<br></br>
        /// This dictionary is indexed by name, please only use unique names for game objects.
        /// </summary>
        protected Dictionary<string, Queue<GameObject>> GoQueueByName = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// The clones that will themselves be cloned if needed to expand the pool.
        /// </summary>
        protected Dictionary<string, GameObject> GoCloneByName = new Dictionary<string, GameObject>();

        protected Transform PoolHierarchy;

        /// <summary>
        /// The layer which pooled objects will be pooled under.
        /// </summary>
        protected abstract int Layer
        {
            get;
        }

        protected abstract void CreateHierarchy();
        protected abstract void Pool();

        public virtual GameObject Spawn(string name)
        {
            Queue<GameObject> goQueue;

            try
            {
                goQueue = GoQueueByName[name];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("PoolBase.cs: The game object: " + name +
                    " that you are trying to spawn can't find it's GoQueue dictionary key.");

                return null;
            }

            if (goQueue.Count == 0)
            {
                ExpandPool(name);
            }

            GameObject go = goQueue.Dequeue();

            go.SetActive(true);

            return go;
        }

        protected virtual void ExpandPool(string name)
        {
            GameObject originalClone = FindGoClone(name);

            if (originalClone == null)
            {
                Debug.Log("PoolBase.cs: A pool needed to expand and was unable to find" +
                    " the original clone to clone it.");
 
                return;
            }

            GameObject newClone = CreateClone(name, originalClone);

            try
            {
                GoQueueByName[name].Enqueue(newClone);
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("PoolBase.cs: The game object: " + name +
                    " that you are trying to expand its pool can't find its GoQueue dictionary key.");
            }

            return;
        }

        protected virtual GameObject FindGoClone(string name)
        {
            try
            {
                return GoCloneByName[name];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("PoolBase.cs: The game object clone: " + name +
                    " that you are trying to duplicate can't find its GoClone dictionary key.");

                return null;
            }
        }

        /// <summary>
        /// Creates a game object clone that will be used in case the pool needs to expand.
        /// </summary>
        /// <param name="name">The name of the object to clone.</param>
        /// <param name="go">The game object which will be cloned.</param>
        /// <returns>The cloned game object.</returns>
        protected virtual GameObject CreateClone(string name, GameObject go)
        {
            GameObject goClone = Instantiate(go);
            goClone.name = name;
            goClone.layer = Layer;
            goClone.transform.parent = PoolHierarchy;
            goClone.SetActive(false);
            return goClone;
        }

        public virtual void Despawn(GameObject go)
        {
            go.SetActive(false);

            if(go.layer != Layer)
            {
                Debug.Log("Attempting to despawn object with layer: " + go.layer + "and the layer of this pool is: " + Layer + 
                    "PoolBase.cs: Couldn't despawn an object, there must be either: \n" +
                    "-Some layer mix up. " +
                    "- Or Maybe you are trying to despawn a game object from the wrong pool. " +
                    "- Or a preview object might have been nested under the prefab and needs to be deleted manually. " + 
                    "- Or You added an FX Eliminator to an object and you chose the despawn option while it " +
                    "hasn't been spawned from an FX spawner in the first place?");

                return;
            }

            try
            {
                GoQueueByName[go.name].Enqueue(go);
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("PoolBase.cs: The game object: " + go.name +
                    " that you are trying to despawn can't find its Queue dictionary key. " +
                    "This can happen in one of those cases: \n" +
                    "-An enemy is not spawned from a spawner and is already active in the scene. " +
                    "-Ground units are not properly heirarchied under the enemy pool. " +
                    "-You added an FX Eliminator to an object and chose the despawn option while it " +
                    "hasn't been spawned from an FX spawner in the first place?");
            }
        }
    }
}