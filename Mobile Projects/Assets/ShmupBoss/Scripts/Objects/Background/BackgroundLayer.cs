using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// All background objects used in a layer and how many times each background is repeated, 
    /// layer speed, depth, sequence operations and contains relavent information for correctly 
    /// spawning/despawning the layer.
    /// </summary>
    [System.Serializable]
    public class BackgroundLayer
    {
        [Tooltip("This layer name is only used for organizational purposes in the editor.")]
        public string LayerName;
        
        /// <summary>
        /// The background used in this layer with their repetition values.
        /// Background names used here must be unique in order to be pooled and spawned/despawned.
        /// </summary>
        public GoCount[] Backgrounds;

        public float Speed;

        /// <summary>
        /// What happens to the background layer after its sequence has finished.
        /// </summary>
        public SequenceEndOptions AfterSequenceEnds;

        /// <summary>
        /// The Z depth layer index where a background layer will be spawned, <br></br> 
        /// an index of 0 means the bottom most layer, higher numbers means the layers will go
        /// on top, the distance between these layers is determined by the space between indices.
        /// </summary>
        [Range(0, ProjectConstants.DepthIndexLimit)]
        public int DepthIndex;

        /// <summary>
        /// A layer seuqence is the order in which backgrounds are spawned, 
        /// a index value of 0 for example means the very first background to spawn etc..
        /// </summary>
        public int CurrentSequenceIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The pool transform is needed to keep the layers orgnized and to parent them to it when unused.
        /// </summary>
        public Transform PoolTransform
        {
            get;
            set;
        }

        /// <summary>
        /// The treadmill is the object which moves layers such as the background and ground targets,
        /// this transform is required so that the background layer is parented to it when used.
        /// </summary>
        public Transform TreadmillTransform
        {
            get;
            set;
        }

        public GameObject LastBackground
        {
            get;
            set;
        }

        /// <summary>
        /// Only used if the sequence end option is stop at final image, used to know when to stop the sequence.
        /// </summary>
        public int StopAtFinalImageCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Used when the sequence end options are either stop at final image or end sequence to know what to stop
        /// spawning new backgrounds when they activate the replace background method.
        /// </summary>
        public bool CanContinueSpawning
        {
            get;
            set;
        }

        /// <summary>
        /// The names of each background to spawn in order, these names will be used with the scrolling 
        /// background script to be able to pull the right background by name. The background names must be unique.
        /// </summary>
        [HideInInspector]
        public List<string> BackgroundsSequence = new List<string>();
    }
}