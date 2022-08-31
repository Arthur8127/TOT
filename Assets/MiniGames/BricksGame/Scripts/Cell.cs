using UnityEngine;
using UnityEngine.EventSystems;


namespace BricksGame
{
    public class Cell : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
    {
        public GameManger gameManger;
        public Item currentItem;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentItem)
            {
                currentItem.image.enabled = false;
                gameManger.drag.image.enabled = true;
                gameManger.drag.image.sprite = currentItem.image.sprite;

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            gameManger.drag.transform.position = Input.mousePosition;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (currentItem == null)
            {
                Cell cell = eventData.pointerDrag.GetComponent<Cell>();
                if (cell)
                {
                    if (cell.currentItem)
                    {
                        currentItem = cell.currentItem;
                        cell.currentItem = null;
                        currentItem.transform.SetParent(transform);
                        currentItem.transform.localPosition = Vector2.zero;
                        currentItem.image.enabled = true;
                        gameManger.drag.image.enabled = false;
                        if (MatchController.instance)
                            MatchController.instance.PlaySound(gameManger.dropClip);


                    }
                }
            }



        }

        public void OnEndDrag(PointerEventData eventData)
        {
            gameManger.drag.image.enabled = false;
            if (currentItem)
            {
                currentItem.image.enabled = true;
                currentItem.transform.localPosition = Vector2.zero;
            }
        }
    }
}

