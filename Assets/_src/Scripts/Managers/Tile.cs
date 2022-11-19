using UnityEngine;

namespace _src.Scripts.Managers {
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
