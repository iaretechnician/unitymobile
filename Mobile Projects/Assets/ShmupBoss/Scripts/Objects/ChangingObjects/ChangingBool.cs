namespace ShmupBoss
{
    /// <summary>
    /// A class for storing a bool value which when changed to true or false invokes an event.
    /// </summary>
    public class ChangingBool
    {
        public event System.Action OnChangeToTrue;
        public event System.Action OnChangeToFalse;

        private bool currentValue;

        /// <summary>
        /// The value of the bool stored, this will invoke an event when its value is changed.
        /// </summary>
        public bool CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                if(!currentValue.Equals(value))
                {
                    currentValue = value;

                    if(value == true)
                    {
                        OnChangeToTrue?.Invoke();
                    }
                    else
                    {
                        OnChangeToFalse?.Invoke();
                    }
                }
            }
        }
    }
}