using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame27
{
    public class Slot : MonoBehaviour, IDropHandler
    {
        public int id;
        public GameManager gameManager;

        public void OnDrop(PointerEventData eventData)
        {
            Item item = eventData.pointerDrag.gameObject.GetComponent<Item>();
            if (item)
            {
                if (item.id == id)
                {
                    item.enabled = false;
                    item.transform.SetParent(transform);
                    item.transform.localPosition = Vector2.zero;
                    item.isDone = true;
                    gameManager.CheckWin();
                }
            }
        }
    }
}


