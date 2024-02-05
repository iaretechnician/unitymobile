namespace ShmupBoss
{
    /// <summary>
    /// All the different constants used by this pack are saved in this class.<br></br>
    /// This will make it easier to change the constants and for me to tell users where 
    /// they can quickly change some values that will affect the pack without having to 
    /// enter each specific class.
    /// </summary>
    public static class ProjectConstants
    {       
        /// <summary>
        /// How many Z depth layers exist in the field, these layers are used to specify
        /// what goes on top of what.<br></br>
        /// Avoid values over 99 because background layers naming is depndent on being a 2 digit number.
        /// </summary>
        public const int DepthIndexLimit = 99;

        /// <summary>
        /// How many sfx audio source are playing simultaneously, this will help avoid the
        /// situation where too clips are playing at the exact same time and causing a
        /// significant noise.
        /// </summary>
        public const int MaxSimultaneousAudioClips = 10;

        /// <summary>
        /// The max volume a volumetric audio clip can have.
        /// </summary>
        public const float MaxAudioClipVolume = 4.0f;

        /// <summary>
        /// The minimum number of backgrounds which are currently active, this value will 
        /// depend on the size of your background in relation to your viewfield/playfield, 
        /// in most cases 2 or 3 will be enough but if you need to you can change it.
        /// </summary>
        public const int MinActiveBackgroundsLimit = 3;

        /// <summary>
        /// The default sapce between z depth layers used in the 2D level.
        /// </summary>
        public const float DefaultSpaceBetween2DDepthIndices = 1.0f;

        /// <summary>
        /// Determines the simulation time of the munition weapons preview button 
        /// (how the munition will look like after being fired for the preview duration.)
        /// </summary>
        public const float DefaultPreviewDuration = 5.0f;

        /// <summary>
        /// Distance which is used to determine when the treadmill will unparent 
        /// everything attached to it, go back to zero and attach it again.
        /// </summary>
        public const float ResettingDistance = -200f;

        /// <summary>
        /// Used to change the numbers you input into the turn speed and make 
        /// them easier to manage.
        /// </summary>
        public const float TurnSpeedFactor = 0.05f;

        /// <summary>
        /// The distance at which any tracker can be considered to have reached its target, 
        /// you might want change this value if your scene have a larger/smaller than usual scale.
        /// </summary>
        public const float ReachedPositionThreshold = 0.1f;

        /// <summary>
        /// The square of the distance at which any tracker can be considered to have reached its target, 
        /// you might want change this value if your scene have a larger/smaller than usual scale.
        /// </summary>
        public const float ReacehdPositionSqrThreshold = 0.1f;

        /// <summary>
        /// The angle at which some modifiers such as the roll by level direction 
        /// will stop trying to change the angle at, typically a value very close to zero. 
        /// </summary>
        public const float AngleTolerance = 0.1f;

        /// <summary>
        /// Delta length of lines which are drawn for preview in the editor such as the ones for the simple 
        /// mover, lower values will make the curve smoother, while higher values will make it more jagged.
        /// </summary>
        public const float LineDeltaLength = 0.1f;

        /// <summary>
        /// The scale of the bezier curve that the curve mover uses.<br></br> 
        /// If you feel that the initial curve created by the curve mover is too big or small you can adjust 
        /// the scale here.
        /// </summary>
        public const float PathConstructorScale = 5.0f;

        /// <summary>
        /// This tolerance determines how many samples are created for a bezier curve (path.)<br></br> 
        /// The lower the number the more samples are created.
        /// </summary>
        public const float PathPointsSampleTolerance = 0.05f;

        /// <summary>
        /// The time between finishing the level and displaying the level complete UI canvas.
        /// </summary>
        public const float LevelCompleteDelayTime = 2.0f;

        /// <summary>
        /// The time between the player final life elimination and displaying the game over UI canvas.
        /// </summary>
        public const float GameOverDelayTime = 2.0f;

        /// <summary>
        /// The margin value for labels used by various gizmos.
        /// </summary>
        public const float LabelMarginDistance = 0.5f;

        /// <summary>
        /// The space above and beneath the help box.
        /// </summary>
        public const float HelpBoxMargins = 10;

        /// <summary>
        /// The help box icon size. Used with the help box class property attribute.
        /// </summary>
        public const float HelpBoxIconWidth = 55;

        /// <summary>
        /// The layer prefix used by the scrolling background and background objects spawner.
        /// </summary>
        public const string LayerPrefix = "Layer";

        /// <summary>
        /// The file name used by the game manager and file management script to save/load game progress data.
        /// </summary>
        public const string SaveFileName = "Save File";

        /// <summary>
        /// The file name used by the game manager and file management script to save/load game settings data.
        /// </summary>
        public const string SettingsFileName = "Settings File";

        /// <summary>
        /// The name of the folder which the settings and save files will be saved in.
        /// </summary>
        public static string SaveFolderName = "Save Folder";

        /// <summary>
        /// This prefix length is used for retreiving the actual object name after the prefix.  
        /// A value of 2 is added to it because the layer prefix will have a 2 digit layer number added to it.
        /// </summary>
        public static int LayerPrefixLength
        {
            get
            {
                return LayerPrefix.Length + 2;
            }
        }
    }    
}