using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public class BulletManager : MonoBehaviour
    {
        [Header("Configs")]
        public int amount;
        public GameObject bulletPrefab;
        public List<GameObject> bulletList;
        
        //Holds current bullet in the scene
        private List<GameObject> _currentList;

        //Store any new bullets being added
        private List<GameObject> _addedTempList;

        private bool _canSwitchTurn;

        private void Awake(){
            _currentList = new List<GameObject>();
            _addedTempList = new List<GameObject>();

            foreach (var unused in bulletList)
            {
                amount++;
            }
        }

        private void Start()
        {
            this.SubscribeListener(EventType.AddBullet, _=>AddBullet());
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

            if (_currentList != null)
            {
                bulletList.AddRange(_addedTempList);
                _addedTempList.Clear();
            }
            yield return null;
        }

        private bool IsAllBulletsActive(){
            return _currentList.Count > 0;
        }

        //Use for bullet pickups
        private void AddBullet(){
            amount++;
            _addedTempList.Add(bulletPrefab);
        }
    }
}
