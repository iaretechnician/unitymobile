using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Stores an array of boss sub phases that make up this phase along with its activity parameters.<br></br>
    /// Please note that if you set a phase as active, can be damaged and is shooting from start all together; 
    /// you are better off as keeping that phase as the first phase, as it will be pointless to have 
    /// a phase which is say at index 3 but is already fully active from start (index 0.)<br></br>
    /// You are meant to be using either "IsGameObjectActiveFromStart" alone or "CanBeDamagedFromStart" or 
    /// "IsShootingFromtStart". having all set to true means that this can be a first phase.
    /// </summary>
    [System.Serializable]
    public class BossPhase
    {
        /// <summary>
        /// The boss sub phases that make up this phase.
        /// </summary>
        [Tooltip("The boss sub phases that make up this phase.")]
        public BossSubPhase[] BossSubPhases;

        [Tooltip("When checked, this will make this phase active from the start even if it's" +
            " not the first phase. But while this phase will be visible it will not fire or be damaged" +
            " unless the parameters below are checked.")]
        public bool IsGameObjectActiveFromStart;

        [Tooltip("This will make it possible to damage this phase from the start.")]
        public bool CanBeDamagedFromStart;

        [Tooltip("This will make this phase start shooting from the start")]
        public bool IsShootingFromStart;

        /// <summary>
        /// This will keep track of how many sub phases have been already eliminated to determine if all
        /// sub phases have been eliminated or not.
        /// </summary>
        public int NumberOfEliminatedSubPhases
        {
            get;
            set;
        }
    }
}