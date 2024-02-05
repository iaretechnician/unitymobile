using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Holds tools related to finding the spawn position, this is used with the different objects 
    /// that are spawned and need to find their initial position in relation to the play field.
    /// </summary>
    public static class SpawnSides
    {
        public static Vector3 FindSpawnPosition(IRandomPositionSpawnable iRandomPositionSpawnable, RectSide spawnSide)
        {
            float spawnPositionRatio = FindSpawnPositionRatio(iRandomPositionSpawnable.IsRandomSpawnPosition, 
                iRandomPositionSpawnable.MinRandomSpawnPosition,
                iRandomPositionSpawnable.MaxRandomSpawnPosition, 
                iRandomPositionSpawnable.SpawnPosition);

            return FindSpawnPosition(iRandomPositionSpawnable.DepthIndex,
                iRandomPositionSpawnable.SpawnSideOffset,
                spawnSide,
                spawnPositionRatio);
        }
        
        public static float FindSpawnPositionRatio(bool isRandomSpawnPosition, 
            float minRandomSpawnPosition, 
            float maxRandomSpawnPosition, 
            float spawnPosition)
        {
            if (isRandomSpawnPosition)
            {
                return UnityEngine.Random.Range(minRandomSpawnPosition, maxRandomSpawnPosition);
            }
            else
            {
                return spawnPosition;
            }
        }

        public static Vector3 FindSpawnPosition(int depthIndex, 
            float offset, 
            RectSide spawnSide, 
            float spawnPositionRatio)
        {
            Vector3 spawnPosition = Vector3.zero;
            spawnPosition.z = FindSpawnZPosition(depthIndex);

            switch (spawnSide)
            {
                case RectSide.Top:
                    spawnPosition.x = Mathf.Lerp(PlayField.Instance.Boundries.xMin, PlayField.Instance.Boundries.xMax, spawnPositionRatio);
                    spawnPosition.y = PlayField.Instance.Boundries.yMax + offset;
                    break;

                case RectSide.Bottom:
                    spawnPosition.x = Mathf.Lerp(PlayField.Instance.Boundries.xMin, PlayField.Instance.Boundries.xMax, spawnPositionRatio);
                    spawnPosition.y = PlayField.Instance.Boundries.yMin - offset;
                    break;

                case RectSide.Left:
                    spawnPosition.x = PlayField.Instance.Boundries.xMin - offset;
                    spawnPosition.y = Mathf.Lerp(PlayField.Instance.Boundries.yMin, PlayField.Instance.Boundries.yMax, spawnPositionRatio);
                    break;

                case RectSide.Right:
                    spawnPosition.x = PlayField.Instance.Boundries.xMax + offset;
                    spawnPosition.y = Mathf.Lerp(PlayField.Instance.Boundries.yMin, PlayField.Instance.Boundries.yMax, spawnPositionRatio);
                    break;
            }

            return spawnPosition;
        }

        /// <summary>
        /// Finds the position on the Z axis by using the depth index value.
        /// </summary>
        /// <param name="depthIndex">The depth index on the Z axis.</param>
        /// <returns>The Z position value.</returns>
        public static float FindSpawnZPosition(int depthIndex)
        {
            float spawnPositionZ = 0.0f;
            
            if (Level.Is2D)
            {
                spawnPositionZ = -depthIndex * Level.SpaceBetweenIndices;
            }

            return spawnPositionZ;
        }

        /// <summary>
        /// Translates the random rect side enum into a rect side and if a random option is 
        /// choosen it picks a random rect side.
        /// </summary>
        /// <param name="spawnSide">The random rect side option you wish to translate.</param>
        /// <returns>The translated rect side.</returns>
        public static RectSide FindRectSideFromRectSideRandom(RectSideRandom spawnSide)
        {
            switch (spawnSide)
            {
                case RectSideRandom.Top:
                    return RectSide.Top;

                case RectSideRandom.Bottom:
                    return RectSide.Bottom;

                case RectSideRandom.Left:
                    return RectSide.Left;

                case RectSideRandom.Right:
                    return RectSide.Right;

                default:
                    return (RectSide)Random.Range(0, 4);
            }
        }

        /// <summary>
        /// Finds the four direction (movement direction) when given a rect side. This is helpful when you spawn 
        /// something from a side to be able to assign the direction it should go automatically.<br></br> 
        /// (i.e. something spawned from the top will need to have its direction down, otherwise you will never 
        /// see it in the playfield and there is no point in you assigning it manually.)
        /// </summary>
        /// <param name="rectSide">The spawn side.</param>
        /// <returns>The movement direction that is compatible with that spawn side.</returns>
        public static FourDirection FindFourDirectionByRectSide(RectSide rectSide)
        {
            switch (rectSide)
            {
                case RectSide.Top:
                    return FourDirection.Down;

                case RectSide.Bottom:
                    return FourDirection.Up;

                case RectSide.Left:
                    return FourDirection.Right;

                case RectSide.Right:
                    return FourDirection.Left;
            }

            return FourDirection.None;
        }
    }
}