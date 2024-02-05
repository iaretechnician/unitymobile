using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Gives the ability to control the main menu music and sfx via a master audio mixer and mixer groups, 
    /// and it also enables selecting different audio sound effects for the different main menu UI events.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/Main Menu Audio Manager")]
    public class MainMenuAudioManager : AudioManager
    {
        /// <summary>
        /// An array of all possible audio clips that can be played by the main menu UI.<br></br>
        /// Please be careful not to give two audio clips for the same main menu UI event.
        /// </summary>
        [Tooltip("An array of all possible audio clips that can be played by the main menu UI. Please be " +
            "careful not to give two audio clips for the same main menu UI event.")]
        [SerializeField]
        private VolumetricACTriggeredByMainUIEvent[] uiAudioClips = new VolumetricACTriggeredByMainUIEvent[]
        {
            new VolumetricACTriggeredByMainUIEvent(MainUIEvent.ButtonPressed),
            new VolumetricACTriggeredByMainUIEvent(MainUIEvent.ToggleChange)
        };

        /// <summary>
        /// For indexing all possible main menu UI events and their associated audio clips.
        /// </summary>
        private Dictionary<MainUIEvent, VolumetricAC> _uiClipsDictionary;

        private void Start()
        {
            SetBackgroundMusic();
            InitializeUIAudio();
        }

        /// <summary>
        /// Indexes the clips in the ui clips dictionary and adds the play UI SFX method to the main menu UI event.
        /// </summary>
        private void InitializeUIAudio()
        {
            _uiClipsDictionary = new Dictionary<MainUIEvent, VolumetricAC>();

            for (int i = 0; i < uiAudioClips.Length; i++)
            {
                if (!_uiClipsDictionary.ContainsKey(uiAudioClips[i].MainUIEventTrigger))
                {
                    _uiClipsDictionary.Add(uiAudioClips[i].MainUIEventTrigger, uiAudioClips[i].Clip);
                }
                else
                {
                    Debug.Log("MainMenuAudioManager.cs: you have two audio clip that use the same event :" + uiAudioClips[i].MainUIEventTrigger.ToString());
                }
            }

            MainMenu.OnUIEvent += PlayUISFX;
        }

        /// <summary>
        /// This method is executed everytime a main menu UI event occurs, it checks for the type of event through 
        /// the arguments and plays the appropriate clips categorized in the dictionary.
        /// </summary>
        /// <param name="args">System event arguments that contain the type of main menu UI event.</param>
        private void PlayUISFX(System.EventArgs args)
        {
            MainUIEventArgs mainUIEventArg = (MainUIEventArgs)args;

            if (mainUIEventArg == null)
            {
                Debug.Log("MainMenuAudioManager.cs: you are trying to play a UI clip through MainMenuUI.OnUIEvent without passing the correct MainUIEventArg.");
                return;
            }

            MainUIEvent uiEvent = mainUIEventArg.UIEvent;

            if (_uiClipsDictionary.ContainsKey(uiEvent))
            {
                PlayUIClip(_uiClipsDictionary[uiEvent]);
            }
        }
    }
}