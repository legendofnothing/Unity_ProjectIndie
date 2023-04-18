using System;
using UnityEngine;

namespace _src.Scripts.Core {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        public static T instance {
            get {
                if (_instance == null) {
                    var instances = FindObjectsOfType<T>();
                    if (instances.Length > 1) {
                        _instance = instances[0];
                    }
                }

                if (_instance == null) {
                    _instance = new GameObject($"{typeof(T)} (singleton)").AddComponent<T>();
                }
                    
                return _instance; 
            }
        }
    }

    public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        public static T Instance { get; private set; }
	
        public virtual void Awake ()
        {
            if (Instance == null) {
                Instance = this as T;
                DontDestroyOnLoad (this);
            } else {
                Destroy (gameObject);
            }
        }
    }
}
