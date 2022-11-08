using System;
using UnityEngine;

namespace _src.Scripts.Grid {
    public class Tile : MonoBehaviour {
        public int x;
        public int y;

        public void Init(int xCord, int yCord){
            x = xCord;
            y = yCord;
        }
    }
}
