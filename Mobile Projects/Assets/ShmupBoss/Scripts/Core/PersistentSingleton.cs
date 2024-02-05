using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Persistent Singleton class is very similar to the singleton class in terms of being a class that 
    /// provides access and only allows one of its kind, but the difference is that it implements 
    /// the "do not destroy on load function" which means this class will not be destroyed when you access
    /// a new scene and will instead live on, this is useful in the case of example of the game manager
    /// where you want the same class to stay persistent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PersistentSingleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T instance;

		public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = FindObjectOfType<T>();

                if (instance != null)
                {
                    return instance;
                }

                GameObject go = new GameObject("----- " + typeof(T).ToString() + " -----");
                instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);

                return instance;
            }
        }

        public static bool IsInitialized
        {
            get 
            { 
                return instance != null; 
            }
        }

        /// <summary>
        /// Saves the instance variable, makes sure no other classes of the same kind exist in 
        /// the scene and implements the do not destroy on load function to make that object 
        /// live inside other scenes when accessed.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            else
            {
                instance = this as T;
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}