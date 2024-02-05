using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Is made up of multiple phases and each of these phases are made up of its own sub phases<br></br>
    /// This boss component can simulate a multi phased boss, with phases being activated according to the
    /// current phase index with options of having them visible/active, damageable and shooting from the 
    /// start or not.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Agents/Boss")]
    public class Boss : MonoBehaviour
    {
        public CoreDelegate OnSubPhaseElimination;

        /// <summary>
        /// When this is true, the boss will not be destroyed when the last phase is eliminated while other 
        /// phases are still active and will instead only be destroyed when each and every phase has been eliminated.
        /// </summary>
        [Tooltip("When this is checked, the boss will not be destroyed when the last phase is eliminated while other " +
            "phases are still active and will instead only be destroyed when each and every phase has been eliminated.")]
        [SerializeField]
        private bool DestroyOnlyWhenAllPhasesDestroyed;

        /// <summary>
        /// The phases that make up the boss.<br></br>
        /// These phases are made up of sub phaese which need to have the boss sub phase script added to it
        /// and not just a standard enemy script.
        /// </summary>
        [Tooltip("The phases that make up the boss. These phases are made up of sub phases which need to " +
            "have the boss sub phase script added to it and not just a standard enemy script.")]
        [SerializeField]
        private BossPhase[] bossPhases;
        public BossPhase[] BossPhases
        {
            get
            {
                return bossPhases;
            }
        }

        /// <summary>
        /// The phase the boss is currently in, this will determine when phases are full activated activated
        /// (active, can be damaged and is shooting)
        /// </summary>
        private int currentPhaseIndex;

        /// <summary>
        /// If a phase can be damaged or is shooting from start, this makes sure it's also active/visible.<br></br> 
        /// (It's pointless to have a phase which can be damaged or is shooting but is inactive at the same time!)<br></br>
        /// Note: I could have renamed or structured things differently so that the phase visiblity is not
        /// related to the other activities but I kept it this way so that it's easier to understand and comprehend
        /// the script overall.
        /// </summary>
        private void OnValidate()
        {
            if(bossPhases == null || bossPhases.Length == 0)
            {
                return;
            }

            foreach (BossPhase bossPhase in bossPhases)
            {
                if (bossPhase.CanBeDamagedFromStart || bossPhase.IsShootingFromStart)
                {
                    if (!bossPhase.IsGameObjectActiveFromStart)
                    {
                        bossPhase.IsGameObjectActiveFromStart = true;
                    }
                }
            }

            // These below lines do not actually change anything, only the check boxes at the inspector.
            // I made sure these are set to active in the inspector as well, to help the user understand
            // that a first phase means that it is fully active and how this boss script works.
            bossPhases[0].IsGameObjectActiveFromStart = true;
            bossPhases[0].CanBeDamagedFromStart = true;
            bossPhases[0].IsShootingFromStart = true;
        }

        private void Awake()
        {
            SetPhaseIndexInSubPhases();
            BindMethodsToEvents();
        }

        private void OnEnable()
        {
            if (bossPhases.Length <= 0)
            {
                Debug.Log("Boss.cs: Boss named: " + name + " does contain any phases!" +
                    " Please do assign phases and sub phases to it.");

                return;
            }

            ResetSettings();
        }

        private void ResetSettings()
        {
            ResetPhasesStatus();
            DeactivateAllSubPhases();
            VerifyAllDestroyedVersionsAreDeactivated();
            ActivateFirstPhase();
            GoThroughAllOtherPhases();
        }

        /// <summary>
        /// A sub phase does not know to which phase it belongs to unless injected with 
        /// the information. This method will assign the phase index to its sub phases.
        /// </summary>
        private void SetPhaseIndexInSubPhases()
        {
            for (int i = 0; i < bossPhases.Length; i++)
            {
                foreach (BossSubPhase bossSubPhase in bossPhases[i].BossSubPhases)
                {
                    bossSubPhase.BossPhaseIndex = i;
                }
            }
        }

        private void BindMethodsToEvents()
        {
            OnSubPhaseElimination += UpdateBoss;
        }

        private void ResetPhasesStatus()
        {
            currentPhaseIndex = 0;

            foreach (BossPhase bossPhase in bossPhases)
            {
                bossPhase.NumberOfEliminatedSubPhases = 0;
            }
        }

        private void DeactivateAllSubPhases()
        {
            for (int i = 0; i < BossPhases.Length; i++)
            {
                foreach (BossSubPhase bossSubPhase in BossPhases[i].BossSubPhases)
                {
                    bossSubPhase.gameObject.SetActive(false);
                    bossSubPhase.IsInvincibleTrigger2 = true;
                    bossSubPhase.CanShoot = false;
                }
            }
        }

        private void VerifyAllDestroyedVersionsAreDeactivated()
        {
            for (int i = 0; i < BossPhases.Length; i++)
            {
                foreach (BossSubPhase bossSubPhase in BossPhases[i].BossSubPhases)
                {
                    if (bossSubPhase.DestroyedVersion != null)
                    {
                        bossSubPhase.DestroyedVersion.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void ActivateFirstPhase()
        {
            foreach (BossSubPhase bossSubPhase in BossPhases[0].BossSubPhases)
            {
                bossSubPhase.gameObject.SetActive(true);
                bossSubPhase.IsInvincibleTrigger2 = false;
                bossSubPhase.CanShoot = true;
            }
        }

        /// <summary>
        /// Activates/deactivates each phase, sets its inviniciblity and if it 
        /// shoots or not based on the phase paramteres.
        /// </summary>
        private void GoThroughAllOtherPhases()
        {
            for (int i = 1; i < BossPhases.Length; i++)
            {
                foreach (BossSubPhase bossSubPhase in BossPhases[i].BossSubPhases)
                {
                    if (BossPhases[i].IsGameObjectActiveFromStart)
                    {
                        bossSubPhase.gameObject.SetActive(true);
                    }

                    if (BossPhases[i].CanBeDamagedFromStart)
                    {
                        bossSubPhase.IsInvincibleTrigger2 = false;
                    }

                    if (BossPhases[i].IsShootingFromStart)
                    {
                        bossSubPhase.CanShoot = true;
                    }
                }
            }
        }

        /// <summary>
        /// When a sub phase has been eliminated, this method is executed and increments the number of
        /// eliminated sub phases in its respective phase.<br></br>
        /// It also checks if the phase index or any other changes are needed to the boss after that sub phase elimination.
        /// </summary>
        /// <param name="args">The boss sub phase args which contain what phase it belongs to.</param>
        private void UpdateBoss(System.EventArgs args)
        {
            BossSubPhaseArgs bossSubPhaseArgs = args as BossSubPhaseArgs;

            if(bossSubPhaseArgs == null)
            {
                return;
            }

            // The phase index in which a sub phase has just been eliminated in.
            int phaseIndex = bossSubPhaseArgs.BossPhaseIndex;
            bossPhases[phaseIndex].NumberOfEliminatedSubPhases++;

            UpdateCurrentPhaseIndex();
        }

        /// <summary>
        /// Checks if all sub phases of the current phase have been eliminated so that
        /// the boss can move to the next phase and if so will activate the next phases.
        /// </summary>
        private void UpdateCurrentPhaseIndex()
        {
            int currentSubPhasesLength = BossPhases[currentPhaseIndex].BossSubPhases.Length;
            int currentSubPhasesDestroyedCounter = BossPhases[currentPhaseIndex].NumberOfEliminatedSubPhases;

            if (currentSubPhasesDestroyedCounter >= currentSubPhasesLength && currentPhaseIndex < BossPhases.Length - 1)
            {
                IncrementPhaseIndex();
                ActivateNewlyAccessedBossPhases();
            }
        }

        /// <summary>
        /// Increments the current phase index, but in addition to that it also check if the next phase has been
        /// already eliminated, if it has, it increments the value again and checks for the next phase and so on.
        /// </summary>
        private void IncrementPhaseIndex()
        {
            currentPhaseIndex++;

            int currentSubPhasesLength = BossPhases[currentPhaseIndex].BossSubPhases.Length;
            int currentSubPhasesDestroyedCounter = BossPhases[currentPhaseIndex].NumberOfEliminatedSubPhases;

            if (currentSubPhasesDestroyedCounter >= currentSubPhasesLength && currentPhaseIndex < BossPhases.Length - 1)
            {
                IncrementPhaseIndex();
            }
        }

        /// <summary>
        /// After the current phase index has been updated, this method looks into the boss
        /// phase that is indexed by it and activates it fully.
        /// </summary>
        private void ActivateNewlyAccessedBossPhases()
        {
            if (currentPhaseIndex > BossPhases.Length - 1)
            {
                return;
            }

            if (!BossPhases[currentPhaseIndex].IsGameObjectActiveFromStart)
            {
                foreach (BossSubPhase enemyBossPhase in BossPhases[currentPhaseIndex].BossSubPhases)
                {
                    enemyBossPhase.gameObject.SetActive(true);
                }
            }

            if (!BossPhases[currentPhaseIndex].CanBeDamagedFromStart)
            {
                foreach (BossSubPhase enemyBossPhase in BossPhases[currentPhaseIndex].BossSubPhases)
                {
                    enemyBossPhase.IsInvincibleTrigger2 = false;
                }
            }

            if (!BossPhases[currentPhaseIndex].IsShootingFromStart)
            {
                foreach (BossSubPhase enemyBossPhase in BossPhases[currentPhaseIndex].BossSubPhases)
                {
                    enemyBossPhase.CanShoot = true;
                }
            }
        }

        /// <summary>
        /// After a sub phase has been eliminated, this method is executed to see if the boss should
        /// be eliminated or not.
        /// </summary>
        public void EliminateIfConditionsAreMet()
        {
            if (DestroyOnlyWhenAllPhasesDestroyed)
            {
                EliminateIfAllPhasesDestroyed();
            }
            else
            {
                EliminateIfLastPhaseDestroyed();
            }
        }

        /// <summary>
        /// Will check if all of the sub phases are eliminated, if all have been eliminated 
        /// the boss will be despawned.
        /// </summary>
        private void EliminateIfAllPhasesDestroyed()
        {
            for (int i = 0; i < bossPhases.Length; i++)
            {
                int numberOfTotalSubPhases = bossPhases[i].BossSubPhases.Length;
                int numberOfEliminatedSubPhases = bossPhases[i].NumberOfEliminatedSubPhases;

                if (numberOfEliminatedSubPhases < numberOfTotalSubPhases)
                {
                    // This means there is still one sub phase which is active.
                    return;
                }
            }

            Despawn();
        }

        /// <summary>
        /// Checks for the last phase of the boss, if it has been eliminated, 
        /// the boss will be despawned regardless if other phases are still active.
        /// </summary>
        private void EliminateIfLastPhaseDestroyed()
        {
            if (bossPhases[bossPhases.Length - 1].NumberOfEliminatedSubPhases >= bossPhases[bossPhases.Length - 1].BossSubPhases.Length)
            {
                Despawn();
            }
        }

        /// <summary>
        /// Eliminates the boss regardless of the conditions of its phases.
        /// This is currently needed when the boss exists the screen.
        /// </summary>
        public void EliminateRegardless()
        {
            Despawn();
        }

        private void Despawn()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
                        
            if (EnemyPool.Instance == null)
            {
                Debug.Log("Boss.cs: a boss named: " + name + " was unable to find the enemy pool" +
                    " and was deactivated instead of despawned from the pool.");

                gameObject.SetActive(false);
                return;
            }

            EnemyPool.Instance.Despawn(gameObject);
        }

        public void NotifyOfSubPhaseElimination(System.EventArgs args)
        {
            RaiseOnSubPhaseElimination(args);
        }

        protected void RaiseOnSubPhaseElimination(System.EventArgs args)
        {                       
            OnSubPhaseElimination?.Invoke(args);
        }
    }
}