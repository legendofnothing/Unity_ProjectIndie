/// <summary>
/// Store EventTypes here, foreach new events create an enum here
/// </summary>
public enum EventType {
    None = 0,
    
    //Game State Events
    SwitchToShooting,
    SwitchToEnemy,
    SwitchToPlayer,
    SwitchToEnd,
    
    //Player Script Events
    DisablePlayerInput, 
    EnablePlayerInput,
    AddPlayerHealth,

    //Bullet Manager Events
    AddBullet,
    BuffBullet,
    
    //Pickup Manager Events
    SpawnPickup,

    //Enemy Manager Events
    EnemyTurn,
    EnemyKilled, 
    SpawnEnemy,
    EnemyDamagePlayer,
    
    //Score Manager Events
    AddScore,
    OnScoreChange,
    
    //UI Events
    OnPlayerHpChange,
    OnPlayerDie,
    OnTurnNumberChange,
    
    OnPlayerCoinChange,
    OnPlayerCoinAdd,
    OnPlayerCoinReduce,

    //PowerUp Events
    PowerupHealth,
    PowerupDamageBuff,
    PowerupExplosion,
    PowerupNuke
}