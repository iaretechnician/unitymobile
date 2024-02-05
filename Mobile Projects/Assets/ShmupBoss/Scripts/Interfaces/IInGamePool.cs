namespace ShmupBoss
{
    /// <summary>
    /// An interface to be used with classes you wish to track with "IInGamePoolManager".<br></br>
    /// Its events are not related to the actual pooling of agents, FX or pikcups and is instead for 
    /// keeping track of the active instances in the level.
    /// </summary>
    public interface IInGamePool
    {
        event CoreDelegate OnAddToInGamePool;
        event CoreDelegate OnRemoveFromInGamePool;
    }
}