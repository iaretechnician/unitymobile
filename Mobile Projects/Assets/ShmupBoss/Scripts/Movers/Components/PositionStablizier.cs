using UnityEngine;

namespace ShmupBoss
{
    public class PositionStablizier : MonoBehaviour
    {

        private Vector3 startingPosition;
        private Vector3 positionBeforeStabilization;
        private bool isStablized;

        private void Awake()
        {
            startingPosition = transform.position;
        }

        private void OnEnable()
        {
            isStablized = false;
        }

        private void Update()
        {
            if (!isStablized)
            {
                positionBeforeStabilization = transform.position;
                isStablized = true;
            }
            else
            {
                transform.position = positionBeforeStabilization;
            }

        }

        private void OnDisable()
        {
            transform.position = startingPosition;
            isStablized = false;
        }
    }
}