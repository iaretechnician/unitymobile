namespace ShmupBoss
{
    /// <summary>
    /// System event arguments which store the state of a mover (moving or not moving). 
    /// This is used when the mover state is changed and an event is raised to store in
    /// what kind of state the mover is currently in.
    /// </summary>
    public class MoverChangeArgs : System.EventArgs
    {
        public MoverState State;

        public MoverChangeArgs(MoverState state)
        {
            State = state;
        }
    }
}