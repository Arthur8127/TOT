using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame2
{

    public class Cell : MonoBehaviour, IDropHandler
    {
        public Item currentItem = null;
        public void OnDrop(PointerEventData eventData)
        {
            if (!currentItem)
            {
                Item item = eventData.pointerDrag.GetComponent<Item>();
                if (item)
                {
                    if (item.InSlot == false)
                    {
                        GameManager.instance.spawner.SpawnPrefab();
                    }
                    item.transform.SetParent(transform);
                    item.transform.SetSiblingIndex(0);
                    item.transform.localPosition = new Vector2(0, 66);
                    item.InSlot = true;
                    if (item.cell)
                        item.cell.currentItem = null;
                    item.cell = this;
                    currentItem = item;


                }
            }
        }
    }

}
