using UnityEngine;

namespace ShmupBoss
{
    // Note: Technically speaking this singleton pattern might not be classified as a true singleton because
    // it doesn't destroy any other versions, but I had to go in this form to avoid some issues with the static 
    // variable when Unity reloads a level.

    /// <summary>
    /// The singleton pattern is a pattern that gives access to the class and works if only one version 
    /// exists in the scene.<br></br> 
    /// When you override the awake method of this class, make sure that you always implement the base 
    /// version of the awake method.<br></br>
    /// </summary>
    /// <typeparam name="T">The class that is inherting from singleton</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool IsInitialized
        {
            get
            {
                if (instance == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private static T instance;

        public static T Instance
        {
            get
            {
                if(instance != null)
                {
                    return instance;
                }

                instance = FindObjectOfType<T>();

                if(instance != null)
                {
                    return instance;
                }

                string className = StringTools.FindClassName(typeof(T));

                GameObject go = new GameObject("----- " + className + " -----");
                instance = go.AddComponent<T>();
                return instance;
            }
        }

        /// <summary>
        /// Saves the instance variable.
        /// </summary>
        protected virtual void Awake()
        {          
            instance = this as T;
        }
    }
}