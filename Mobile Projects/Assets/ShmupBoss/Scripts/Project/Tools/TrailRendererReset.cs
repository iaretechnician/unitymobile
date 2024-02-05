using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Will reset a trail renderer upon disable, because everything in Shmup Boss is used and reused 
    /// for the most optimal performance, it can happen if you are using a trail renderer with a 
    /// munition for example; that when it is reused, its trail picks up the location from its last 
    /// location and cause what may seem as a rendering glitch. This script will clear the trail renderer 
    /// so it can be reused from a clean slate. 
    /// </summary>
    public class TrailRendererReset : MonoBehaviour
    {
        private TrailRenderer tr;
        
        private void Awake()
        {
            tr = GetComponent<TrailRenderer>();
        }

        private void OnDisable()
        {
            tr.Clear();
        }
    }
}