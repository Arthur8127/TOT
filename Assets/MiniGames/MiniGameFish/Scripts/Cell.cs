using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGameFish
{
    public class Cell : MonoBehaviour, IDropHandler
    {
        public Item currentItem = null;
        public GameManager gameManager;
        public void OnDrop(PointerEventData eventData)
        {
            
            if (currentItem) return;
            Item item = eventData.pointerDrag.GetComponent<Item>();
            if(item)
            {
                item.transform.SetParent(transform);
                currentItem = item;                
                gameManager.CheckComplite();
                for (int i = 0; i < gameManager.allCell.Count; i++)
                {
                    if(gameManager.allCell[i].currentItem)
                    {
                        gameManager.indicators[i].sprite = gameManager.greenFish;
                    }
                }
            }
        }
    }

}

