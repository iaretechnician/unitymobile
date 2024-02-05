using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// An audio clip with volume control (Volumetric Audio Clip) which is played after a level UI event occurs.
    /// </summary>
    [System.Serializable]
    public class volumetricACTriggeredByLevelUIEvent
    {
        /// <summary>
        /// The audio clip that will be played when a level UI event occurs.
        /// </summary>
        [Tooltip("The audio clip that will be played when a level UI event occurs.")]
        [SerializeField]
        private VolumetricAC volumetricAC;
        public VolumetricAC Clip
        {
            get
            {
                return volumetricAC;
            }
        }

        /// <summary>
        /// The level UI event that will cause an audio clip to play.
        /// </summary>
        [Tooltip("The level UI event that will cause an audio clip to play.")]
        [SerializeField]
        private LevelUIEvent levelUIEventTrigger;
        public LevelUIEvent LevelUIEventTrigger
        {
            get
            {
                return levelUIEventTrigger;
            }
        }

        public volumetricACTriggeredByLevelUIEvent(LevelUIEvent uiEvent)
        {
            levelUIEventTrigger = uiEvent;
            volumetricAC = new VolumetricAC();
        }
    }
}

