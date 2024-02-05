using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Component for playing sound effects when player events occur.<br></br>
    /// The reason this class basically only passes the type of audio clip is to make sure the list of 
    /// events the user sees are only the ones of the agent being used (either player or enemy.)
    /// </summary>
    /// <typeparam name="T">Audio clips which are triggered by player events.</typeparam>
    [AddComponentMenu("Shmup Boss/FX/Player/SFX Player")]
    [RequireComponent(typeof(Player))]
    public class SfxPlayer : Sfx<volumetricACTriggeredByPlayerEvent>
    {
        protected override void PrintAgentNullMessage()
        {
            Debug.Log("SfxPlayer.cs: a SFX Player has been added to a game object which isn't a player. " +
                "SFX player is only applied to game objects with the Player component.");
        }
    }
}