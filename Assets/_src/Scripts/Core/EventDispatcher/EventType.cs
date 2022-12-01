public enum EventType {
    None = 0,
    
    //Game State Events
    SwitchToShooting,
    SwitchToEnemy,
    SwitchToPlayer,
    
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
    
    //UI Events
    OnPlayerHpChange,

    //PowerUp Events
    PowerupHealth,
    PowerupDamageBuff,
    PowerupExplosion,
    PowerupNuke
}