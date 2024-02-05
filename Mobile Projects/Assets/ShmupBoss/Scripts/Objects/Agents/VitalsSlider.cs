using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// A Unity Engine UI slider that is controlled by an agent vitals (health or shield).
    /// </summary>
    [System.Serializable]
    public class VitalsSlider
    {
        /// <summary>
        /// Whether this slider is controlled by the agent health or shield.
        /// </summary>
        [SerializeField]
        private VitalsSliderType vitalsSliderType;

        /// <summary>
        /// The slider that will show the health or shield changes that occur to the agent.
        /// </summary>
        [SerializeField]
        private Slider sliderToControl;

        /// <summary>
        /// The agent whose vitals will control the slider.
        /// </summary>
        private AgentCollidable masterAgent;

        /// <summary>
        /// Subscribes the proper vital method to their related agent events.
        /// </summary>
        /// <param name="agent">The master agent whose vitals will control the slider.</param>
        public void Initialize(AgentCollidable agent)
        {
            masterAgent = agent;

            switch (vitalsSliderType)
            {              
                case VitalsSliderType.HealthBar:
                    {
                        masterAgent.OnActivation += UpdateHealthSlider;
                        masterAgent.OnHealthDamage += UpdateHealthSlider;

                        if (masterAgent is Player player)
                        {
                            player.OnPickUp += CheckForHealthPickUp;
                        }

                        break;
                    }

                case VitalsSliderType.ShieldBar:
                    {
                        masterAgent.OnActivation += UpdateShieldSlider;
                        masterAgent.OnShieldDamage += UpdateShieldSlider;

                        if (masterAgent is Player player)
                        {
                            player.OnPickUp += CheckForShieldPickUp;
                        }

                        break;
                    }
            }
        }

        private void UpdateHealthSlider(System.EventArgs args)
        {
            VitalsBarChangeArgs barChangeArgs = (VitalsBarChangeArgs)args;

            if (barChangeArgs == null)
            {
                return;
            }

            UpdateSlider(barChangeArgs.CurrentHealthBar, barChangeArgs.CurrentFullHealthBar);
        }

        private void UpdateShieldSlider(System.EventArgs args)
        {
            VitalsBarChangeArgs barChangeArgs = (VitalsBarChangeArgs)args;

            if (barChangeArgs == null)
            {
                return;
            }

            UpdateSlider(barChangeArgs.CurrentShieldhBar, barChangeArgs.CurrentFullShieldBar);
        }

        private void UpdateSlider(float currentValue, float maxValue)
        {
            sliderToControl.maxValue = maxValue;
            sliderToControl.value = currentValue;
        }

        /// <summary>
        /// Is activated when player makes a pickup to check if it's a health related pickup and if 
        /// the health slider needs to be updated.
        /// </summary>
        /// <param name="args">The arguments used by the raised event this method is 
        /// added to which should be a PickupArgs.</param>
        private void CheckForHealthPickUp(System.EventArgs args)
        {
            PickupArgs pickUpArgs = args as PickupArgs;

            if(pickUpArgs == null)
            {
                return;
            }

            if (pickUpArgs.PickupType == PickupType.Heal || 
                pickUpArgs.PickupType == PickupType.Health || 
                pickUpArgs.PickupType == PickupType.HealthUpgrade)
            {
                UpdateSlider(masterAgent.CurrentVitals.Health, masterAgent.CurrentVitals.FullHealth);
            }
        }

        /// <summary>
        /// Is activated when player makes a pickup to check if it's a shield related pickup and if 
        /// the shield slider needs to be updated.
        /// </summary>
        /// <param name="args">The arguments used by the raised event this method is 
        /// added to which should be a PickupArgs.</param>
        private void CheckForShieldPickUp(System.EventArgs args)
        {
            PickupArgs pickUpArgs = args as PickupArgs;

            if (pickUpArgs == null)
            {
                return;
            }

            if (pickUpArgs.PickupType == PickupType.Heal || 
                pickUpArgs.PickupType == PickupType.Shield || 
                pickUpArgs.PickupType == PickupType.ShieldUpgrade)
            {
                UpdateSlider(masterAgent.CurrentVitals.Shield, masterAgent.CurrentVitals.FullShield);
            }
        }
    }
}