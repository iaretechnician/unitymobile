using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This was added to find the random spawn side but later deprecated by another method inside SpawnSides.cs.<br></br>
    /// It was kept just in case it was needed for anything later on.
    /// </summary>
    public static class EnumTools
    {
        /// <summary>
        /// Returns a random enum value in a list of enums.
        /// </summary>
        /// <typeparam name="T">The enum name.</typeparam>
        /// <returns>the picked random enum value.</returns>
        static T GetRandomEnum<T>()
        {
            System.Array enumValuesArray = System.Enum.GetValues(typeof(T));
            T EnumValue = (T)enumValuesArray.GetValue(Random.Range(0, enumValuesArray.Length));
            return EnumValue;
        }
    }
}