using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains tools related to the modifications of names used in this pack.
    /// </summary>
    public static class StringTools
    {
        /// <summary>
        /// Typically used in naming layers, this will add the number to the name, while making sure 
        /// its a 2 digit number. This uniform way of naming allows for easy retrievals.
        /// </summary>
        /// <param name="name">The name you wish to add a number to.</param>
        /// <param name="number">The number you want to add to the name, please make sure that the 
        /// number is no more than 99.</param>
        /// <returns></returns>
        public static string CombineNameAndNumber(string name, int number)
        {
            if(number < 10)
            {
                return (name + "0" + number);
            }
            else if(number < 100)
            {
                return (name + number);
            }
            else
            {
                Debug.Log("StringTools.cs: You have combined a name and a number (typically a layer)" +
                    "bigger than 99, only a maximum of 99 is allowed.");

                return (name + "99");
            }
        }

        /// <summary>
        /// Removes the layer prefix of a name.<br></br>
        /// Certain objects will have the layer name prefix added to thier names. This method will remove this prefix.
        /// </summary>
        /// <param name="name">The object name which contains a layer name as a prefix.</param>
        /// <returns>The object name minus the layer prefix.</returns>
        public static string GetLayerName(string name)
        {
            return name.Remove(ProjectConstants.LayerPrefixLength);
        }

        /// <summary>
        /// When duplicating objects in the hierarchy or scene view, a number in parentheses will be added to their names. 
        /// This method detects if the last character in an object name is a parentheses and if it is, it will remove the 
        /// parentheses to return the name to the orignal name so that it can be pooled with the same name as the original 
        /// object before duplicating.
        /// </summary>
        /// <param name="go">The game object which you want to check its name.</param>
        public static void RemoveParenthesesAfterLastSpace(GameObject go)
        {
            string childName = go.name;
            int childNameLength = childName.Length;
            char lastChar = childName[childNameLength - 1];

            if (lastChar != ')')
            {
                return;
            }
            else
            {
                for (int j = childNameLength - 1; j >= 0; j--)
                {
                    if (childName[j] == ' ')
                    {
                        go.name = childName.Remove(j);
                        j = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a class name.
        /// </summary>
        /// <param name="type">The class you want to find its name.</param>
        /// <returns>the name of the class minus any namespace.</returns>
        public static string FindClassName(System.Type type)
        {
            string className = type.ToString();
            string namespaceName = type.Namespace.ToString();
            int lengthOfNamespace = namespaceName.Length;
            string classNameMinusNamespace = className.Substring(lengthOfNamespace + 1);

            return classNameMinusNamespace;
        }
    }
}