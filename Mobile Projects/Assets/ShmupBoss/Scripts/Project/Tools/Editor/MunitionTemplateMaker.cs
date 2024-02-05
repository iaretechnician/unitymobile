using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    public class MunitionTemplateMaker : ScriptableObject
    {
        [MenuItem("Edit/Shmup Boss/Munition/Create Basic Bullet Template", false, 1031)]
        private static void CreateBulletTemplate()
        {
            GameObject bulletGO = new GameObject("Bullet",
                typeof(Rigidbody2D),
                typeof(CircleCollider2D),
                typeof(Bullet));

            Rigidbody2D rb2D = bulletGO.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;

            CircleCollider2D collider2D = bulletGO.GetComponent<CircleCollider2D>();
            collider2D.isTrigger = true;

            Undo.RegisterCreatedObjectUndo(bulletGO, "Create Basic Bullet Template");
        }

        [MenuItem("Edit/Shmup Boss/Munition/Create Player Missile Template", false, 1032)]
        private static void CreatePlayerMissileTemplate()
        {
            GameObject missileGO = new GameObject(
                "PlayerMissile",
                typeof(CircleCollider2D),
                typeof(Missile),
                typeof(TrackerRandomEnemy),
                typeof(MissileMoverFollowingRandomEnemy),
                typeof(MunitionHitFX));

            missileGO.layer = ProjectLayers.PlayerMunition;

            CircleCollider2D collider2D = missileGO.GetComponent<CircleCollider2D>();
            collider2D.isTrigger = true;

            Undo.RegisterCreatedObjectUndo(missileGO, "Create Player Missile Template");
        }

        [MenuItem("Edit/Shmup Boss/Munition/Create Enemy Missile Template", false, 1033)]
        private static void CreateEnemyMissileTemplate()
        {
            GameObject missileGO = new GameObject(
                "EnemyMissile",
                typeof(CircleCollider2D),
                typeof(Missile),
                typeof(TrackerPlayer),
                typeof(MissileMoverFollowingPlayer),
                typeof(MunitionHitFX));

            missileGO.layer = ProjectLayers.EnemyMunition;

            CircleCollider2D collider2D = missileGO.GetComponent<CircleCollider2D>();
            collider2D.isTrigger = true;

            Undo.RegisterCreatedObjectUndo(missileGO, "Create Enemy Missile Template");
        }
    }
}

