using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Bullet.Types;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;
using Random = UnityEngine.Random;

namespace Scripts.Bullet {
    /// <summary>
    /// Manager to handle bullets
    /// </summary>
    public class BulletManager : Singleton<BulletManager>
    {
        public List<GameObject> bulletList;

        private float _critChance; 
        private float _damageModifier; 
        
        //Holds current bullet in the scene
        private List<GameObject> _currentList;
        //Store any new bullets being added
        private List<GameObject> _addedTempList;

        private void Awake(){
            _currentList = new List<GameObject>();
            _addedTempList = new List<GameObject>();
        }

        private void Start() {
            EventDispatcher.instance.SubscribeListener(EventType.BulletDestroyed, bullet => OnBulletDestroyed((GameObject) bullet));
        }

        private void OnBulletDestroyed(GameObject bullet) {
            _currentList.Remove(bullet);
            if (_currentList.Count > 0) return;

            bulletList.AddRange(_addedTempList);
            _addedTempList.Clear();

            //Switch to Enemy Turn
            EventDispatcher.instance.SendMessage(EventType.SwitchToEnemy);
        }
        
        
        //Spawn Bullet, called in PlayerController
        public IEnumerator SpawnBullet(Vector3 position, Quaternion rotation)
        {
            //Instantiate each bullet in the bulletList
            foreach (var bulletInst in bulletList.Select(bullet => Instantiate(bullet, position, rotation))) {
                //Add all instantiated bullet into the currentList
                _currentList.Add(bulletInst);
                
                //Set Bullet Damage w/ any modifiers
                var bulletComp = bulletInst.GetComponent<BulletBase>();
                bulletComp.damage = Random.Range(0f, 1f) < _critChance
                    ? bulletComp.damage * 2 * _damageModifier
                    : bulletComp.damage * _damageModifier;

                if (bulletComp.specialTag == BulletSpecialTag.Homing) 
                    TargetingSystem.instance.AddHomingBullet((BulletHoming) bulletComp);
                
                yield return new WaitForSeconds(0.05f);
            }
            
            yield return null;
        }
        
        public void AddBullet(GameObject bullet) { 
            if (_currentList.Count <= 0) bulletList.Add(bullet);
            else _addedTempList.Add(bullet);
        }
        
        public void AddBulletOnScene(GameObject bullet) { 
            _currentList.Add(bullet);
        }

        public void ChangeDamageModifier(float amount) {
            _damageModifier = amount;
        }
        
        public void ChangeCritModifier(float percent) {
            _critChance = percent;
        }
    }
}
