using UnityEngine;
using UnityEditor;
using ShmupBoss;
using System;

namespace ShmupBossEditor
{
    public class LevelTemplateMaker : ScriptableObject
    {
        [MenuItem("Edit/Shmup Boss/Level/Create Finite Level Components", false, 1000)]
        private static void CreateFiniteTemplate()
        {
            PrintUIReminderMessage();
            CreateLevelComp();
            CreateMultiplier();
            CreateInputHandler();
            CreateScrollingBackground();
            CreateFields();
            CreatePools();
            CreateFiniteSpawner();
            AddCameraComponent();
            CreatePrefabsEditingAndTesting();
            CreateEmptyGameObjectSeparator();
        }

        [MenuItem("Edit/Shmup Boss/Level/Create Infinite Level Components", false, 1001)]
        private static void CreateInfiniteTemplate()
        {
            PrintUIReminderMessage();
            CreateLevelComp();
            CreateMultiplier();
            CreateInputHandler();
            CreateScrollingBackground();
            CreateFields();
            CreatePools();
            CreateInfiniteSpawner();
            AddCameraComponent();
            CreatePrefabsEditingAndTesting();
            CreateEmptyGameObjectSeparator();
        }

        private static void PrintUIReminderMessage()
        {
            Debug.Log("You still need to add a level UI for the level to function, please go to the " +
                        "prefabs folder, UI, then drag and drop LevelUI into the scene.");
        }

        private static void CreateLevelComp()
        {
            GameObject level = new GameObject("Level", typeof(Level));

            Undo.RegisterCreatedObjectUndo(level, "Create Level Comp");
        }

        private static void CreateMultiplier()
        {
            GameObject multiplier = Instantiate(Resources.Load<GameObject>("Multiplier/Multiplier"));
            multiplier.name = "Multiplier";

            Undo.RegisterCreatedObjectUndo(multiplier, "Create Multiplier");
        }

        private static void CreateInputHandler()
        {
            GameObject inputHandler = new GameObject("InputHandler", typeof(InputHandler));

            Undo.RegisterCreatedObjectUndo(inputHandler, "Create Input Handler");
        }

        private static void CreateScrollingBackground()
        {
            GameObject scrollingBackground = new GameObject("Scrolling Background", 
                typeof(ScrollingBackground), 
                typeof(Treadmill));

            Undo.RegisterCreatedObjectUndo(scrollingBackground, "Create Scrolling Background");
        }

        private static void CreateFields()
        {
            GameObject fields = new GameObject("Fields");
            GameObject playField = new GameObject("PlayField", typeof(PlayField));
            GameObject despawningField = new GameObject("DespawningField", typeof(DespawningField));
            GameObject groundEnemiesSpawningField = new GameObject("GroundEnemiesSpawningField", typeof(GroundEnemiesSpawningField));
            GameObject particleDestructionField = new GameObject("ParticleDestructionField", typeof(ParticleDestructionField));

            playField.transform.parent = fields.transform;
            despawningField.transform.parent = fields.transform;
            groundEnemiesSpawningField.transform.parent = fields.transform;
            particleDestructionField.transform.parent = fields.transform;

            groundEnemiesSpawningField.gameObject.SetActive(false);
            particleDestructionField.gameObject.SetActive(false);

            Undo.RegisterCreatedObjectUndo(fields, "Create Fields");
        }

        private static void CreatePools()
        {
            GameObject pools = new GameObject("Pools",
                typeof(EnemyPool),
                typeof(FXPool),
                typeof(PickupPool),
                typeof(MunitionPoolEnemy),
                typeof(MunitionPoolPlayer));

            Undo.RegisterCreatedObjectUndo(pools, "Create Pools");
        }

        private static void CreateFiniteSpawner()
        {
            GameObject finiteSpawner = new GameObject("FiniteSpawner", typeof(FiniteSpawner));

            Undo.RegisterCreatedObjectUndo(finiteSpawner, "Create Finite Spawner");
        }

        private static void CreateInfiniteSpawner()
        {
            GameObject infiniteSpawner = new GameObject("InfiniteSpawner", typeof(InfiniteSpawner));

            Undo.RegisterCreatedObjectUndo(infiniteSpawner, "Create Infinite Spawner");
        }


        private static void AddCameraComponent()
        {
            Camera cam = Camera.main;

            if (cam != null)
            {
                LevelCamera levelCamera = cam.gameObject.GetComponent<LevelCamera>();

                if (levelCamera == null)
                {
                    Undo.AddComponent(cam.gameObject, typeof(LevelCamera));
                }
            }
            else
            {
                Debug.Log("Was unable to find a main camera in the scene, please do assign a Level Camera " +
                    "script to the main camera in the scene.");
            }
        }

        private static void CreatePrefabsEditingAndTesting()
        {
            GameObject prefabsEditingTesting = new GameObject("----- Prefabs Editing And Testing -----");
            prefabsEditingTesting.AddComponent<Disposable>();

            Undo.RegisterCreatedObjectUndo(prefabsEditingTesting, "Create Prefabs Editing And Testing Template");
        }

        private static void CreateEmptyGameObjectSeparator()
        {
            GameObject gamePlaySeparator = new GameObject("----- GamePlay -----");

            Undo.RegisterCreatedObjectUndo(gamePlaySeparator, "Create Level Template");
        }
    }
}