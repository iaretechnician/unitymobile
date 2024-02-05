using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This interface is implemented to any mover that spawns from one of the sides of the playfield
    /// and is used to determine the initial direction of that mover.<br></br> 
    /// (i.e. if something is spawned from the top its initial direction will be down, if it is 
    /// spawned from the left, its initial direction will be to the right etc..) 
    /// </summary>
    public interface ISideSpawnable
    {
        FourDirection InitialDirection
        {
            get;
            set;
        }

        GameObject gameObject
        {
            get;
        }

        string name
        {
            get;
        }
    }
}