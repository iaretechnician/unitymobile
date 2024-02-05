using UnityEngine;

/// <summary>
/// Basic implementation of a proportional-integral-derivative controller.
/// </summary>
public class PIDController
{
    public float pFactor, iFactor, dFactor;

    private Vector3 integral;
    private Vector3 lastError;

    public PIDController(float pFactor, float iFactor, float dFactor)
    {
        this.pFactor = pFactor;
        this.iFactor = iFactor;
        this.dFactor = dFactor;
    }

    public Vector3 Update(Vector3 currentError, float timeFrame)
    {
        integral += currentError * timeFrame;
        var deriv = (currentError - lastError) / timeFrame;
        lastError = currentError;
        return currentError * pFactor
            + integral * iFactor
            + deriv * dFactor;
    }
}