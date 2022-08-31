using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace MiniGame2
{
    public class DropHole : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {

            Item item = eventData.pointerDrag.GetComponent<Item>();
            if (item)
            {
                GameManager.instance.dragObject.EndDrag();
                if (item.cell) item.cell.currentItem = null;
                Destroy(item.gameObject);
                if (item.InSlot == false)
                    GameManager.instance.spawner.SpawnPrefab();

            }
        }
    }

}


