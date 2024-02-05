using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Component for playing sound effects when enemy events occur.<br></br>
    /// The reason this class basically only passes the type of audio clip is to make sure the list of 
    /// events the user sees are only the ones of the agent being used (either player or enemy.)
    /// </summary>
    /// <typeparam name="T">Audio clips which are triggered by enemy events.</typeparam>
    [AddComponentMenu("Shmup Boss/FX/Enemy/SFX Enemy")]
    [RequireComponent(typeof(Enemy))]
    public class SfxEnemy : Sfx<volumetricACTriggeredByEnemyEvent>
    {
        protected override void PrintAgentNullMessage()
        {
            Debug.Log("SfxPlayer.cs: a SFX Enemy has been added to a game object which isn't an enemy. " +
                "SFX Enemy is only applied to game objects with the enemy component or something that" +
                "inherits from an enemy.");
        }
    }
}

