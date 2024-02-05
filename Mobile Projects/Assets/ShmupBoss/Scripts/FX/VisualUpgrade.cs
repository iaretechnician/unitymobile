using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// When a visual upgrade drop item is picked up by the player, this component will update the player visual.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Player/Visual Upgrade")]
    [RequireComponent(typeof(Player))]
    public class VisualUpgrade : MonoBehaviour
    {
        [Tooltip("An array of all the possible visual upgrades for the player." + "\n" + "Each array element " +
            "represents an upgrade stage, element 0 will display the first stage, element 1 will display the " +
            "second stage and so on.")]
        [SerializeField]
        private GameObject[] upgradeVisuals;

        private Player player;

        private void Awake()
        {
            player = GetComponent<Player>();

            if(player == null)
            {
                Debug.Log("VisualUpgrade.cs: Can't find the player component.");

                return;
            }

            player.OnActivation += UpdateVisual;
            player.OnVisualUpgrade += UpdateVisual;
            player.OnVisualDowngrade += UpdateVisual;
        }

        public void UpdateVisual(System.EventArgs args)
        {
            for (int i = 0; i < upgradeVisuals.Length; i++)
            {
                if (i == player.CurrentVisualUpgradeStage)
                {
                    upgradeVisuals[i].SetActive(true);
                }
                else
                {
                    upgradeVisuals[i].SetActive(false);
                }

                if (player.CurrentVisualUpgradeStage >= upgradeVisuals.Length)
                {
                    upgradeVisuals[upgradeVisuals.Length - 1].SetActive(true);
                }
            }
        }
    }
}