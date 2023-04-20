using System.Collections.Generic;
using _src.Scripts.Managers;
using UnityEngine;

namespace _src.Scripts.ScriptableObjects {
    [CreateAssetMenu(fileName = "PickupBulletSpawningData", menuName = "Pickup/SpawningDataBullet", order = 4)]
    public class PickupBulletSpawningData : ScriptableObject {
        public List<GlobalDefines.SpawnData> pickupData;
    }
}