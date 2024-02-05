using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// An audio clip with volume control (Volumetric Audio Clip) which is played after a main menu UI event occurs.
    /// </summary>
    [System.Serializable]
    public class VolumetricACTriggeredByMainUIEvent
    {
        /// <summary>
        /// The audio clip that will be played when a main menu UI event occurs.
        /// </summary>
        [Tooltip("The audio clip that will be played when a main menu UI event occurs.")]
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
        /// The main menu UI event that will cause an audio clip to play.
        /// </summary>
        [Tooltip("The main menu UI event that will cause an audio clip to play.")]
        [SerializeField]
        private MainUIEvent mainUIEventTrigger;
        public MainUIEvent MainUIEventTrigger
        {
            get
            {
                return mainUIEventTrigger;
            }
        }

        public VolumetricACTriggeredByMainUIEvent(MainUIEvent uiEvent)
        {
            mainUIEventTrigger = uiEvent;
            volumetricAC = new VolumetricAC();
        }
    }
}
