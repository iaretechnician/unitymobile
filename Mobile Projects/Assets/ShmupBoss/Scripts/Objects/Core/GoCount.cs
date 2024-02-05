using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A game object with the repetition count value added to it,
    /// typically used to know how many times to repeat or pool a game object.
    /// </summary>
    [System.Serializable]
    public class GoCount
    {
        /// <summary>
        /// The game object prefab that will be used repeatedly inside the level.
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// How many times is the game object repeated.
        /// </summary>
        public int Count;

        public GoCount()
        {
            Prefab = null;
            Count = 0;
        }

        public GoCount(GameObject go)
        {
            Prefab = go;
            Count = 1;
        }

        public GoCount(GameObject go, int count)
        {
            Prefab = go;
            Count = count;
        }
    }
}