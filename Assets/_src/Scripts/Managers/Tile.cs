using UnityEngine;

namespace _src.Scripts.Managers {
    public enum Contains
    {
        None,
        Enemy,
        Pickup
    }
    
    public class Tile : MonoBehaviour {
        public int x;
        public int y;
        public Contains contains;
        
        public void Init(int xCord, int yCord, Contains type){
            x = xCord;
            y = yCord;
            contains = type;
        }
        
        /// <summary>
        /// Update which content this Tile is holding
        /// </summary>
        /// <param name="type">Contain Type</param>
        public void UpdateTile(Contains type)
        {
            contains = type;
        }
    }
}
