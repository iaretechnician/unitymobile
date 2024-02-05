using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Audio clip with volume control (volumetric audio clip)
    /// </summary>
    [System.Serializable]
    public class VolumetricAC
    {
        /// <summary>
        /// The audio clip that will be played.
        /// </summary>        
        [Tooltip("The audio clip that will be played.")]
        [SerializeField]
        private AudioClip ac;
        public AudioClip AC
        {
            get
            {
                return ac;
            }
        }

        [Range(0, ProjectConstants.MaxAudioClipVolume)]
        [SerializeField]
        private float volume = 1.0f;
        public float Volume
        {
            get
            {
                return volume;
            }
        }

        public VolumetricAC()
        {
            volume = 1.0f;
        }
    }
}