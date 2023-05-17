using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "PickupBulletSpawningData", menuName = "Pickup/SpawningDataBullet", order = 4)]
    public class PickupBulletSpawningData : ScriptableObject {
        public List<GlobalDefines.SpawnData> pickupData;
    }
}