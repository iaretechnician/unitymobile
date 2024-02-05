using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Script for changing parameters of an animator according to mover 
    /// direction to enable playing an animation when the direction changes.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Components/Animator Mover")]
    public class AnimatorMover : MonoBehaviour
    {
        [Tooltip("The animator which will be changed when the mover changes direction, " +
    "this animator must have the parameters named \"X\" and \"Y\", pleease refer " +
    "to the Animator update scene and its animator controller to see how it's implemented.")]
        [SerializeField]
        private Animator animator;

        [Tooltip("This AnimatorMover component will use the current direction of this mover. " +
            "Reference here the mover of the agent.")]
        [SerializeField]
        private Mover mover;

        private bool canSetAnimator;

        private void Awake()
        {
            FindIfCanSetAnimator();
        }

        private void FindIfCanSetAnimator()
        {
            if (animator == null)
            {
                Debug.Log("You need to manually assign an Animator to the AnimatorMover " +
                    "if you want to change the animator using player direction.\n" +
                    "Please make sure that animator has two parameters named \"X\" and \"Y\"");
            }

            if (mover == null)
            {
                mover = GetComponent<Mover>();
            }

            if (mover == null)
            {
                Debug.Log("You need to assign a mover to the AnimatorMover");
            }

            canSetAnimator = true;
        }

        private void Update()
        {
            if (!canSetAnimator)
            {
                return;
            }

            animator.SetFloat("X", mover.CurrentDirection.x);
            animator.SetFloat("Y", mover.CurrentDirection.y);
        }
    }
}