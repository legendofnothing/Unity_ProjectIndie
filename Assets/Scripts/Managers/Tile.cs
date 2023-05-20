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

        public void UpdateTile(Contains type) {
            contains = type;
        }
    }
}
