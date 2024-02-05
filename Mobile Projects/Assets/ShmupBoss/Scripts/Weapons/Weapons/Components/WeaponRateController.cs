using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Weapon modifier which will change the firing rate of the weapon based on a timer.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Components/Weapon Rate Controller")]
    [RequireComponent(typeof(Weapon))]
    public class WeaponRateController : MonoBehaviour
    {
        [Tooltip("These rates will be multiplied with the weapon stage original rate. Each in turn.")]
        [SerializeField]
        private TimedRate[] timedRates;

        [Tooltip("If checked, your timed rate array will execute again after its finished and repeat infinitely." + 
            "\n" + "If unchecked the weapon will remain at the final timed rate.")]
        [SerializeField]
        private bool isLooped;

        private Weapon weapon;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();

            if(weapon == null)
            {
                Debug.Log("RateController.cs: Rate Multiplier needs a weapon to control.");

                return;
            }
        }

        private void OnEnable()
        {
            if(timedRates.Length <= 0)
            {
                Debug.Log("RateController.cs: You have assigned a rate " +
                    "controller to a weapon but you forgot to add any timed rates.");

                return;
            }
            
            StartCoroutine(MultiplyRate());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator MultiplyRate()
        {
            do
            {
                for (int i = 0; i < timedRates.Length; i++)
                {
                    weapon.RateControlMultiplier = timedRates[i].RateMultplier;
                    yield return new WaitForSeconds(timedRates[i].Duration);
                }
            }
            while (isLooped);

            yield return null;
        }
    }
}