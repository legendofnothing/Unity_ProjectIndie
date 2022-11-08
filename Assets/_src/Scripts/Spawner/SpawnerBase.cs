using UnityEngine;

namespace _src.Scripts.Spawner {
    public class SpawnerBase : MonoBehaviour
    {
        public enum Type {
            Enemy,
            Pickup
        }

        public Type spawnerType;
        [Header("Enemy Prefabs")] public GameObject enemyPrefab;
    
        public void Spawn(){
            switch (spawnerType)
            {
                case Type.Enemy:
                    Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                    break;
                case Type.Pickup:
                    UnityEngine.Debug.Log("Spawn Pickup");
                    break;
            }
        }
    }
}
