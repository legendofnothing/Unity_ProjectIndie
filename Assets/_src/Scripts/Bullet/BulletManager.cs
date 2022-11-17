using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using _src.Scripts.Grid;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public class BulletManager : MonoBehaviour
    {
        [Header("Configs")]
        public int amount;
        public GameObject bulletPrefab;
        public List<GameObject> bulletList;
        private List<GameObject> _currentList;

        private bool _canSwitchTurn = false; 

        private void Awake(){
            _currentList = new List<GameObject>();
            amount = bulletList.Count;
        }
        
        private void Update(){
            if (IsAllBulletsActive())
            {
                _currentList?.RemoveAll(destroyedBullet => destroyedBullet == null);
            }
            
            if (_canSwitchTurn && !IsAllBulletsActive())
            {
                _canSwitchTurn = false;
                this.SendMessage(EventType.SwitchToEnemy);
            }
        }

        public IEnumerator SpawnBullet(Vector3 position, Quaternion rotation)
        {
            foreach (var bulletInst in bulletList.Select(bullet => Instantiate(bullet, position, rotation)))
            {
                _currentList.Add(bulletInst);
                yield return new WaitForSeconds(0.2f);
            }
            
            _canSwitchTurn = true;
            
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
