namespace ShmupBoss
{
    /// <summary>
    /// System Event arguments which store the pickup effect type just picked up.<br></br>
    /// This is used when the player makes a pickup to pass the pickup type which can
    /// affect how the UI for example is processed.
    /// </summary>
    public class PickupArgs : System.EventArgs
    {
        public PickupType PickupType;

        public PickupArgs(PickupType pickup)
        {
            PickupType = pickup;
        }
    }
}