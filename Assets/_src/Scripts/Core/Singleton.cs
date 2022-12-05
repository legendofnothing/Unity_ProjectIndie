using UnityEngine;

namespace _src.Scripts.Core {
    /// <summary>
    /// Singleton Pattern
    /// Example Usage: ClassName : Singleton<ClassName/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //Private instance to keep track
        private static T _instance;
        
        //Public instance to be called
        public static T instance {
            get {
                
                //Create a new instance if no instance of the object existed, else return the one already exist
                //This is to make sure there's only 1 instance per singleton objects
                if (_instance == null) {
                    var newObj = new GameObject {
                        name = "EventDispatcher" //What with the name, just gonna add TODO: Change Name For Singleton Objects
                    };

                    _instance = newObj.AddComponent<T>();
                }

                return _instance;
            }

            private set { }
        }
        
        /// <summary>
        /// Check if any instance exists 
        /// </summary>
        /// <returns>True if any</returns>
        public static bool HasInstance() {
            return _instance != null;
        }

        private void Awake() {
            //Destroy any previous instances
            if (_instance != null && _instance.GetInstanceID() != GetInstanceID()) {
                Destroy(gameObject);
            }
            
            //Assign new instance to this object
            else _instance = this as T;
        }
        
        private void OnDestroy() {
            //Destroy instance if the object gets destroyed
            if (_instance == this) _instance = null;
        }
    }
}
