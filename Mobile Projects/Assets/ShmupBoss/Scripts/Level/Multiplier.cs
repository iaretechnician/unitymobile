using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This component is tightly involved in all aspects of the pack, it offers control for agents vitals, movers speed, 
    /// weapons rate, and munition damage and speed. This will essentially give you an easy to change control setting
    /// instead of rebuilding each level with a different difficulty manually, it will also allow difficulty
    /// global changes, that being said you will also need to be careful about the data array you input and 
    /// to make sure the input is correct and if using a main menu is in accordance with the difficulty
    /// values there.<br></br>
    /// All the multiplier data calculated here is stored inside the static CurrentMultiplier.cs class so that they can 
    /// be easily accessed throughout the pack without negatively affecting performace.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Level/Multiplier")]
    public sealed class Multiplier : Singleton<Multiplier>
    {
        /// <summary>
        /// Where the difficulty level is set, whether from the main menu 
        /// (By Game Manager) or from the number you input below (By Multiplier).
        /// </summary>
        [Tooltip("Where the difficulty level is set, whether from the main menu (By Game Manager) " +
            "or from the number you input below (By Multiplier).")]
        [SerializeField]
        private DifficultyLevelSource difficultyLevelSource;

        /// <summary>
        /// The index at which the multiplier data array will return the indexed array difficulty.<br></br>
        /// This will only be used if you have selected the difficulty level source to be: by multiplier.
        /// </summary>
        [Header("If Source Is By Multiplier")]
        [Tooltip("The index at which the multiplier data array will return the indexed array difficulty. This " +
            "will only be used if you have selected the difficulty level source to be: by multiplier.")]
        [SerializeField]
        private int difficultyLevel;

        /// <summary>
        /// The index at which the multiplier data array will return the indexed array difficulty.
        /// </summary>
        public int DifficultyLevel
        {
            get
            {
                int currentDifficultyLevel;
                
                if(difficultyLevelSource == DifficultyLevelSource.ByMultiplier)
                {
                    currentDifficultyLevel = difficultyLevel;
                }
                else
                {
                    if (GameManager.Instance.CurrentGameSettings == null)
                    {
                        return 0;
                    }

                    currentDifficultyLevel = GameManager.Instance.CurrentGameSettings.DifficultyLevel;
                }

                if (currentDifficultyLevel < 0)
                {
                    return 0;
                }
                else if (currentDifficultyLevel >= multiplierData.Length)
                {
                    return multiplierData.Length - 1;
                }

                return currentDifficultyLevel;
            }
        }

        /// <summary>
        /// This array is essential to the functioning of this pack, it gives you the option to change the 
        /// difficulty of the game. If you do not wish to have difficulties, keep its length at one and the 
        /// difficulty level at 0 and make sure difficulty source is set by Multiplier. and fill it with a 
        /// multiplier data. The difficulty level determines which multiplier data is returned from the array. 
        /// This diffuclty level can be set by the multiplier here or by the game manager settings menu. 
        /// The settings difficulty names array are currently set to 3, so the array here needs to be set to 3 as well, 
        /// if you change this array length to more or less you will need to change the available difficulty names 
        /// array in the SettingsMenu.cs script.
        /// </summary>
        [Header("Multiplier Data Array")]
        [Tooltip("This array is essential to the functioning of this pack, it gives you the option to change the " +
            "difficulty of the game. If you do not wish to have difficulties, keep its length at one and the " +
            "difficulty level at 0 and make sure difficulty source is set by Multiplier and fill it with a " +
            "multiplier data." + "\n\n" + "The difficulty level determines which multiplier data is returned from the array." +
            " This diffuclty level can be set by the multiplier here or by the game manager settings menu. " +
            "\n\n" +  "Settings difficulty names array are currently set to 3, so the array here needs to be set " +
            "to 3 as well, if you change this array length to more or less you will need to change the " +
            "available difficulty names array in the SettingsMenu.cs script.")]
        [SerializeField]
        private MultiplierData[] multiplierData;

        /// <summary>
        /// This data is used to change the difficulty in every level progression when using the infinite spawner.
        /// </summary>
        [Header("Infinite Level Multiplier Settings")]
        [Tooltip("This data is used to change the difficulty in every level progression when using the " +
            "infinite spawner.")]
        [SerializeField]
        private MultiplierData infiniteIncrementMultiplierData;

        public MultiplierData InfiniteIncrementMultiplierData
        {
            get
            {
                if (infiniteIncrementMultiplierData == null)
                {
                    MultiplierData defaultMultiplierData = 
                        Resources.Load<MultiplierData>("Multiplier/DefaultInfiniteMultiplierData");

                    if (defaultMultiplierData == null)
                    {
                        Debug.Log("Multiplier.cs: cannot find the default multuplier " +
                            "data inside the resources folder, have you moved it?");

                        return null;
                    }

                    return defaultMultiplierData;
                }

                return infiniteIncrementMultiplierData;
            }
        }

        /// <summary>
        /// This stores at what infinite level the game is at, and can determine the value of the 
        /// infinite multiplier data being returned after it has been multiplied by that level number.<br></br>
        /// This data is only updated after a new level has been reached which reduces the resources needed
        /// to multiply difficulty data values.
        /// </summary>
        private int currentInfiniteLevelIndex;
        public int CurrentInfiniteLevelIndex
        {
            get
            {
                return currentInfiniteLevelIndex;
            }
            set
            {
                currentInfiniteLevelIndex = value;
                UpdateInfiniteIncrementMultiplierData();
            }
        }

        /// <summary>
        /// Calculates the new incremented multiplied data after a new infinite level has been reached. 
        /// This takes into account the multiplier data difficulty and the newly reached level number.
        /// </summary>
        private void UpdateInfiniteIncrementMultiplierData()
        {
            currentInfiniteIncrementMultiplierData = GetUsedMultiplierData() +
                (InfiniteIncrementMultiplierData * CurrentInfiniteLevelIndex);

            CurrentMultiplier.SetData(currentInfiniteIncrementMultiplierData);
        }

        /// <summary>
        /// The current infinite multiplier data after having been calculated and updated according 
        /// to the current infinite level reached.
        /// </summary>
        private MultiplierData currentInfiniteIncrementMultiplierData;

        private void OnValidate()
        {
            if(multiplierData == null || multiplierData.Length <= 0)
            {
                difficultyLevel = 0;
                return;
            }

            int multiplierDataArrayLength = multiplierData.Length;

            if (difficultyLevel < 0)
            {
                difficultyLevel = 0;
            }
            else if(difficultyLevel >= multiplierDataArrayLength)
            {
                difficultyLevel = multiplierDataArrayLength - 1;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            CurrentMultiplier.SetData(GetUsedMultiplierData());
        }

        private MultiplierData GetUsedMultiplierData()
        {
            if (multiplierData == null || multiplierData.Length <= 0)
            {
                MultiplierData defaultMultiplierData = Resources.Load<MultiplierData>("Multiplier/DefaultMultiplierData");

                if (defaultMultiplierData == null)
                {
                    Debug.Log("Multiplier.cs: cannot find the default multuplier " +
                        "data inside the resources folder, have you moved it or changed its name?");

                    return null;
                }

                return defaultMultiplierData;
            }

            return multiplierData[DifficultyLevel];
        }
    }
}