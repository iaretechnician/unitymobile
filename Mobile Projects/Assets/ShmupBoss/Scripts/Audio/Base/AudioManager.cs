using UnityEngine;
using UnityEngine.Audio;

namespace ShmupBoss 
{
    /// <summary>
    /// Base class for the audio managers used in the level and the main menu which links the SFX and music 
    /// volume to a master audio mixer and mixer groups.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        private const string MasterGroupName = "Master";
        private const string MasterVolumeParameterName = "MasterVol";
        private const string MusicGroupName = "Music";
        private const string MusicVolumeParameterName = "MusicVol";
        private const string SfxGroupName = "SFX";
        private const string SfxVolumeParameterName = "SFXVol";

        /// <summary>
        /// The background music that will be looped and controlled by 
        /// the music volume in the settings.
        /// </summary>
        [Tooltip("The background music that will be looped and controlled by " +
            "the music volume in the settings.")]
        [SerializeField]
        protected AudioClip backgroundMusic;

        [HideInInspector]
        public AudioMixer MasterMixer;

        [HideInInspector]
        public AudioSource MusicSource;

        [HideInInspector]
        public AudioSource SfxSource;

        protected AudioMixerGroup MasterMixerGroup;
        protected AudioMixerGroup MusicMixerGroup;
        public AudioMixerGroup SfxMixerGroup
        {
            get;
            protected set;
        }

        private float _masterVolume = 1.0f;
        public float MasterVolume
        {
            get
            {
                return _masterVolume;
            }
            set
            {
                if(MasterMixerGroup != null)
                {
                    if(value <= -20.0f)
                    {
                        value = -80.0f;
                    }

                    _masterVolume = value;
                    MasterMixerGroup.audioMixer.SetFloat(MasterVolumeParameterName, _masterVolume);
                }
            }
        }

        private float _musicVolume = 1.0f;
        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                if(MusicMixerGroup != null)
                {
                    if(value <= -20.0f)
                    {
                        value = -80.0f;
                    }

                    _musicVolume = value;
                    MusicMixerGroup.audioMixer.SetFloat(MusicVolumeParameterName, _musicVolume);
                }
            }
        }

        private float _sfxVolume = 1.0f;
        public float SfxVolume
        {
            get
            {
                return _sfxVolume;
            }
            set
            {
                if (SfxMixerGroup != null)
                {
                    if (value <= -20.0f)
                    {
                        value = -80.0f;
                    }

                    _sfxVolume = value;
                    SfxMixerGroup.audioMixer.SetFloat(SfxVolumeParameterName, _sfxVolume);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            CacheMasterMixer();

            if (MasterMixer == null)
            {
                Debug.Log("AudioManager.cs: Master Mixer couldn't be found in: Resources/Audio/Mixers/MasterMixer. " +
                    "Did you move it?");

                return;
            }

            FindMixerGroupsInMasterMixer();
            CreateAudioSources();
            LinkAudioSourceOutputToMixerGroups();

            Initializer.Instance.SubscribeToStage(Initialize, 4);
        }

        private void CacheMasterMixer()
        {
            MasterMixer = Resources.Load<AudioMixer>("Audio/Mixers/MasterMixer");
        }

        private void FindMixerGroupsInMasterMixer()
        {
            AudioMixerGroup[] groups = MasterMixer.FindMatchingGroups("");

            MasterMixerGroup = GetGroupByName(groups, MasterGroupName);
            MusicMixerGroup = GetGroupByName(groups, MusicGroupName);
            SfxMixerGroup = GetGroupByName(groups, SfxGroupName);
        }

        private void CreateAudioSources()
        {
            MusicSource = gameObject.AddComponent<AudioSource>();
            SfxSource = gameObject.AddComponent<AudioSource>();
        }

        private void LinkAudioSourceOutputToMixerGroups()
        {
            MusicSource.outputAudioMixerGroup = MusicMixerGroup;
            SfxSource.outputAudioMixerGroup = SfxMixerGroup;
        }

        /// <summary>
        /// Searches for the exact name of an audio mixer group in an array of audio mixer groups 
        /// and returns the match.
        /// </summary>
        /// <param name="groups">The audio mixer groups to search its names</param>
        /// <param name="name">The name of the audio mixer group that you want to return.</param>
        /// <returns>The audio mixer group with the exact name you are looking for.</returns>
        private AudioMixerGroup GetGroupByName(AudioMixerGroup[] groups, string name)
        {
            for(int i =0; i < groups.Length; i++)
            {
                if(groups[i].name == name)
                {
                    return groups[i];
                }
            }

            Debug.Log("AudioManager.cs: the audio mixer you have in the resource somehow does not match teh Shmup Boss naming convention");
            return null;
        }

        /// <summary>
        /// Sets the current game settings to the current audio mixer groups so that they 
        /// can be controlled by user settings.
        /// </summary>
        protected virtual void Initialize()
        {
            if (GameManager.IsInitialized)
            {
                MasterVolume = GameManager.Instance.CurrentGameSettings.MasterVolume;
                MusicVolume = GameManager.Instance.CurrentGameSettings.MusicVolume;
                SfxVolume = GameManager.Instance.CurrentGameSettings.SfxVolume;
            }
        }

        protected void SetBackgroundMusic()
        {
            MusicSource.loop = true;
            MusicSource.clip = backgroundMusic;
            MusicSource.Play();
        }

        public void PauseMusic()
        {
            MusicSource.Pause();
            SfxSource.Pause();
        }

        public void UnPauseMusic()
        {
            MusicSource.UnPause();
            SfxSource.UnPause();
        }

        /// <summary>
        /// Plays a UI clip using the SFX source so that its volume can also be controlled by the SFX settings.<br></br>
        /// This does not use sound pooling since a UI clip is a relatively rare occurrence.
        /// </summary>
        /// <param name="clip"></param>
        public void PlayUIClip(VolumetricAC clip)
        {
            if (SfxSource == null)
            {
                return;
            }
               
            if(clip.AC != null)
            {
                SfxSource.PlayOneShot(clip.AC, clip.Volume);
            }
        }
    }
}