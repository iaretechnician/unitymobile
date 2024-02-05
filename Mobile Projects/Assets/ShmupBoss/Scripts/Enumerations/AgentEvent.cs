namespace ShmupBoss
{
    // Note: The reason for breaking my enumerator convention of including each enumerator in a different
    // file in this instance is because the player and enemy events are actually agent events which are translated. 
    // The player and enemy events are merely a reduced form of the agent event so that it is more 
    // convenient for the user to see only the events which are related to the agent he/she are using. 
    // These events are closely associated with the AgentEventTranslator.cs script. Any modifications here
    // needs to be done as well in the AgentEventTranslator.cs script or you will get translation errors.

    /// <summary>
    /// <para>Names of events which are initiated by all the different agents (Player, Enemy, Mines, missiles, etc..).
    /// These are never selected directly by the user and are instead translated from their respective
    /// Player or Enemy event names. This approach has been taken to enable the ease of use of a unified
    /// agent driven events and to make the user only select events which are relavent to the type of agent
    /// he/she are using.
    /// If you plan to add your own events, you will need to add an enum here, and the corresponding enum
    /// in the player or enemy agent and to also add the translation in the AgentEventTranslator.cs.
    /// I know this is quite a loop but it really has the added advantages of sharing events yet at the same time
    /// not confusing the user with events that are not used depending on the type of agent.
    /// These include the following: </para>
    /// <br>Activation</br>
    /// <br>Elimination</br>
    /// <br>TakeHit</br>
    /// <br>HealthDamage</br>
    /// <br>ShieldDamage</br>
    /// <br>TakeCollision</br>
    /// <br>DealCollision</br>
    /// <br>InvincibilityStart</br>
    /// <br>InvincibilityEnd</br>
    /// <br>PickUp</br>
    /// <br>Heal</br>
    /// <br>HealHealth</br>
    /// <br>HealShield</br>
    /// <br>HealthUpgrade</br>
    /// <br>ShieldUpgrade</br>
    /// <br>InvincibilityPickup</br>
    /// <br>LivesPickup</br>
    /// <br>WeaponUpgrade</br>
    /// <br>WeaponDowngrade</br>
    /// <br>VisualUpgrade</br>
    /// <br>VisualDowngrade</br>
    /// <br>SpeedChange</br>
    /// <br>PointPickup</br>
    /// <br>CoinPickup</br>
    /// <br>Drop</br>
    /// <br>DetonationStart</br>
    /// <br>BombsPickup</br>
    /// <br>FiringBomb</br>
    /// <br>TranslationError</br>
    /// </summary>
    public enum AgentEvent
    {
        Activation,
        Elimination,
        TakeHit,
        HealthDamage,
        ShieldDamage,
        TakeCollision,
        DealCollision,
        InvincibilityStart,
        InvincibilityEnd,
        PickUp,
        Heal,
        HealHealth,
        HealShield,
        HealthUpgrade,
        ShieldUpgrade,
        InvincibilityPickup,
        LivesPickup,
        WeaponsUpgrade,
        WeaponsDowngrade,
        VisualUpgrade,
        VisualDowngrade,
        SpeedChange,
        PointPickup,
        CoinPickup,
        Drop,       
        DetonationStart,
        BombsPickup,
        FiringBomb,
        TranslationError
    }

    /// <summary>
    /// <para>Names of events which are initiated by the Player. These are translated into an AgentEvent name
    /// through the AgentEventTranslator.
    /// They are used with components such as an FX spawner, Flash FX, SFX, etc..
    /// These include the following: </para>
    /// <br>Activation</br>
    /// <br>Elimination</br>
    /// <br>TakeHit</br>
    /// <br>HealthDamage</br>
    /// <br>ShieldDamage</br>
    /// <br>TakeCollision</br>
    /// <br>DealCollision</br>
    /// <br>InvincibilityStart</br>
    /// <br>InvincibilityEnd</br>
    /// <br>PickUp</br>
    /// <br>Heal</br>
    /// <br>HealHealth</br>
    /// <br>HealShield</br>
    /// <br>HealthUpgrade</br>
    /// <br>ShieldUpgrade</br>
    /// <br>InvincibilityPickup</br>
    /// <br>LivesPickup</br>
    /// <br>WeaponUpgrade</br>
    /// <br>WeaponDowngrade</br>
    /// <br>VisualUpgrade</br>
    /// <br>VisualDowngrade</br>
    /// <br>SpeedChange</br>
    /// <br>PointPickup</br>
    /// <br>CoinPickup</br>
    /// <br>BombsPickup</br>
    /// <br>FiringBomb</br>
    /// </summary>
    public enum PlayerEvent
    {
        Activation,
        Elimination,
        TakeHit,
        HealthDamage,
        ShieldDamage,
        TakeCollision,
        DealCollision,
        InvincibilityStart,
        InvincibilityEnd,
        PickUp,
        Heal,
        HealHealth,
        HealShield,
        HealthUpgrade,
        ShieldUpgrade,
        InvincibilityPickup,
        LivesPickup,
        WeaponsUpgrade,
        WeaponsDowngrade,
        VisualUpgrade,
        VisualDowngrade,
        SpeedChange,
        PointPickup,
        CoinPickup,
        BombsPickup,
        FiringBomb
    }

    /// <summary>
    /// <para>Names of events which are initiated by the Enemy and any enemy derived classes. 
    /// These are translated into an AgentEvent name through the AgentEventTranslator.
    /// They are used with components such as an FX spawner, Flash FX, SFX, etc..
    /// These include the following: </para>
    /// <br>Activation</br>
    /// <br>Elimination</br>
    /// <br>TakeHit</br>
    /// <br>HealthDamage</br>
    /// <br>ShieldDamage</br>
    /// <br>TakeCollision</br>
    /// <br>DealCollision</br>
    /// <br>InvincibilityStart</br>
    /// <br>InvincibilityEnd</br>
    /// <br>Drop</br>
    /// <br>DetonationStart</br>
    /// </summary>
    public enum EnemyEvent
    {
        Activation,
        Elimination,
        TakeHit,
        HealthDamage,
        ShieldDamage,
        TakeCollision,
        DealCollision,
        InvincibilityStart,
        InvincibilityEnd,
        Drop,       
        DetonationStart
    }
}