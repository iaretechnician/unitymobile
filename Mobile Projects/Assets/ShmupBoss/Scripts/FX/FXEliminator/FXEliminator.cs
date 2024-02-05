using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Eliminates an FX by either despawning it to the FXPool or destroying it depending on how it was created.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Eliminators/FX Eliminator")]
    public class FXEliminator : MonoBehaviour
    {
        /// <summary>
        /// After the assigned time is expired, do you want to despawn an FX to the FXPool 
        /// or do you wish to destroy it?<br></br>
        /// This option is important, if an FX was spawned, then you must select the despawn option, 
        /// if it was instantiated then you must select the destroy option, destroying a spawned FX 
        /// will make the FXPool empty and attempting to despawn an instatiated FX won't work.
        /// </summary>
        [Tooltip("After the assigned time is expired, do you want to despawn an FX to the FXPool or " +
            "do you wish to destroy it? This option is important, if an FX was spawned, then you must " +
            "select the despawn option, if it was instantiated then you must select the destroy option, " +
            "destroying a spawned FX will make the FXPool empty and attempting to despawn an instantiated " +
            "FX won't work.")]
        [SerializeField]
        protected EliminationOption eliminationOption;

        /// <summary>
        /// The time until eliminating an FX.
        /// </summary>
        [Tooltip("The time until eliminating an FX.")]
        [SerializeField]
        protected float waitTime;

        /// <summary>
        /// If this is true, then FX will move with the background, giving it the feeling of a grounded effect.
        /// </summary>
        [Tooltip("If this is checked, then FX will move with the background, " +
            "giving it the feeling of a grounded effect.")]
        [SerializeField]
        protected bool isParentedToTreadmill;

        protected virtual void OnEnable()
        {
            Eliminate();

            if (isParentedToTreadmill && Level.IsFinishedLoading)
            {
                if(Treadmill.Instance == null)
                {
                    return;
                }

                if(Treadmill.Instance.GroundEnemiesOnTreadmillHierarchy == null)
                {
                    return;
                }

                transform.parent = Treadmill.Instance.GroundEnemiesOnTreadmillHierarchy;
            }
        }

        protected virtual void OnDisable()
        {
            if (isParentedToTreadmill && Level.IsFinishedLoading)
            {
                transform.position = Vector3.zero;
            }
        }

        protected virtual void Eliminate()
        {
            if (eliminationOption == EliminationOption.Despawn && FXPool.Instance != null)
            {
                StartCoroutine(DespawnAfterPlay());
            }
            else
            {
                DestroyAfterPlay();
            }
        }

        protected virtual IEnumerator DespawnAfterPlay()
        {
            yield return new WaitForSeconds(waitTime);

            FXPool.Instance.Despawn(gameObject);

            yield return null;
        }

        protected virtual void DestroyAfterPlay()
        {
            Destroy(gameObject, waitTime);
        }
    }
}