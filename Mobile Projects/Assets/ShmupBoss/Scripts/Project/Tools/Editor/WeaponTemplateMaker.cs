using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    public class WeaponTemplateMaker : ScriptableObject
    {
        [MenuItem("Edit/Shmup Boss/Weapons/Create Player Munition Weapon", false, 1021)]
        private static void CreatePlayerMunitionWeapon()
        {
            GameObject weaponGO = new GameObject("WeaponMunitionPlayer",
                typeof(WeaponMunitionPlayer));

            Undo.RegisterCreatedObjectUndo(weaponGO, "Create Player Munition Weapon Template");
        }

        [MenuItem("Edit/Shmup Boss/Weapons/Create Player Particle Weapon", false, 1022)]
        private static void CreatePlayerParticleWeapon()
        {
            GameObject weaponGO = new GameObject("WeaponParticlePlayer",
                typeof(ParticleSystem),
                typeof(WeaponParticlePlayer));

            Undo.RegisterCreatedObjectUndo(weaponGO, "Create Player Particle Weapon Template");
        }

        [MenuItem("Edit/Shmup Boss/Weapons/Create Enemy Munition Weapon", false, 1023)]
        private static void CreateEnemyMunitionWeapon()
        {
            GameObject weaponGO = new GameObject("WeaponMunitionEnemy",
                typeof(WeaponMunitionEnemy));

            Undo.RegisterCreatedObjectUndo(weaponGO, "Create Enemy Munition Weapon Template");
        }

        [MenuItem("Edit/Shmup Boss/Weapons/Create Enemy Particle Weapon", false, 1024)]
        private static void CreateEnemyParticleWeapon()
        {
            GameObject weaponGO = new GameObject("WeaponParticleEnemy",
                typeof(ParticleSystem),
                typeof(WeaponParticleEnemy));

            Undo.RegisterCreatedObjectUndo(weaponGO, "Create Enemy Particle Weapon Template");
        }

        [MenuItem("Edit/Shmup Boss/Weapons/Create Bomb Feature", false, 1025)]
        private static void CreateBombFeature()
        {
            GameObject bombGO = new GameObject("Bomb",
                typeof(Bomb));

            Undo.RegisterCreatedObjectUndo(bombGO, "Create Bomb Feature Template");
        }
    }
}

