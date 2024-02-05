namespace ShmupBoss
{
    /// <summary>
    /// This class can hold multiple methods which are stored in the event OnStageCall.
    /// This class is also compared and executed according to its stage order.
    /// </summary>
    public class StagedMethod : System.IComparable<StagedMethod>
    {
        public StagedMethod(System.Action method, int stageOrder)
        {
            StageOrder = stageOrder;
            SubscribeToOnStageCall(method);
        }

        /// <summary>
        /// Methods are added to this event which are later executed according the stage order.
        /// </summary>
        private event System.Action OnStageCall;

        /// <summary>
        /// This number determines the order of the execution of the staged method and 
        /// all of the stages which are subscribed to its OnStageCall event.
        /// </summary>
        public int StageOrder
        {
            get;
            private set;
        }

        /// <summary>
        /// Subscribes to the OnStageCall event so that all subscribed methods 
        /// can be later executed according to the index stage of the staged method.
        /// </summary>
        /// <param name="method"></param>
        public void SubscribeToOnStageCall(System.Action method)
        {
            if (method != null)
            {
                OnStageCall += method;
            }
        }

        /// <summary>
        /// Executes all of the methods which were subcribed to the 
        /// initializer and have the same stage order.
        /// </summary>
        public void Execute()
        {
            OnStageCall?.Invoke();
        }

        /// <summary>
        /// Compares this staged method to another staged method accoring 
        /// to their respective stage order.
        /// </summary>
        /// <param name="other">The other staged method to compare to</param>
        /// <returns>The comparison result</returns>
        public int CompareTo(StagedMethod other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return StageOrder.CompareTo(other.StageOrder);
        }
    }
}