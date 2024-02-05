using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// The pickup effect type and amount of its change
    /// </summary>
    [System.Serializable]
    public class PickupEffect
    {
        [SerializeField]
        private PickupType pickupType;
        public PickupType Type
        {
            get
            {
                return pickupType;
            }
        }

        [SerializeField]
        private int amount;
        public int Amount
        {
            get
            {
                return amount;
            }
        }
    }
}