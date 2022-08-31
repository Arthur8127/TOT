using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame2
{
    public class DragObject : MonoBehaviour
    {
        public Image image;
        public Item dragItem;


        public void BeginDrag(Item item)
        {
            image.enabled = true;
            dragItem = item;
            image.sprite = item.image.sprite;
        }

        public void EndDrag()
        {
            dragItem = null;
            image.enabled = false;
        }


    }

   
}


