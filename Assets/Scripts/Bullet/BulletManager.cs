using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Bullet;
using Scripts.Bullet.Types;
using Scripts.Core;
using Scripts.Core.EventDispatcher;
using UI.Components;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;
using Random = UnityEngine.Random;

namespace Bullet {
    [Serializable]
    public struct GunInfo {
        public Sprite gunSprite;
        public float offsetToPlayer;
        [Space] 
        public float baseAttack;
        public float baseFireRate;
        public int baseAmmoCount;
        [Space] 
        public string gunDescription;
    }
    
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
            UIStatic.FireUIEvent(TextUI.Type.AmmoCount, bulletList.Count);
            EventDispatcher.instance.SubscribeListener(EventType.BulletDestroyed, bullet => OnBulletDestroyed((GameObject) bullet));
        }

        private void OnBulletDestroyed(GameObject bullet) {
            _currentList.Remove(bullet);
            if (_currentList.Count > 0) return;

            bulletList.AddRange(_addedTempList);
            UIStatic.FireUIEvent(TextUI.Type.AmmoCount, bulletList.Count);
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
                    ? bulletComp.damage * bulletComp.damageModifier * 2 * _damageModifier
                    : bulletComp.damage * bulletComp.damageModifier * _damageModifier;

                if (bulletComp.specialTag == BulletSpecialTag.Homing) 
                    TargetingSystem.instance.AddHomingBullet((BulletHoming) bulletComp);
                
                yield return new WaitForSeconds(0.2f);
            }
            
            yield return null;
        }
        
        public void AddBullet(GameObject bullet) {
            if (_currentList.Count <= 0) {
                bulletList.Add(bullet);
                UIStatic.FireUIEvent(TextUI.Type.AmmoCount, bulletList.Count);
            }
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
