using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Saves the current input the user has made
    /// </summary>
    public static class PlayerInput
    {
        public static Vector2 Direction;
        public static bool IsAutoFireEnabled;
        public static bool IsFire1Pressed;
        public static bool IsFire2Pressed;
        public static bool IsSubmitPressed;
        public static bool IsCancelPressed;
    }
}