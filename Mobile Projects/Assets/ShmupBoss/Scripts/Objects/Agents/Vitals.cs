namespace ShmupBoss
{
    /// <summary>
    /// Stores the values of health and shield, including full values and maximum values 
    /// with proper protections. Used in agents.<br></br>
    /// Make sure to first set the maximum values and then full values before assigning 
    /// the current values because there are protections that test first the max
    /// and full values before assigning any current values.
    /// </summary>
    [System.Serializable]
    public struct Vitals
    {
        private float health;
        public float Health
        {
            get
            {
                return health;
            }
            set
            {
                if (value < 0f)
                {
                    health = 0f;
                }
                else if (value > FullHealth)
                {
                    health = FullHealth;
                }
                else
                {
                    health = value;
                }
            }
        }

        private float fullHealth;
        public float FullHealth
        {
            get
            {
                return fullHealth;
            }
            set
            {
                if (value > MaxHealth)
                {
                    fullHealth = MaxHealth;
                }
                else
                {
                    fullHealth = value;
                }
            }
        }

        public float MaxHealth
        {
            get;
            set;
        }

        private float shield;
        public float Shield
        {
            get
            {
                return shield;
            }
            set
            {
                if (value < 0.0f)
                {
                    shield = 0.0f;
                }
                else if (value > FullShield)
                {
                    shield = FullShield;
                }
                else
                {
                    shield = value;
                }
            }
        }

        private float fullShield;
        public float FullShield
        {
            get
            {
                return fullShield;
            }
            set
            {
                if (value > MaxShield)
                {
                    fullShield = MaxShield;
                }
                else
                {
                    fullShield = value;
                }
            }
        }

        public float MaxShield
        {
            get;
            set;
        }

    }
}