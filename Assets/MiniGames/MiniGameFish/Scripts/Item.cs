using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace MiniGameFish
{
    public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image image;
        Vector2 oldPos;
        Transform parent;
        public GameManager gameManager;
        public void Setup(int id)
        {
            image.sprite = gameManager.allFish[id];
            parent = transform.parent;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            oldPos = transform.localPosition;
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = Vector2.up * 100f;
            if (transform.parent == parent)
                image.raycastTarget = true;
        }
    }
}


