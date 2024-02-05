using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShmupBoss
{
    /// <summary>
    /// The bomb component can be added to any scene game object, it will enable firing a 
    /// bomb with the Fire2 button, or show a virtual bomb button in case of building for 
    /// mobile devices. All the fields have tooltips which explain their functionality.
    /// <br>Do not add this to the player or something that is spawned, add it only to 
    /// scene game objects or a new game object in your level.</br>
    /// <br>Please make sure to give the bomb Timer to avoid firing all the bombs with 
    /// one button as the fire button is caught when pressed and not when button is down.</br>
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Bomb")]
    public class Bomb : MonoBehaviour
    {
        public static Bomb Instance;
        
        public event CoreDelegate OnBombsCountChange;

        [Tooltip("How many bombs the player starts off with in the level. This value is " +
            "set in each level and is not persistent. The player will not save any bombs " +
            "from one level to another and is unaffected by the death of the player.")]
        [SerializeField]
        private int bombsCount = 3;
        public int BombsCount
        {
            get
            {
                return bombsCount;
            }
            set
            {
                if (value > bombsMaxCount)
                {
                    bombsCount = bombsMaxCount;
                }
                else if (value < 0)
                {
                    bombsCount = 0;
                }
                else
                {
                    bombsCount = value;
                }
            }
        }

        [Tooltip("The max bomb count the player can have after taking bomb drops/pickups.")]
        [SerializeField]
        private int bombsMaxCount = 5;

        [Tooltip("The bomb works by damaging all currently active enemies with this value. " +
            "Each enemy will have its health decreased by this value.")]
        [SerializeField]
        private float bombDamage = 100.0f;

        [Tooltip("Important, this is the time between firing each bomb, the button for the bomb " +
            "is detected whenever its pressed and not when its down. This means if you do not " +
            "have a value here, a single click could release all bombs. Please make sure to have " +
            "a few seconds delay.")]
        [SerializeField]
        private float bombTimer = 3.0f;

        [Tooltip("This time will give the player invincibility after firing the bomb.")]
        [SerializeField]
        private float playerInvincibilityTime = 2.0f;

        [Tooltip("The sound effect played when the bomb has been fired. You can use this, " +
            "or you can also use the SFX Player Firing Bomb event.")]
        [SerializeField]
        VolumetricAC bombSFX;

        [Tooltip("A Unity event raised when firing the bomb which you can add methods to. " +
            "Please note that you can also use the FX Spawner Player to spawn effects " +
            "when the bomb is fired away.")]
        public UnityEvent OnFiringBomb;

        private bool isFiring;
        private float currentBombTimer;
        private AudioSource audioSource;
        private bool canPlaySFX;

        private void OnValidate()
        {
            if(bombsCount < 0)
            {
                bombsCount = Mathf.Abs(bombsCount);
            }

            if(bombsMaxCount < bombsCount)
            {
                bombsMaxCount = bombsCount;
            }
        }

        private void Awake()
        {
            Instance = this;
            PrepareAudioSource();
        }

        void Update()
        {
            if (currentBombTimer < Mathf.Epsilon)
            {
                isFiring = false;
                currentBombTimer = bombTimer;
            }

            if (isFiring)
            {
                currentBombTimer -= Time.deltaTime;
                return;
            }

            if (PlayerInput.IsFire2Pressed)
            {
                if (bombsCount <= 0)
                {
                    return;
                }

                isFiring = true;
                InvokeEvents();

                PlaySFX();
                ActivatePlayerInvinciblity();
                DecrementBombCount();
                DamageAllCurrentEnemies();
            }
        }

        private void InvokeEvents()
        {
            OnFiringBomb?.Invoke();

            if(Level.Instance == null)
            {
                return;
            }

            if(Level.Instance.PlayerComp == null)
            {
                return;
            }

            if (!Level.Instance.IsPlayerAlive)
            {
                return;
            }

            Player player = Level.Instance.PlayerComp;
            player.NotifiyOfFiringBomb();
        }

        private void PrepareAudioSource()
        {
            if (bombSFX == null)
            {
                return;
            }

            if (bombSFX.AC == null)
            {
                return;
            }

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = bombSFX.AC;
            audioSource.volume = bombSFX.Volume;

            canPlaySFX = true;
        }

        private void PlaySFX()
        {
            if (!canPlaySFX)
            {
                return;
            }

            audioSource.Play();
        }

        private void ActivatePlayerInvinciblity()
        {
            if (playerInvincibilityTime <= Mathf.Epsilon)
            {
                return;
            }

            if (Level.Instance == null)
            {
                return;
            }

            if (Level.Instance.PlayerComp == null)
            {
                return;
            }

            Player player = Level.Instance.PlayerComp;
            player.AddImmunityTime(playerInvincibilityTime);
        }

        private void DecrementBombCount()
        {
            BombsCount--;
            RaiseOnBombCountChange();
        }

        private void DamageAllCurrentEnemies()
        {
            if (InGamePoolManager.DoesContainItems<Enemy>())
            {
                List<IInGamePool> iInGamePool = InGamePoolManager.GetList<Enemy>();
                List<IInGamePool> iInGamePoolCopy = new List<IInGamePool>();

                foreach (IInGamePool iInGame in iInGamePool)
                {
                    iInGamePoolCopy.Add(iInGame);
                }

                foreach (IInGamePool inGameObject in iInGamePoolCopy)
                {
                    Enemy enemy = inGameObject as Enemy;

                    if (enemy != null)
                    {
                        enemy.TakeHit(bombDamage);
                    }
                }
            }
        }

        public void AdjustBombsCount(int adjustment)
        {
            BombsCount += adjustment;

            if (adjustment != 0)
            {
                RaiseOnBombCountChange();
            }
        }

        private void RaiseOnBombCountChange()
        {
            OnBombsCountChange?.Invoke(new BombArgs(bombsCount));
        }
    }
}