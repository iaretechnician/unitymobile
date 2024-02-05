using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains methods for spawning all the different wave types and movers.
    /// </summary>
    public static class WaveDataTools
    {
        /// <summary>
        /// Spawns an enemy from a side spawnable wave by its index.
        /// </summary>
        /// <param name="sideSpawnableWaveData">The wave data to spawn an enemy from.</param>
        /// <param name="indexOfEnemyToSpawn">The index of the spawned enemy from the wave enemy prefabs array.</param>
        /// <returns>True if spawn has been successful</returns>
        public static bool SpawnSideSpawnableWaveEnemy(SideSpawnableWaveData sideSpawnableWaveData, int indexOfEnemyToSpawn)
        {
            GameObject spawnedEnemy = SpawnEnemyFromPool(sideSpawnableWaveData, indexOfEnemyToSpawn);

            if (spawnedEnemy == null)
            {
                return false;
            }

            ISideSpawnable iSideSpawnable = spawnedEnemy.GetComponent<ISideSpawnable>();

            if (iSideSpawnable == null)
            {
                Debug.Log("Spawner.cs: Was unable to access the side spawnable mover of enemy: "
                     + spawnedEnemy.name + ".  A sidespawnable wave requires an Enemy which uses a mover with" +
                    " a side spawnable interface, namely: simpleMover, AImover, magnetMoverSideSpawnable," +
                    " MissileMoversideSpawnable. Have you used a mover which doesn't use a side spawnable" +
                    " interface with a side spawnable wave?");

                return false;
            }

            RectSide spawnSide = SpawnSides.FindRectSideFromRectSideRandom(sideSpawnableWaveData.SpawnSide);
            iSideSpawnable.InitialDirection = SpawnSides.FindFourDirectionByRectSide(spawnSide);
            spawnedEnemy.transform.position = SpawnSides.FindSpawnPosition(sideSpawnableWaveData, spawnSide);

            return true;
        }

        /// <summary>
        /// Spawns an enemy from a curve wave by its index.
        /// </summary>
        /// <param name="curveWaveData">The wave data to spawn an enemy from.</param>
        /// <param name="indexOfEnemyToSpawn">The index of the spawned enemy from the wave enemy prefabs array.</param>
        /// <returns>True if spawn has been successful</returns>
        public static bool SpawnCurveWaveEnemy(CurveWaveData curveWaveData, int indexOfEnemyToSpawn)
        {
            GameObject spawnedEnemy = SpawnEnemyFromPool(curveWaveData, indexOfEnemyToSpawn);

            if (spawnedEnemy == null)
            {
                return false;
            }

            float spawnPositionZ = SpawnSides.FindSpawnZPosition(curveWaveData.DepthIndex);

            CurveMover curveMover = spawnedEnemy.GetComponent<CurveMover>();

            if (curveMover == null)
            {
                Debug.Log("Spawner.cs: Was unable to access the the curve mover of enemy: "
                     + spawnedEnemy.name + ".  A curve wave requires an Enemy which uses a curve mover. " +
                     "Have you modified the script somehow?");

                return false;
            }

            curveMover.ZPos = spawnPositionZ;

            Path path = curveMover.CurvePath;

            Vector3 firstPoint = new Vector3(path[0].x, path[0].y, spawnPositionZ);
            spawnedEnemy.transform.position = firstPoint;

            return true;
        }

        /// <summary>
        /// Spawns an enemy from a waypoint wave by its index.
        /// </summary>
        /// <param name="curveWaveData">The wave data to spawn an enemy from.</param>
        /// <param name="indexOfEnemyToSpawn">The index of the spawned enemy from the wave enemy prefabs array.</param>
        /// <returns>True if spawn has been successful</returns>
        public static bool SpawnWaypointWaveEnemy(WaypointWaveData waypointWaveData, int indexOfEnemyToSpawn)
        {
            GameObject spawnedEnemy = SpawnEnemyFromPool(waypointWaveData, indexOfEnemyToSpawn);

            if (spawnedEnemy == null)
            {
                return false;
            }

            float spawnPositionZ = SpawnSides.FindSpawnZPosition(waypointWaveData.DepthIndex);

            WaypointMover waypointMover = spawnedEnemy.GetComponent<WaypointMover>();

            if (waypointMover == null)
            {
                Debug.Log("Spawner.cs: Was unable to access the the waypoint mover of enemy: "
                     + spawnedEnemy.name + ".  A waypoint wave requires an Enemy which uses a waypoint mover. " +
                     "Have you modified the script somehow?");

                return false;
            }

            waypointMover.ZPos = spawnPositionZ;

            Waypoint2D startingWaypoint = waypointMover[0];
            Vector3 firstPoint = new Vector3(startingWaypoint.Position.x, startingWaypoint.Position.y, spawnPositionZ);
            spawnedEnemy.transform.position = firstPoint;

            return true;
        }

        public static GameObject SpawnEnemyFromPool(SideSpawnableWaveData waveData, int indexOfEnemyToSpawn)
        {
            if (waveData.IsSpawningEnemiesInRandomOrder)
            {
                indexOfEnemyToSpawn = UnityEngine.Random.Range(0, waveData.EnemyPrefabs.Length);
            }
            else if (indexOfEnemyToSpawn >= waveData.EnemyPrefabs.Length)
            {
                indexOfEnemyToSpawn = 0;
            }

            if (waveData.EnemyPrefabs[indexOfEnemyToSpawn] == null)
            {
                Debug.Log("WaveDataTools.cs: attempting to spawn an enemy from a side spanwable wave, " +
                    "but one of the enemies in the wave appears to be empty.");

                return null;
            }

            string enemyToSpawnName = waveData.EnemyPrefabs[indexOfEnemyToSpawn].name;
            GameObject spawnedEnemy = EnemyPool.Instance.Spawn(enemyToSpawnName);

            if (spawnedEnemy == null)
            {
                Debug.Log("WaveDataTools.cs: was unable to spawn enemy: " + enemyToSpawnName
                     + "in side spawnable wave because it was not possible to retrieve it from the pool.");

                return null;
            }

            return spawnedEnemy;
        }

        public static GameObject SpawnEnemyFromPool(CurveWaveData waveData, int indexOfEnemyToSpawn)
        {
            if (waveData.IsSpawningEnemiesInRandomOrder)
            {
                indexOfEnemyToSpawn = UnityEngine.Random.Range(0, waveData.EnemyPrefabs.Length);
            }
            else if (indexOfEnemyToSpawn >= waveData.EnemyPrefabs.Length)
            {
                indexOfEnemyToSpawn = 0;
            }

            if (waveData.EnemyPrefabs[indexOfEnemyToSpawn] == null)
            {
                Debug.Log("WaveDataTools.cs: attempting to spawn an enemy from a curve wave, " +
                    "but one of the enemies in the curve wave appears to be empty.");

                return null;
            }

            string enemyToSpawnName = waveData.EnemyPrefabs[indexOfEnemyToSpawn].name;
            GameObject spawnedEnemy = EnemyPool.Instance.Spawn(enemyToSpawnName);

            if (spawnedEnemy == null)
            {
                Debug.Log("WaveDataTools.cs: was unable to spawn enemy: " + enemyToSpawnName
                     + ". in curve wave because it was not possible to retrieve it from the pool.");

                return null;
            }

            return spawnedEnemy;
        }

        public static GameObject SpawnEnemyFromPool(WaypointWaveData waveData, int indexOfEnemyToSpawn)
        {
            if (waveData.IsSpawningEnemiesInRandomOrder)
            {
                indexOfEnemyToSpawn = UnityEngine.Random.Range(0, waveData.EnemyPrefabs.Length);
            }
            else if (indexOfEnemyToSpawn >= waveData.EnemyPrefabs.Length)
            {
                indexOfEnemyToSpawn = 0;
            }

            if (waveData.EnemyPrefabs[indexOfEnemyToSpawn] == null)
            {
                Debug.Log("WaveDataTools.cs: attempting to spawn an enemy from waypoint wave, " +
                    "but one of the enemies in the waypoint wave appears to be empty.");

                return null;
            }

            string enemyToSpawnName = waveData.EnemyPrefabs[indexOfEnemyToSpawn].name;
            GameObject spawnedEnemy = EnemyPool.Instance.Spawn(enemyToSpawnName);

            if (spawnedEnemy == null)
            {
                Debug.Log("WaveDataTools.cs: was unable to spawn enemy: " + enemyToSpawnName
                     + "in waypoint wave because it was not possible to retrieve it from the pool.");

                return null;
            }

            return spawnedEnemy;
        }
    }
}