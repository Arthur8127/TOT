using UnityEngine;
using UnityEngine.UI;

namespace BricksGame
{
    public class Item : MonoBehaviour
    {
        public Image image;
        public enum ItemType { Green = 0, Red = 1 }
        public ItemType itemType;
    }
}

