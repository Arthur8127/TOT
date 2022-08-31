using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniGameBur
{
    public class GameManager : MonoBehaviour
    {
        public Image progress;
        public List<Rock> rocks;
        public GameObject crystallPrefab;
        public List<Sprite> allCrystall;
        public int currentCrystall = 0;
        public RectTransform background;
        public Text infoText;
        public UnityEvent OnComplite, OnHide;
        public bool isActive;

       
        public void DragOff()
        {
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].anim.SetBool("action", false);
                rocks[i].timer = 1f;
            }
        }
        public void RemoveRock(Rock r)
        {
            progress.fillAmount += 0.25f;
            rocks.Remove(r);
            Crystall c = Instantiate(crystallPrefab, background).GetComponent<Crystall>();
            c.img.sprite = allCrystall[currentCrystall];
            c.img.SetNativeSize();
            c.transform.localPosition = r.transform.localPosition;
            currentCrystall++;
            Destroy(r.gameObject);
            infoText.text = currentCrystall + "/" + allCrystall.Count;
            CheckWin();
        }
        public void CheckWin()
        {
            if (rocks.Count == 0)
            {
                OnComplite.Invoke();
                if (MatchController.instance)
                    MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
            }
        }


        public void Show()
        {
            isActive = true;
            infoText.text = currentCrystall + "/" + allCrystall.Count;
        }

        private void Update()
        {
            if(isActive && Input.GetKeyDown(KeyCode.Escape))
            {
                isActive = false;
                OnHide.Invoke();
            }
        }
    }
}

