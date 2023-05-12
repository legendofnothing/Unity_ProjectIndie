/// <summary>
/// Store EventTypes here, foreach new events create an enum here
/// </summary>
public enum EventType {
    None = 0,
    
    //Game State Events
    SwitchToShooting,
    SwitchToEnemy,
    SwitchToShop,
    SwitchToPlayer,
    SwitchToEnd,
    
    //Pickup Manager Events
    SpawnPickup,

    //Enemy Manager Events
    EnemyTurn,
    EnemyKilled, 
    SpawnEnemy,
    
    //Bullet Events
    BulletDestroyed,

    //UI Events
    OnInitUI,
    OnPlayerHpChange,
    OnPlayerDie,
    OnTurnNumberChange,
    OnScoreChange,
    OnPlayerCoinChange,
}