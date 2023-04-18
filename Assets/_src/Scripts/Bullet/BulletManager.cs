using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Bullet {
    /// <summary>
    /// Manager to handle bullets
    /// </summary>
    public class BulletManager : MonoBehaviour
    {
        public List<GameObject> bulletList;
        
        private const float BaseBulletDamageModifier = 1;
        private float _currentBulletDamageModifier; 

        //Holds current bullet in the scene
        private List<GameObject> _currentList;
        //Store any new bullets being added
        private List<GameObject> _addedTempList;

        private bool _canSwitchTurn;
        private void Awake(){
            _currentList = new List<GameObject>();
            _addedTempList = new List<GameObject>();

            _currentBulletDamageModifier = BaseBulletDamageModifier;
        }

        private void Update(){
            //Remove all destroyed bullet in the currentList
            if (IsAllBulletsActive())
            {
                _currentList?.RemoveAll(destroyedBullet => destroyedBullet == null);
            }
            
            //Switch Turn when all bullet in the scene is destroyed
            if (_canSwitchTurn && !IsAllBulletsActive())
            {
                _canSwitchTurn = false;
                
                bulletList.AddRange(_addedTempList);
                _addedTempList.Clear();
                
                //Reset Modifier
                _currentBulletDamageModifier = BaseBulletDamageModifier;
                
                //Switch to Enemy Turn
                this.SendMessage(EventType.SwitchToEnemy);
            }
        }
        
        private bool IsAllBulletsActive(){
            return _currentList.Count > 0;
        }
        
        //Spawn Bullet, called in PlayerController
        public IEnumerator SpawnBullet(Vector3 position, Quaternion rotation)
        {
            //Instantiate each bullet in the bulletList
            foreach (var bulletInst in bulletList.Select(bullet => Instantiate(bullet, position, rotation)))
            {
                //Add all instantiated bullet into the currentList
                _currentList.Add(bulletInst);
                
                //Set Bullet Damage w/ any modifiers
                var bulletDamage = bulletInst.GetComponent<BulletBase>().damage;
                bulletInst.GetComponent<BulletBase>().damage = bulletDamage * _currentBulletDamageModifier;
                
                yield return new WaitForSeconds(0.2f);
            }
            
            _canSwitchTurn = true;
            yield return null;
        }
        
        public void AddBullet(GameObject bullet) { 
            _addedTempList.Add(bullet);
        }

        public void ChangeDamageModifier(float amount) {
            _currentBulletDamageModifier = amount;
        }
    }
}
