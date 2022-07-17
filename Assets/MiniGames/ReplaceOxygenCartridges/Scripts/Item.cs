using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace MiniGame2
{

    public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
    {
        public bool isGreen,InSlot = false;
        public Image image;
        public Cell cell = null;
        public void OnBeginDrag(PointerEventData eventData)
        {
            image.enabled = false;
            GameManager.instance.dragObject.BeginDrag(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
           GameManager.instance.dragObject.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            image.enabled = true;
            GameManager.instance.dragObject.EndDrag();
        }
    }
}


