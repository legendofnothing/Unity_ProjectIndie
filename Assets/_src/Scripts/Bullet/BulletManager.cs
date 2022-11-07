using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public class BulletManager : MonoBehaviour {
        public int amount;
        public GameObject bulletPrefab;
        public List<GameObject> bulletList;
        private List<GameObject> _currentList;

        private void Awake(){
            _currentList = new List<GameObject>();
        }

        private void Update(){
            _currentList?.RemoveAll(destroyedBullet => destroyedBullet == null);
        }

        public IEnumerator SpawnBullet(Vector3 position, Quaternion rotation){
            foreach (var bulletInst in bulletList.Select(bullet => Instantiate(bullet, position, rotation)))
            {
                _currentList.Add(bulletInst);
                yield return new WaitForSeconds(0.2f);
            }

            yield return null;
        }

        public bool IsAllBulletsActive(){
            return _currentList.Count > 0;
        }

        //Use for bullet pickups
        public void AddBullet(GameObject type){
            amount++;
            bulletList.Add(type);
        }
    }
}
