using System;
using UnityEngine;

namespace _src.Scripts.Bullet {
    public enum Type {
        Basic
    }
    
    public class BulletBase : MonoBehaviour {
        [Serializable]
        public struct Bullet {
            public float damage;
            public float bounceTimes;
            public Type type;
        }

        public Bullet bullet; 
        
        private void Start(){
            throw new NotImplementedException();
        }
    }
}
