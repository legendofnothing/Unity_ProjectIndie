using System;
using _src.Scripts.Core;
using _src.Scripts.Enemy;
using UnityEngine;

namespace _src.Scripts.Grid {
    public class Tile : MonoBehaviour {
        public int x;
        public int y;

        [Space] public LayerMask enemyLayer;
        
        public void Init(int xCord, int yCord){
            x = xCord;
            y = yCord;
        }
    }
}
