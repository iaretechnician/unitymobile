namespace ShmupBossEditor
{
    /// <summary>
    /// Class for holding a UnityEngine Object, used for inspector serialization.
    /// </summary>
    [System.Serializable]
    public class ObjectContainer
    {
        public UnityEngine.Object ContainedObject;

        public ObjectContainer(UnityEngine.Object newObject)
        {
            ContainedObject = newObject;
        }
    }
}