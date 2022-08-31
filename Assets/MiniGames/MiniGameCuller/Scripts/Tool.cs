using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MiniGameKuller
{
    public class Tool : MonoBehaviour //IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Vector2 defaultPos;
        public Transform t;
        private Image img;
        public GameManger gm;
        private void Start()
        {
            t = transform;
            defaultPos = transform.position;
            img = GetComponent<Image>();
        }

        private void Update()
        {
            if(gm.isActive)
            {
                t.position = Input.mousePosition;
            }
        }

        //public void OnBeginDrag(PointerEventData eventData)
        //{
        //    img.raycastTarget = false;
        //}
        //public void OnDrag(PointerEventData eventData)
        //{
        //    t.position = eventData.position;
        //}
        //public void OnEndDrag(PointerEventData eventData)
        //{
        //    t.position = defaultPos;
        //    img.raycastTarget = true;
        //}
    }
}

