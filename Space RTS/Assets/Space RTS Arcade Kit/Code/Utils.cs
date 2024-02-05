﻿using UnityEngine;

public class Utils {

    #region data parsers
    public static Vector3 ParseVector3(string vector)
    {
        string[] sArray = vector.Split(' ');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static Vector2 ParseVector2(string vector)
    {
        // split the items
        string[] sArray = vector.Split(' ');

        // store as a Vector2
        Vector2 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

    public static Color ParseColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    public static string ColorToString(Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }

    public static Quaternion ParseRotation(string euler)
    {
        // split the items
        string[] sArray = euler.Split(' ');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return Quaternion.Euler(result);
    }
    #endregion data parsers
}
