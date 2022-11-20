using System;
using System.Collections.Generic;
using _src.Scripts.Core.Collections;
using UnityEngine;

namespace _src.Scripts.Pickups.Bullets
{
    /// <summary>
    /// Spawns a bullet pickup
    /// </summary>
    public class PickupBulletSpawner : MonoBehaviour
    {
        [Serializable]
        public struct BulletData
        {
            public GameObject prefab;
            public float weight;
        }

        [Header("Bullet Spawn Configs")] [SerializeField]
        private List<BulletData> bulletDatas = new();

        private readonly WeightedList<BulletData> _weightedBulletList = new();

        private void Awake()
        {
            foreach (var bulletData in bulletDatas)
            {
                _weightedBulletList.AddElement(bulletData, bulletData.weight);
            }
        }

        private void Start()
        {
            var randomBulletPickup = _weightedBulletList.GetRandomItem().prefab;
            Instantiate(randomBulletPickup, transform.position, Quaternion.identity);
        }
    }
}