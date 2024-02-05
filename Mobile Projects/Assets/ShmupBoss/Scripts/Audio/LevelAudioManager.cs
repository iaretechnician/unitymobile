using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Gives the ability to control the music and sfx via a master audio mixer and mixer groups, 
    /// and it also enables selecting different audio sound effects for the different level UI events.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Level/Level Audio Manager")]
    public class LevelAudioManager : AudioManager
    {
        /// <summary>
        /// An array of all possible audio clips that can be played by any level UI.<br></br>
        /// Please be careful not to give two audio clips for the same UI event.
        /// </summary>
        [Tooltip("An array of all possible audio clips that can be played by any level UI. Please be " +
            "careful not to give two audio clips the same UI event.")]
        public volumetricACTriggeredByLevelUIEvent[] uiAudioClips = new volumetricACTriggeredByLevelUIEvent[]
        {
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.GameOverCanvasAppear),
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.LevelCompleteAppear),
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.PauseButtonPressed),
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.ResumeButtonPressed),
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.RetryButtonPressed),
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.MainMenuButtonPressed),
            new volumetricACTriggeredByLevelUIEvent (LevelUIEvent.NextLevelButtonPressed),
        };

        /// <summary>
        /// For indexing all possible level UI events and their associated audio clips.
        /// </summary>
        private Dictionary<LevelUIEvent, VolumetricAC> _uiClipsDictionary;

        protected override void Initialize()
        {
            base.Initialize();

            InitializeUIAudio();
            SetBackgroundMusic();
        }

        /// <summary>
        /// Indexes the clips in the ui clips dictionary and adds the play UI SFX method to the LevelUI event.
        /// </summary>
        private void InitializeUIAudio()
        {
            _uiClipsDictionary = new Dictionary<LevelUIEvent, VolumetricAC>();

            for (int i = 0; i < uiAudioClips.Length; i++)
            {
                if (!_uiClipsDictionary.ContainsKey(uiAudioClips[i].LevelUIEventTrigger))
                {
                    _uiClipsDictionary.Add(uiAudioClips[i].LevelUIEventTrigger, uiAudioClips[i].Clip);
                }
                else
                {
                    Debug.Log("LevelAudioManager.cs: you have two audio clip that use the same event :" + uiAudioClips[i].LevelUIEventTrigger.ToString());
                }
            }

            LevelUI.OnUIEvent += PlayUiSfx;
        }

        /// <summary>
        /// This method is executed everytime a level UI event occurs, it checks for the type of event through 
        /// the arguments and plays the appropriate clips categorized in the dictionary.
        /// </summary>
        /// <param name="args">System event arguments that contain the type of level UI event.</param>
        private void PlayUiSfx(System.EventArgs args)
        {
            LevelUIEventArgs levelUIEventArg = (LevelUIEventArgs)args;

            if (levelUIEventArg == null)
            {
                Debug.Log("LevelAudioManager.cs: you are trying to play a UI clip through LevelUI.OnUIEvent " +
                    "without passing the correct LevelUIEventArg.");

                return;
            }            

            LevelUIEvent uiEvent = levelUIEventArg.UIEvent;

            if (_uiClipsDictionary.ContainsKey(uiEvent))
            {
                PlayUIClip(_uiClipsDictionary[uiEvent]);
            }
        }
    }
}