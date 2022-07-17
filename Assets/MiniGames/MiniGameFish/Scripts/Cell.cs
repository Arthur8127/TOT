using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGameFish
{
    public class Cell : MonoBehaviour, IDropHandler
    {
        public Item currentItem = null;
        public void OnDrop(PointerEventData eventData)
        {
            
            if (currentItem) return;
            Item item = eventData.pointerDrag.GetComponent<Item>();
            if(item)
            {
                item.transform.SetParent(transform);
                currentItem = item;

                GameManager.instance.CheckComplite();
            }
        }
    }

}

