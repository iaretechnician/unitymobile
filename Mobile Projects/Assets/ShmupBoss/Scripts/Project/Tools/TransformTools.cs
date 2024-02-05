using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains tools related to adjusting the transform of game objects.<br></br>
    /// (Currently only the reset parent position is included in this class.)
    /// </summary>
    public static class TransformTools
    {
        /// <summary>
        /// Resets the position of a parent hierarchy to zero without affecting the position of any 
        /// of the nested children.
        /// </summary>
        /// <param name="hierarchy">The parent transform you want to reset.</param>
        public static void ResetParentPosition(Transform hierarchy)
        {
            int childCount = hierarchy.childCount;
            Transform[] children = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = hierarchy.GetChild(i);
            }

            foreach (Transform child in children)
            {
                child.transform.parent = null;
            }

            hierarchy.position = Vector3.zero;

            foreach (Transform child in children)
            {
                child.transform.parent = hierarchy;
            }
        }
    }
}