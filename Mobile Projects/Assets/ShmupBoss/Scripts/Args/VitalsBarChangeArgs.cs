namespace ShmupBoss
{
    /// <summary>
    /// System Event arguments which store the current vitals for an agent. <br></br>
    /// This is passed in different events that are concerned with health/shield
    /// changes which can affect how the vitals slider UI is updated for example.
    /// </summary>
    public class VitalsBarChangeArgs : System.EventArgs
    {
        public float CurrentHealthBar;
        public float CurrentFullHealthBar;
        public float CurrentShieldhBar;
        public float CurrentFullShieldBar;

        public VitalsBarChangeArgs(Vitals currentVitals)
        {
            CurrentHealthBar = currentVitals.Health;
            CurrentFullHealthBar = currentVitals.FullHealth;
            CurrentShieldhBar = currentVitals.Shield;
            CurrentFullShieldBar = currentVitals.FullShield;
        }
    }
}