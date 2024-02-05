namespace ShmupBoss
{
    /// <summary>
    /// Interface applied to anything which will edit a rotation method, this is 
    /// applied typically to movers which are rotated on a single axis.
    /// </summary>
    public interface IRotatable
    {
        float EditRotation(float angle);
    }
}