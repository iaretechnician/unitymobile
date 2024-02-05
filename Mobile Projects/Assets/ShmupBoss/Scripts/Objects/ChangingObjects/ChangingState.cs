namespace ShmupBoss
{
    /// <summary>
    /// Used to hold the value of a struct (such as an int, Vector3 or an enum) 
    /// and raise an event whenever the struct value changes. 
    /// </summary>
    /// <typeparam name="T">The struct (such as an int, Vector3 or an enum) that you want an event risen 
    /// when it's changed.</typeparam>
    public class ChangingState<T> where T : struct
    {
        /// <summary>
        /// The event that will be risen when the value of this generic class struct is changed.
        /// </summary>
        public event System.Action OnStateChange;

        private T current;

        public T Current
        {
            get
            {
                return current;
            }
            set
            {
                if (!current.Equals(value))
                {
                    current = value;
                    OnStateChange?.Invoke();
                }

                current = value;
            }
        }
    }
}