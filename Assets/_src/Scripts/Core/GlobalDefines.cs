using System;
using UnityEngine;

public class GlobalDefines {
    [Serializable]
    public struct SpawnData {
        public GameObject prefab;
        public float chance; 
    }
}