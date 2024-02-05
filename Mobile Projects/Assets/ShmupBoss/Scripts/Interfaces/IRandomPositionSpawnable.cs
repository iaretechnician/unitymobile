namespace ShmupBoss
{
    /// <summary>
    /// Determines how an object is spawned from a random side and position ratio in relation to the play field.
    /// </summary>
    public interface IRandomPositionSpawnable
    {
        int DepthIndex
        {
            get;
        }

        RectSideRandom SpawnSide
        {
            get;
        }

        float SpawnSideOffset
        {
            get;
        }

        float SpawnPosition
        {
            get;
        }

        bool IsRandomSpawnPosition
        {
            get;
        }

        float MinRandomSpawnPosition
        {
            get;
        }

        float MaxRandomSpawnPosition
        {
            get;
        }
    }
}