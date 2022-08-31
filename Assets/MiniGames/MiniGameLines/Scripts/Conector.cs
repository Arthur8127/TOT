using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geniikw.DataRenderer2D;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MiniGameLines

{
    public class Conector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public UILine line;
        public RectTransform rtPoint;
        public enum ConnectorColor { Red = 0, Green = 1, Blue = 2, Purpure = 3, Orange = 4 }
        public ConnectorColor connectorColor;
        public bool isDone, isRight;
        
        
        public GameManager gameManager;

        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isRight|| isDone) return;           
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (isDone || isRight) return;
            Point p = new Point();
            p.width = 7;
            p.position = eventData.position;
            line.line.EditPoint(0, p);
        }
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag && isRight)
            {
                Conector con = eventData.pointerDrag.GetComponent<Conector>();
                if (con)
                {
                    if (con.connectorColor == connectorColor)
                    {
                        con.isDone = true;
                        isDone = true;
                        Point p = new Point();
                        p.width = 7;
                        p.position = rtPoint.position;
                        con.line.line.EditPoint(0, p);
                        gameManager.CheckWin();
                    }
                    
                }
                
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDone)
            {
                Point p = new Point();
                p.width = 7;
                p.position = rtPoint.position;
                line.line.EditPoint(0, p);
                
            }

        }
    }

}



