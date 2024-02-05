using UnityEngine;

/// <summary>
/// Holds multiple direction selections to pick from randomly.
/// </summary>
public static class RamdomDirections
{
    /// <summary>
    /// The direction that the mover will pick from when it's going to the right.
    /// </summary>
    public static readonly Vector2[] Rightward = new Vector2[]
    {
            new Vector2(0.5f,0.5f),
            new Vector2(1f, 0),
            new Vector2(0.5f, -0.5f)
    };

    /// <summary>
    /// The direction that the mover will pick from when it's going to the left.
    /// </summary>
    public static readonly Vector2[] Leftward = new Vector2[]
    {
            new Vector2(-0.5f, 0.5f),
            new Vector2(-1f, 0),
            new Vector2(-0.5f, -0.5f)
    };

    /// <summary>
    /// The direction that the mover will pick from when it's going up.
    /// </summary>
    public static readonly Vector2[] Upward = new Vector2[]
    {
            new Vector2(0.5f, 0.5f),
            new Vector2(0, 1f),
            new Vector2(-0.5f, 0.5f)
    };

    /// <summary>
    /// The direction that the mover will pick from when it's going down.
    /// </summary>
    public static readonly Vector2[] Downward = new Vector2[]
    {
            new Vector2(0.5f, -0.5f),
            new Vector2(0, -1f),
            new Vector2(-0.5f, -0.5f)
    };
}
