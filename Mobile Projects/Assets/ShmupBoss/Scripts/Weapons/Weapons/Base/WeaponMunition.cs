using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for enemy and player weapons which spawn pooled game objects which can be either 
    /// bullets or missiles.
    /// </summary>
    public abstract class WeaponMunition : Weapon
    {
        /// <summary>
        /// If true, the weapon will use the rotation which is the result of the heirarchy.<br></br>
        /// When false, it will instead use the weapon local rotation regardless of any rotation 
        /// in the hierarchy.<br></br>
        /// By default, it is uncheked, so that any agent rotation does not affect the weapons.
        /// </summary>
        [SB_HelpBox("Preview button only works if the prefab is dropped inside the scene." + "\n" +
            "Additionally, the preview does not show any weapon, nested or input rotation or the " +
            "effect of the scatter angle.")]
        [Header("Advanced Rotation Options")]
        [Tooltip("When checked, the weapon will use the rotation which is the result of the hierarchy." + "\n" + 
            "Unchecked, it will instead use the weapon local rotation regardless of any rotation in the hierarchy."
            + "\n" + "By default, it is uncheked, so that any agent rotation does not affect the weapons.")]
        [SerializeField]
        protected bool isUsingNestedRotation;

        /// <summary>
        /// This can be used if you wish to use a single game object to rotate multiple weapons on the same 
        /// agent without nesting them under the same parent.
        /// </summary>
        [Tooltip("This can be used if you wish to use a single game object to rotate multiple weapons on the " +
            "same agent without nesting them under the same parent.")]
        [Space]
        [SerializeField]
        protected bool isUsingInputRotation;

        /// <summary>
        /// Only used if isUsingInputRotation is checked, the trnasform you reference here will function as 
        /// a rotator for this weapon instead of this game object local rotation or nested rotation.
        /// </summary>
        [Tooltip("Only used if isUsingInputRotation is checked, the transform you reference here will function " +
            "as a rotator for this weapon instead of this game object local rotation or nested rotation.")]
        [SerializeField]
        protected Transform inputRotation;

        /// <summary>
        /// Use if you do not want to be concerned with any agent or local weapon rotation.
        /// </summary>
        [Tooltip("Use if you do not want to be concerned with any agent or local weapon rotation.")]
        [Space]
        [SerializeField]
        protected bool isUsingOverrideDirection;

        /// <summary>
        /// Will only be used if isUsingOverrideDirection is true, this vector will override any other 
        /// agent or weapon rotation.
        /// </summary>
        [Tooltip("Will only be used if isUsingOverrideDirection is checked, this vector will override " +
            "any other agent or weapon rotation.")]
        [SerializeField]
        protected Vector2 overrideDirection;

        /// <summary>
        /// The munition (bullet or missile) which will be fired by this weapon.
        /// </summary>
        public Munition MunitionPrefab 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Modifies the scale of the fired munition.
        /// </summary>
        public Vector3 MunitionScale
        {
            get;
            private set;
        }

        /// <summary>
        /// Forces the munition to be fired in a random radial manner, the bigger the angle 
        /// the more spread out the munition will be.
        /// </summary>
        protected float ScatterAngle;

        /// <summary>
        /// The sound effect which will be played when this weapon fires.
        /// </summary>
        protected VolumetricAC FiringSFX;

        /// <summary>
        /// Data used to get and store this weapon variables.
        /// </summary>
        public abstract WeaponMunitionStage CurrentStage
        {
            get;
        }

        [HideInInspector]
        [SerializeField]
        protected Transform previewMunitionHierarchy;

        /// <summary>
        /// Resets the firing time and the rate control multiplier and sets the weapon 
        /// variables using the currently used stage.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            SetToStage(CurrentStage);

            DestroyPreviewMunition();
        }

        /// <summary>
        /// Resets the firing SFX cool down timer.
        /// </summary>
        protected virtual void OnEnable()
        {
            isInFiringSfxCooldown = false;
        }

        /// <summary>
        /// Stores all the variables of this weapon using an input weapon stage.
        /// </summary>
        /// <param name="stage">The weapon stage the agent is currently using.</param>
        public virtual void SetToStage(WeaponMunitionStage stage)
        {
            if (stage == null)
            {
                Debug.Log("WeaponMunition.cs: Trying to set a munition " +
                    "weapon stage, but that stage seems to be null.");

                return;
            }

            rate = stage.Rate;
            MunitionPrefab = stage.MunitionPrefab;

            MunitionScale = stage.MunitionScale;

            FiringSFX = stage.FiringSFX;
            SfxCoolDownTime = stage.FiringSfxCoolDownTime;

            ScatterAngle = stage.ScatterAngle;

            BulletsNumber = stage.BulletsNumber;
            ArcAngleSpread = stage.ArcAngleSpread;
        }

        /// <summary>
        /// Fires the munition after it has set its firing direction (based on agent direction or override 
        /// direction) and its emit direction which is related to any radial firing options.
        /// </summary>
        /// <param name="agentDirection">This direction can be used to determine the firing direction.</param>
        public override void Fire(Vector2 agentDirection)
        {
            if (MunitionPrefab == null)
            {
                Debug.Log("WeaponMunition.cs: Weapon: " + name + " is trying to fire a munition " +
                    "but it can't find the munition prefab.");

                return;
            }

            if (!CanFindMunitionPool())
            {
                return;
            }

            Vector2 firingDirection;

            if (isUsingOverrideDirection)
            {
                firingDirection = overrideDirection;
            }
            else
            {
                firingDirection = agentDirection;
            }

            if (BulletsNumber <= 1)
            {
                EmitMunition(firingDirection);
            }
            else
            {
                float firingDirectionAngle = Math2D.VectorToDegree(firingDirection);
                float arcStartAngle = firingDirectionAngle - (ArcAngleSpread * 0.5f);
                float arcStepAngle = ArcAngleSpread / (BulletsNumber - 1);

                for (int i = 0; i < BulletsNumber; i++)
                {
                    float emitAngle = arcStartAngle + (i * arcStepAngle);
                    Vector2 emitDirection = Math2D.DegreeToVector2(emitAngle);

                    EmitMunition(emitDirection);
                }
            }
        }

        /// <summary>
        /// spawns the munition and prepares its initial position, rotation and direction by initializing it.
        /// </summary>
        /// <param name="emitDirection">The direction the munition is fired at, this combines the firing 
        /// direction and any radial firing options.</param>
        protected void EmitMunition(Vector2 emitDirection)
        {
            GameObject munitionGO = SpawnMunition();
            InitializeMunition(munitionGO, emitDirection);
        }

        /// <summary>
        /// Prepares the munition initial position, rotation and direction.
        /// </summary>
        /// <param name="munitionGO">The munition game object which will be initialized.</param>
        /// <param name="emitDirection">The direction the munition is fired at, this combines the firing 
        /// direction and any radial firing options.</param>
        protected virtual void InitializeMunition(GameObject munitionGO, Vector2 emitDirection)
        {
            munitionGO.transform.localScale = MunitionScale;

            Quaternion firingRotation;

            if (isUsingInputRotation)
            {
                if (ScatterAngle != 0.0f)
                {
                    float randomScatterAngle = UnityEngine.Random.Range(-ScatterAngle, ScatterAngle);
                    firingRotation = inputRotation.localRotation * Quaternion.AngleAxis(randomScatterAngle, Vector3.forward);
                }
                else
                {
                    firingRotation = inputRotation.localRotation;
                }
            }
            else
            {
                Quaternion weaponRotation;

                if (isUsingNestedRotation)
                {
                    weaponRotation = transform.rotation;
                }
                else
                {
                    weaponRotation = transform.localRotation;
                }

                Vector3 weaponRotationInAngles = weaponRotation.eulerAngles;

                // This is to make sure the bullets are always facing the screen even if the weapon firing 
                // them had rotation on the X or Y axis, This is especially needed if you are firing a 2D sprite.
                Vector3 weapon2DRotationInAngles = new Vector3(0.0f, 0.0f, weaponRotationInAngles.z);
                Quaternion weapon2DRotation = Quaternion.Euler(weapon2DRotationInAngles);

                if (ScatterAngle != 0.0f)
                {
                    float randomScatterAngle = UnityEngine.Random.Range(-ScatterAngle, ScatterAngle);
                    firingRotation = weapon2DRotation * Quaternion.AngleAxis(randomScatterAngle, Vector3.forward);
                }
                else
                {
                    firingRotation = weapon2DRotation;
                }
            }

            Munition munition = munitionGO.GetComponent<Munition>();

            if (!isUsingOverrideDirection)
            {
                munition.Initialize(transform.position, firingRotation, emitDirection);
            }
            else
            {
                munition.Initialize(transform.position, Quaternion.identity, emitDirection);
            }
        }

        /// <summary>
        /// Checks if the proper agent munition pool exists or not
        /// </summary>
        /// <returns>True if the munition pool is found.</returns>
        protected abstract bool CanFindMunitionPool();

        /// <summary>
        /// Spawns the munition from its proper agent munition pool.
        /// </summary>
        /// <returns>The spawned munition.</returns>
        protected abstract GameObject SpawnMunition();

        /// <summary>
        /// Will play the sound using a coroutine if there is a cool down time, if there is no cool down, 
        /// it will simply play the sound using the SfxSource instantly.
        /// </summary>
        protected override void StartFiringSoundCoroutine()
        {
            if (FiringSFX.AC == null)
            {
                return;
            }

            if (SfxCoolDownTime <= 0.0f)
            {
                SfxSource.Instance.PlayClip(FiringSFX.AC,FiringSFX.Volume);
            }
            else
            {
                StartCoroutine(PlayFiringSound());
            }
        }

        protected IEnumerator PlayFiringSound()
        {
            if (!isInFiringSfxCooldown)
            {
                SfxSource.Instance.PlayClip(FiringSFX.AC, FiringSFX.Volume);
                isInFiringSfxCooldown = true;

                yield return new WaitForSeconds(SfxCoolDownTime);

                isInFiringSfxCooldown = false;
            }
        }

        /// <summary>
        /// Creates in the scene an approximate preview of how the munition would look like after the weapon 
        /// has been operating for the preview duration.<br></br>
        /// This preview does not take into account the rotation of the weapon, locally or nested, or any input 
        /// rotation or the scatter angle.
        /// </summary>
        /// <param name="previewDuration">How the fired munition would like after this duration of time.</param>
        /// <param name="stageToPreview">The weapon munition stage which will be previewed.</param>
        /// <param name="isEnemyMunition">To know in which direction to spawn the munition.</param>
        protected void CreatePreviewMunitionInEditor(float previewDuration, WeaponMunitionStage stageToPreview, bool isEnemyMunition)
        {
            if (stageToPreview.MunitionPrefab == null)
            {
                Debug.Log("WeaponMunition.cs: You need to have a munition prefab refernced to be able to " +
                    "preview a munition weapon.");

                return;
            }

            Munition munition = stageToPreview.MunitionPrefab;

            float munitionSpeed = MunitionTools.FindMunitionSpeed(munition);

            int numberOfPreviewMunition = Mathf.CeilToInt(stageToPreview.Rate * previewDuration);

            if (numberOfPreviewMunition <= 0)
            {
                Debug.Log("WeaponMunition.cs: weapon munition rate is too small " +
                    "or the preview duration is too short to be able to preview any " +
                    "munition, try to either increase the preview duration or the weapon's rate.");

                return;
            }

            float timeIntervalForEachMunition = previewDuration / numberOfPreviewMunition;
            float stepDistanceForEachMunition = munitionSpeed * timeIntervalForEachMunition;           

            previewMunitionHierarchy = new GameObject("----- Preview Munition -----").transform;
            previewMunitionHierarchy.gameObject.AddComponent<Disposable>();

            Vector3 firingDirection;

            if (isUsingOverrideDirection)
            {
                firingDirection = overrideDirection;
            }
            else
            {
                if (isEnemyMunition)
                {
                    firingDirection = FacingDirections.NonPlayer;
                }
                else
                {
                    firingDirection = FacingDirections.Player;
                }
            }

            if (stageToPreview.BulletsNumber <= 1)
            {
                for (int i = 0; i < numberOfPreviewMunition; i++)
                {
                    GameObject previewMunition = Instantiate(stageToPreview.MunitionPrefab.gameObject,
                        transform.position,
                        Quaternion.identity,
                        previewMunitionHierarchy);

                    previewMunition.transform.localScale = stageToPreview.MunitionScale;
                    previewMunition.transform.position = transform.position + (firingDirection * i * stepDistanceForEachMunition);

                    if (isEnemyMunition)
                    {
                        previewMunition.transform.Rotate(Vector3.forward, FacingAngles.NonPlayer - 90.0f);
                    }
                    else
                    {
                        previewMunition.transform.Rotate(Vector3.forward, FacingAngles.Player - 90.0f);
                    }
                }
            }
            else
            {
                float firingDirectionAngle = Math2D.VectorToDegree(firingDirection);
                float arcStartAngle = firingDirectionAngle - (stageToPreview.ArcAngleSpread * 0.5f);
                float arcStepAngle = stageToPreview.ArcAngleSpread / (stageToPreview.BulletsNumber - 1);

                for (int i = 0; i < numberOfPreviewMunition; i++)
                {
                    for (int j = 0; j < stageToPreview.BulletsNumber; j++)
                    {
                        GameObject previewMunition = Instantiate(stageToPreview.MunitionPrefab.gameObject,
                            transform.position,
                            Quaternion.identity,
                            previewMunitionHierarchy);

                        previewMunition.transform.localScale = stageToPreview.MunitionScale;

                        float emitAngle = arcStartAngle + (j * arcStepAngle);
                        Vector3 emitDirection = Math2D.DegreeToVector3(emitAngle);

                        previewMunition.transform.position = transform.position + (emitDirection * i * stepDistanceForEachMunition);

                        Vector3 munitionRotationInAngles = new Vector3(0.0f, 0.0f, emitAngle - 90.0f);
                        Quaternion munitionRotation = Quaternion.Euler(munitionRotationInAngles);

                        previewMunition.transform.localRotation = munitionRotation;
                    }
                }
            }
        }

        public void DestroyPreviewMunitionInEditor()
        {
            if (previewMunitionHierarchy != null)
            {
                DestroyImmediate(previewMunitionHierarchy.gameObject);
            }
        }

        /// <summary>
        /// Clears the scene of any previously created preview munitions.
        /// </summary>
        public void DestroyPreviewMunition()
        {
            if (previewMunitionHierarchy != null)
            {
                Destroy(previewMunitionHierarchy.gameObject);
            }
        }
    }
}