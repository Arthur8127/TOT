using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MiniGame27
{

    public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int id;
        private Vector2 startPos;
        public Image image;
        public bool isDone;
        public void OnBeginDrag(PointerEventData eventData)
        {
            startPos = transform.position;
            image.raycastTarget = false;

        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }



        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = startPos;
            image.raycastTarget = true;
        }
    }

}

