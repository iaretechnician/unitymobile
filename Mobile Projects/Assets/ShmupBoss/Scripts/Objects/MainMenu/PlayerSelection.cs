namespace ShmupBoss
{
    /// <summary>
    /// Class for holding the player prefabs for both the vertical and horizontal 
    /// levels in case you are building a game which has both types of levels.
    /// </summary>
    [System.Serializable]
    public class PlayerSelection
    {
        public UnityEngine.Object Vertical;
        public UnityEngine.Object Horizontal;
    }
}