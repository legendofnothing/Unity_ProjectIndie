using System;
using System.Collections.Generic;
using UI.InGame;
using UnityEngine;

namespace ScriptableObjects {
    [Serializable]
    public struct ShopItemSO {
        public float chanceToAppear;
        public ShopItem shopItem;
    }
    
    [CreateAssetMenu(fileName = "ShopData", menuName = "ShopData", order = 5)]
    public class ShopItemData : ScriptableObject {
        public List<ShopItemSO> shopItems;
    }
}