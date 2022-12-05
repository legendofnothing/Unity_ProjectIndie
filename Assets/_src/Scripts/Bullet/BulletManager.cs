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
        [Header("Configs")]
        public int amount;
        public GameObject bulletPrefab;
        
        //Store the bullets that will be fired
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
            
            //Keep track of amount 
            foreach (var unused in bulletList)
            {
                amount++;
            }

            _currentBulletDamageModifier = BaseBulletDamageModifier;
        }

        private void Start()
        {
            //Listen to Event: AddBullet
            this.SubscribeListener(EventType.AddBullet, bullet=>AddBullet((GameObject) bullet));
            
            //Listen to Event: BuffBullet (Change Damage Modifier)
            this.SubscribeListener(EventType.BuffBullet, param => ChangeDamageModifier((float) param));
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
        
        /// <summary>
        /// Check if there's any active bullet left in the scene 
        /// </summary>
        /// <returns>True if have any</returns>
        private bool IsAllBulletsActive(){
            return _currentList.Count > 0;
        }

        /// <summary>
        /// Add bullet of type
        /// </summary>
        /// <param name="bullet">Added Bullet</param>
        private void AddBullet(GameObject bullet){
            amount++;
            _addedTempList.Add(bullet);
        }

        private void ChangeDamageModifier(float amount) {
            _currentBulletDamageModifier = amount;
        }
    }
}
