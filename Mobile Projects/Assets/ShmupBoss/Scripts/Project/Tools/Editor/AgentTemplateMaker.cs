using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    public class AgentTemplateMaker : ScriptableObject
    {
        [MenuItem("Edit/Shmup Boss/Agents/Create Player Template", false, 1011)]
        private static void CreatePlayerTemplate()
        {
            GameObject playerGO = new GameObject(
                "Player",
                typeof(Rigidbody2D),
                typeof(CircleCollider2D),
                typeof(Player), 
                typeof(PlayerMover), 
                typeof(PlayerShooter), 
                typeof(SfxPlayer), 
                typeof(FxSpawnerPlayer));

            playerGO.layer = ProjectLayers.Player;

            Rigidbody2D rb2D = playerGO.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;
            rb2D.useFullKinematicContacts = true;
            rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            GameObject visualsHierarchy = new GameObject("Visuals");
            visualsHierarchy.transform.parent = playerGO.transform;

            GameObject weaponsHierarchy = new GameObject("Weapons");
            weaponsHierarchy.transform.parent = playerGO.transform;

            Undo.RegisterCreatedObjectUndo(playerGO, "Create Basic Player Template");
        }


        private static void CreateEnemyTemplate<T,U>(string name) 
            where T: Enemy 
            where U: Mover
        {
            GameObject enemyGO = new GameObject(
                name,
                typeof(Rigidbody2D),
                typeof(CircleCollider2D),
                typeof(T),
                typeof(U),
                typeof(EnemyShooter),
                typeof(SfxEnemy),
                typeof(FxSpawnerEnemy));

            enemyGO.layer = ProjectLayers.Enemies;

            Rigidbody2D rb2D = enemyGO.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;

            GameObject visualsHierarchy = new GameObject("Visuals");
            visualsHierarchy.transform.parent = enemyGO.transform;

            GameObject weaponsHierarchy = new GameObject("Weapons");
            weaponsHierarchy.transform.parent = enemyGO.transform;

            Debug.Log("Please make sure to use unique names for the prefabs you use in a level. Enemy " +
                "prefabs with identical names will be treated and pooled as if they were the same enemy.");

            Undo.RegisterCreatedObjectUndo(enemyGO, "Create Enemy Template");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Simple Mover Enemy Template", false, 1012)]
        private static void CreateSimpleMoverEnemyTemplate()
        {
            CreateEnemyTemplate<Enemy, SimpleMover>("Simple");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Ground Enemy Template", false, 1013)]
        private static void CreateGroundEnemyTemplate()
        {
            CreateEnemyTemplate<Enemy, DirectionalMover>("SimpleGround");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create AI Mover Enemy Template", false, 1014)]
        private static void CreateAIMoverEnemyTemplate()
        {
            CreateEnemyTemplate<Enemy, AIMover>("AI");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Enemy Mine Template", false, 1015)]
        private static void CreateMagnetMoverEnemyTemplate()
        {
            CreateEnemyTemplate<EnemyDetonator, MagnetMoverSideSpawnable>("Mine");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Waypoint Mover Enemy Template", false, 1016)]
        private static void CreateWaypointMoverEnemyTemplate()
        {
            CreateEnemyTemplate<Enemy, WaypointMover>("Waypoint");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Curve Mover Enemy Template", false, 1017)]
        private static void CreateCurveMoverEnemyTemplate()
        {
            CreateEnemyTemplate<Enemy, CurveMover>("Curve");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Boss Template", false, 1018)]
        private static void CreateBossTemplate()
        {
            GameObject bossGO = new GameObject(
                "boss",
                typeof(Boss),
                typeof(WaypointMover));

            GameObject bossSubPhase = new GameObject(
                "BossSubPhase1", 
                typeof(Rigidbody2D),
                typeof(CircleCollider2D),
                typeof(BossSubPhase),
                typeof(EnemyShooter),
                typeof(SfxEnemy),
                typeof(FxSpawnerEnemy));

            Rigidbody2D rb2D = bossSubPhase.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;

            GameObject visualsHierarchy = new GameObject("Visuals");
            visualsHierarchy.transform.parent = bossSubPhase.transform;

            GameObject weaponsHierarchy = new GameObject("Weapons");
            weaponsHierarchy.transform.parent = bossSubPhase.transform;

            GameObject bossPhase = new GameObject("Phase1");

            bossPhase.transform.parent = bossGO.transform;
            bossSubPhase.transform.parent = bossPhase.transform;

            Debug.Log("Please make sure to use unique names for the prefabs you use in a level. Boss " +
                "prefabs with identical names will be treated and pooled as if they were the same Boss.");

            Undo.RegisterCreatedObjectUndo(bossGO, "Create Boss Template");
        }

        [MenuItem("Edit/Shmup Boss/Agents/Create Pickup Template", false, 1019)]
        private static void CreatePickupTemplate()
        {
            GameObject pickupGO = new GameObject(
                "Pickup",
                typeof(Rigidbody2D),
                typeof(CircleCollider2D),
                typeof(Pickup),
                typeof(TrackerPlayer),
                typeof(MagnetMover));

            pickupGO.layer = ProjectLayers.Pickups;

            Rigidbody2D rb2D = pickupGO.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;

            CircleCollider2D collider2D = pickupGO.GetComponent<CircleCollider2D>();
            collider2D.isTrigger = true;

            Undo.RegisterCreatedObjectUndo(pickupGO, "Create Pickup Template");
        }
    }
}