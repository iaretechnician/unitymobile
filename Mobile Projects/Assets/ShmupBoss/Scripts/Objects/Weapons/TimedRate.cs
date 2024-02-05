using UnityEngine;

/// <summary>
/// Rate multplier value and its associated duration.
/// </summary>
[System.Serializable]
public class TimedRate
{
    [Tooltip("For how long will this stage last.")]
    [SerializeField]
    private float duration;
    public float Duration
    {
        get
        {
            return duration;
        }
    }

    [Tooltip("The value the weapon’s rate will be multiplied with.")]
    [SerializeField]
    private float rateMultplier;
    public float RateMultplier
    {
        get
        {
            return rateMultplier;
        }
    }
}
