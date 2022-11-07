using UnityEngine;

namespace _src.Scripts.Core {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
        }
    }
}
