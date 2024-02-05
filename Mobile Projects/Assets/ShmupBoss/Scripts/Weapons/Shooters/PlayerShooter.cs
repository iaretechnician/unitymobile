using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Handles shooting all player weapons based on input or auto fire.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Shooters/Player Shooter")]
    [RequireComponent(typeof(Player))]
    public class PlayerShooter : MonoBehaviour
    {
        private Player player;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        void Update()
        {
            if (PlayerInput.IsFire1Pressed || PlayerInput.IsAutoFireEnabled)
            {
                for (int i = 0; i < player.MunitionWeapons.Length; i++)
                {
                    if(player.MunitionWeapons[i] == null)
                    {
                        continue;
                    }

                    player.MunitionWeapons[i].FireWhenPossible(FacingDirections.Player);
                }

                for (int i = 0; i < player.ParticleWeapons.Length; i++)
                {
                    if (player.ParticleWeapons[i] == null)
                    {
                        continue;
                    }

                    player.ParticleWeapons[i].FireWhenPossible(FacingDirections.Player);
                }
            }
        }
    }
}