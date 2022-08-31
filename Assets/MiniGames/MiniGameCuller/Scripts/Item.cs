using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGameKuller
{
    public class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform rt;
        public float timerDeley;
        public Vector2[] randomPos;
        public Vector2 defPos;
        public GameManger gameManger;
        public void Show()
        {
            rt = (transform as RectTransform);
            defPos = rt.position;
            for (int i = 0; i < randomPos.Length; i++)
            {
                randomPos[i] = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(Anim());
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            rt.position = defPos;
            timerDeley = 0.5f;
        }
        private IEnumerator Anim()
        {

            while (timerDeley > 0)
            {
                yield return new WaitForSeconds(0.01f);
                rt.position = defPos + randomPos[Random.Range(0, randomPos.Length)];
                timerDeley -= 0.01f;
            }
            gameManger.RemoveItem(this);
        }
    }
}


