using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Responsible for playing all the different sfx in this pack, this will make sure audio sources are 
    /// reused and that they are also capped.<br></br> 
    /// If you wish to change the limit of how many sfx audio sources are played simultaneously you can go 
    /// to the projects constants script and change the max simultaneous audio clips value. 
    /// </summary>
    public class SfxSource : Singleton<SfxSource>
    {
        /// <summary>
        /// Audio sources which have finished playing and are ready to be reused.
        /// </summary>
        private Stack<AudioSource> audioSourcesPool = new Stack<AudioSource>();

        /// <summary>
        /// Audios sources currently playing sound effects.
        /// </summary>
        private List<AudioSource> audioSourcesInUse = new List<AudioSource>();

        private bool isInitialized;
        private Transform sfxHierarchy;

        /// <summary>
        /// This will save the frame number of the last time it was checked and is used for culling 
        /// any excess audio source not currently playing to put them back in the pool stack.
        /// </summary>
        private int lastCheckedFrame = -1;

        private void initialize()
        {
            sfxHierarchy = new GameObject("----- SFX Audio Sources-----").transform;
            isInitialized = true;
        }

        public void PlayClip(AudioClip audioClip, float volume = 1.0f)
        {
            if (audioClip == null)
            {
                Debug.Log("SfxSource.cs: SfxSource is being asked to play a clip which is empty, " +
                    "please double check that all of your sound effects have audio " +
                    "clips attached to them.");

                return;
            }

            if (!isInitialized)
            {
                initialize();
            }

            CullAudioSourceOnceEveryFrame();

            AudioSource source;

            // If there is no more available audio sources in the pool, and the audio sources currently 
            // playing do not exceed the max simultaneous audio clips limit a new audio source is created.
            if (audioSourcesPool.Count == 0)
            {
                if (audioSourcesInUse.Count >= ProjectConstants.MaxSimultaneousAudioClips)
                {
                    return;
                }

                source = CreateNewSfxPlayer(audioClip);
            }
            else
            {
                source = audioSourcesPool.Pop();
                source.name = audioClip.name;
            }

            audioSourcesInUse.Add(source);

            source.clip = audioClip;
            source.volume = volume;
            source.Play();
        }

        /// <summary>
        /// Culls the audio sources in use if this has not already happened this frame.
        /// </summary>
        private void CullAudioSourceOnceEveryFrame()
        {
            if (lastCheckedFrame != Time.frameCount)
            {
                lastCheckedFrame = Time.frameCount;
                CullAudioSourcesInUse();
            }
        }

        /// <summary>
        /// Checks if the audio sources in use have finished playing, if they 
        /// have they will be put back into the audio source pool stack and 
        /// removed from the in use list.
        /// </summary>
        private void CullAudioSourcesInUse()
        {
            if(audioSourcesInUse.Count == 0)
            {
                return;
            }

            for(int i = audioSourcesInUse.Count-1; i >=0; i--)
            {
                if(audioSourcesInUse[i] != null)
                {
                    if (!audioSourcesInUse[i].isPlaying)
                    {
                        audioSourcesPool.Push(audioSourcesInUse[i]);
                        audioSourcesInUse.RemoveAt(i);                       
                    }
                }           
            }          
        }

        /// <summary>
        /// Creates a new audio source which will play the sfx audio clip.
        /// </summary>
        /// <param name="audioClip">The audio clip to play</param>
        /// <returns>The newly created audio source which is ready to receive the sfx clip and play it.</returns>
        private AudioSource CreateNewSfxPlayer(AudioClip audioClip)
        {
            AudioSource source = new GameObject(audioClip.name).AddComponent<AudioSource>();
            source.transform.parent = sfxHierarchy;

            // This will enable the sfx audio source to be controlled by the level and main menu audio managers.
            source.outputAudioMixerGroup = AudioManager.Instance.SfxMixerGroup;

            return source;
        }
    }
}