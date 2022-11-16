using UnityEngine;

namespace _src.Scripts.Core {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T instance {
            get {
                if (_instance == null) {
                    var newObj = new GameObject {
                        name = "EventDispatcher"
                    };

                    _instance = newObj.AddComponent<T>();
                }

                return _instance;
            }

            private set { }
        }

        public static bool HasInstance() {
            return _instance != null;
        }

        private void Awake() {
            if (_instance != null && _instance.GetInstanceID() != GetInstanceID()) {
                Destroy(gameObject);
            }
            
            else _instance = this as T;
        }

        private void OnDestroy() {
            if (_instance == this) _instance = null;
        }
    }
}
