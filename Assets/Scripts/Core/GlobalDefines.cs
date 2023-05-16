using System;
using UnityEngine;

namespace Scripts.Core {
    public class GlobalDefines {
        [Serializable]
        public struct SpawnData {
            public GameObject prefab;
            public float chance; 
        }
    }

    public class FloatPair {
        public float float1;
        public float float2;

        public FloatPair(float float1, float float2) {
            this.float1 = float1;
            this.float2 = float2;
        }
    }
}