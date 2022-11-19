public enum EventType {
    None = 0,
    
    //Game State Events
    SwitchToShooting,
    SwitchToEnemy,
    SwitchToPlayer,
    
    //Player Script Events
    DisablePlayerInput, 
    EnablePlayerInput,
    
    //Bullet Manager Events
    AddBullet,
    
    //Pickup Manager Events
    SpawnPickup,
    
    //Enemy Manager Events
    EnemyTurn,
    EnemyKilled, 
    SpawnEnemy,
    EnemyDamagePlayer,
    
    //UI Events
    OnPlayerHPChange
}