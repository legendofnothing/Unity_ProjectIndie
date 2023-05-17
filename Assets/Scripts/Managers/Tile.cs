using UnityEngine;

namespace Managers {
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
        
        public void Init(Sprite sprite, int xCord, int yCord, Contains type) {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
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
