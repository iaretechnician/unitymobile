using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Eliminates an animated FX by either despawning it to the FXPool or destroying it 
    /// depending on how it was created.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Eliminators/Animated FX Eliminator")]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Renderer))]
    public class AnimatedFXEliminator : FXEliminator
    {
        /// <summary>
        /// If this is true, the animated clip time will be discarded and the wait time will be used instead.
        /// </summary>
        [Space]
        [Header("Animated Clip Settings")]
        [Tooltip("If this is true, the animated clip time will be discarded and the wait time will be used instead.")]        
        [SerializeField]
        private bool isUsingWaitTimeInsteadOfClipTime;

        /// <summary>
        /// Delays playing the animated clip by this time value.
        /// </summary>
        [Tooltip("Delays playing the animated clip by this time value.")]
        [SerializeField]
        protected float startDelayTime;

        /// <summary>
        /// The animated clip time length.
        /// </summary>
        protected float playTime;

        private Animator animator;
        private Renderer targetRenderer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            targetRenderer = GetComponent<Renderer>();

            if (animator == null)
            {
                Debug.Log("AnimatedFXEliminator.cs: " + name +" Can't find animator to cache.");

                return;
            }

            if(targetRenderer == null)
            {
                Debug.Log("AnimatedFXEliminator.cs: " + name + " Can't find a renderer to cache.");

                return;
            }

            Deactivate();
            FindPlayTime();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(PlayClip(startDelayTime));
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Deactivate();
        }

        /// <summary>
        /// Stops playing the animated clip
        /// </summary>
        private void Deactivate()
        {
            animator.enabled = false;
            targetRenderer.enabled = false;
        }

        private void FindPlayTime()
        {
            if (isUsingWaitTimeInsteadOfClipTime && waitTime > 0.0f)
            {
                playTime = waitTime;
            }
            else
            {
                playTime = GetClipLength();
            }
        }

        float GetClipLength()
        {
            return animator.GetCurrentAnimatorStateInfo(0).length;
        }

        protected virtual IEnumerator PlayClip(float delay)
        {
            yield return new WaitForSeconds(delay);

            Activate();
            Eliminate();
        }

        private void Activate()
        {
            animator.enabled = true;
            targetRenderer.enabled = true;
        }

        protected override IEnumerator DespawnAfterPlay()
        {
            yield return new WaitForSeconds(playTime);

            FXPool.Instance.Despawn(gameObject);

            yield return null;
        }          

        protected override void DestroyAfterPlay()
        {
            Destroy(gameObject, playTime);
        }
    }
}