using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A property attribute class to show a string message in a box inside the inspector
    /// This attribute is drawn using the HlepBoxAttributeDrawer class.
    /// </summary>
    public class HelpBox : PropertyAttribute
    {
        public string Message;

        public HelpBox(string message)
        {
            Message = message;
        }
    }
}