using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGameBur
{
    public class Rock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Animator anim;
        public float timer = 1f;
        public GameManager gameManager;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag)
            {
                StartCoroutine(ActionCar());
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag)
            {
                anim.SetBool("action", false);
                StopAllCoroutines();
                timer = 3f;
            }
        }
        private IEnumerator ActionCar()
        {
            anim.SetBool("action", true);
            while (timer > 0)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
            gameManager.RemoveRock(this);
        }
    }
}


