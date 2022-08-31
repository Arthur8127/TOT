using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGameBur
{
    public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Animator anim;
        public List<Image> imgRayCast;
        public float minX, maxX, minY, maxY;
        public GameManager gameManager;
        public void OnBeginDrag(PointerEventData eventData)
        {
            anim.SetBool("IsWork", true);
            for (int i = 0; i < imgRayCast.Count; i++) imgRayCast[i].raycastTarget = false;

        }
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position+Vector2.up * 100f;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            anim.SetBool("IsWork", false);
            for (int i = 0; i < imgRayCast.Count; i++) imgRayCast[i].raycastTarget = true;
            gameManager.DragOff();
        }
    }
}


