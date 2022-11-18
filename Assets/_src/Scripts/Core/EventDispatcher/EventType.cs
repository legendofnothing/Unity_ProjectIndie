public enum EventType {
    None = 0,
    
    //Game State Events
    SwitchToShooting,
    SwitchToEnemy,
    SwitchToPlayer,
    
    //Player Script Events
    DisablePlayerInput, 
    EnablePlayerInput,
    
    //Enemy Manager Events
    EnemyTurn,
    EnemyKilled, 
    SpawnEnemy,
    EnemyDamagePlayer 
}