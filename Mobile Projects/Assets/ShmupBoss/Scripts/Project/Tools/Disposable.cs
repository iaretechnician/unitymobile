using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A very simple class that will destroy the game object using it on awake.<br></br>
    /// This is typically used when you want to edit a prefab inside the scene but do not wish it 
    /// to be active when the level starts.
    /// </summary>
    public class Disposable : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}