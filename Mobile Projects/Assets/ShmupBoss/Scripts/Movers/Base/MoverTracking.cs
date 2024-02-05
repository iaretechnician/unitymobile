namespace ShmupBoss
{
    /// <summary>
    /// Moves the object based on tracking information which is extracted from a tracker component.
    /// </summary>
    public abstract class MoverTracking : MoverRotatableOnZ
    {
        protected Tracker tracker;

        /// <summary>
        /// Caches speed and tracker, adds on mover state change event and prepares
        /// the mover for following direction.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            CacheTracker();
        }

        protected virtual void CacheTracker()
        {
            tracker = GetComponent<TrackerPlayer>();

            if (tracker == null)
            {
                tracker = gameObject.AddComponent<TrackerPlayer>();
            }
        }
    }
}