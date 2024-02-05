using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for components which spawn FX game objects on certain events and are related to the weapons 
    /// which include: WeaponFireFX, ParticleHitFX and MunitionHiitFX.
    /// </summary>
    public abstract class ArmamentFX : MonoBehaviour
    {
        /// <summary>
        /// The FX game object that will be spawned when the required FX event occurs.
        /// </summary>
        [Tooltip("The FX game object that will be spawned when the required FX event occurs.")]
        [SerializeField]
        protected GameObject fxGo;
        public GameObject FxGo
        {
            get
            {
                return fxGo;
            }
        }

        [Tooltip("The scale change of the spawned FX.")]
        [SerializeField]
        protected Vector3 modifyScale = Vector3.one;

        [Tooltip("Will move the spawned FX away from its spawning position by this value.")]
        [SerializeField]
        protected Vector3 offsetPosition;

        protected Vector3 originalScale;
        protected Vector3 modifiedScale;

        [Tooltip("The new rotation of the spawned FX.")]
        [SerializeField]
        protected Vector3 newRotation;

        [HideInInspector]
        [SerializeField]
        protected Transform previewFXsHierarchy;

        protected virtual void Awake()
        {
            DestroyPreviewFXs();
            CacheArmamentAndBindMethods();
            CacheScale();
        }

        protected virtual void DestroyPreviewFXs()
        {
            if (previewFXsHierarchy != null)
            {
                Destroy(previewFXsHierarchy.gameObject);
            }
        }

        /// <summary>
        /// Will store the weapon or munition (armament) which causes the event to occur to spawn the 
        /// FX game object and it also binds any needed methods to the event.
        /// </summary>
        protected abstract void CacheArmamentAndBindMethods();

        protected virtual void CacheScale()
        {
            originalScale = FxGo.transform.localScale;
            modifiedScale = Vector3.Scale(originalScale, modifyScale);
        }

        protected abstract void Spawn(System.EventArgs args);

#if UNITY_EDITOR
        public virtual void CreatePreviewFXsInEditor()
        {
            previewFXsHierarchy = new GameObject("----- Preview Weapon FXs -----").transform;
            previewFXsHierarchy.gameObject.AddComponent<Disposable>();

            if(FxGo == null)
            {
                Debug.Log("Unable to generate preview because the FX game object is empty, please make " +
                    "sure your FX game object is referenced.");

                return;
            }

            GameObject previewFX = Instantiate(FxGo, transform.position, Quaternion.identity, previewFXsHierarchy);
            previewFX.transform.position += offsetPosition;
            previewFX.transform.localRotation = Quaternion.Euler(newRotation);
            Vector3 originalScale = FxGo.transform.localScale;
            previewFX.transform.localScale = Vector3.Scale(originalScale, modifyScale);
        }

        public void DestroyPreviewFXsInEditor()
        {
            if (previewFXsHierarchy != null)
            {
                DestroyImmediate(previewFXsHierarchy.gameObject);
            }
        }
#endif
    }
}