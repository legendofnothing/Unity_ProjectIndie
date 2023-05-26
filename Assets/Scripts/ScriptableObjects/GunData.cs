using System.Collections.Generic;
using Bullet;
using Scripts.Core;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "GunData", menuName = "GunData", order = 5)]
    public class GunData : ScriptableObject {
        public List<GunInfo> gunInfos;
    }
}