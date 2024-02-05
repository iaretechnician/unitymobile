using System.Collections.Generic;

namespace ShmupBoss
{
    // Note1: This class is based on extension methods and allows for any class to be added to it, 
    // categorized and a list of its active game objects stored. Currently it is only used with the 
    // Enemy class and it could have simply been added to the enemy class instead of it now being in this 
    // separate static class but doing it this way would be good for someone new to coding to learn about
    // extension methods, and you will also have the option to make more lists if you ever needed to.
    
    // Note2: Whenever a word pool is mentioned in this class, it does not refer to the pool of game objects
    // which are spawned/despawned in a level. Instead it refers to instances which have beeen added to
    // THIS pool using the add/remove from pool to be categorized in the list dictionary.

    /// <summary>
    /// Stores a list of in-game active game objects categorized based on class type.
    /// </summary>
    public static class InGamePoolManager
    {
        /// <summary>
        /// Holds a list of the instances which implement the "IInGamePool" interface and are 
        /// categorized by their class type.
        /// </summary>
        private static readonly Dictionary<System.Type, List<IInGamePool>> IInGamePoolListByType;

        static InGamePoolManager()
        {
            IInGamePoolListByType = new Dictionary<System.Type, List<IInGamePool>>();
        }

        /// <summary>
        /// Clears the dictionary which stores in game active objects (implementing IInGamePool interface) 
        /// which are caregorized by class type.
        /// </summary>
        public static void ClearIndex()
        {
            IInGamePoolListByType.Clear();
        }

        public static bool WasListCreated<T>()
        {
            System.Type poolType = typeof(T);

            if (IInGamePoolListByType.ContainsKey(poolType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds if a system type contains any items in its IInGamePool list.
        /// </summary>
        /// <typeparam name="T">The system type you want to search.</typeparam>
        /// <returns>True if a list was created and contains itesm.</returns>
        public static bool DoesContainItems<T>()
        {
            System.Type poolType = typeof(T);

            if (!WasListCreated<T>())
            {
                return false;
            }

            if (IInGamePoolListByType[poolType].Count <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extracts the list of instances which have been added to the pool from the 
        /// dictionary using the class type as key.
        /// </summary>
        /// <typeparam name="T">The class type you wish to extract its instnances list.</typeparam>
        /// <returns>The list of the instances of the class which have been added to the pool.</returns>
        public static List<IInGamePool> GetList<T>()
        {
            if (IInGamePoolListByType.ContainsKey(typeof(T)))
            {
                return IInGamePoolListByType[typeof(T)];
            }
            else
            {
                return null;
            }               
        }

        /// <summary>
        /// Adds a class instance (pool object) to its categorized dictionary list ("IInGamePoolListByType".)
        /// </summary>
        /// <typeparam name="T">The class type you wish to add an instance of.</typeparam>
        /// <param name="poolObject">The instance of the class you wish to add to its dictionary list.</param>
        public static void AddToPool<T>(this IInGamePool poolObject)
        {
            System.Type poolType = typeof(T);

            if (!IInGamePoolListByType.ContainsKey(poolType))
            {
                IInGamePoolListByType.Add(poolType, new List<IInGamePool>());
            }
                
            IInGamePoolListByType[poolType].Add(poolObject);
        }

        /// <summary>
        /// Removes a class instance (pool object) from its categorized dictionary list ("IInGamePoolListByType".)
        /// </summary>
        /// <typeparam name="T">The class type you wish to remove an instance of.</typeparam>
        /// <param name="poolObject">The instance of the class you wish to remove from its dictionary list.</param>
        /// <returns>True if it's the last object to be removed from the pool.</returns>
        public static bool RemoveFromPool<T>(this IInGamePool poolObject)
        {
            System.Type poolType = typeof(T);

            if (!IInGamePoolListByType.ContainsKey(poolType))
            {
                return true;
            }
            
            List<IInGamePool> poolObjectList = IInGamePoolListByType[poolType];

            if (poolObjectList.Count == 1)
            {
                poolObjectList.Clear();              
                return true;
            }

            poolObjectList.Remove(poolObject);

            return false;
        }
    }
}