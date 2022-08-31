using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniGame_circle
{
    public class GameManager : MonoBehaviour
    {
        public List<Circle> circles = new List<Circle>();
        public UnityEvent OnCompline, OnHide;
        public bool isActive;
        private void Start()
        {
            Scatter();
        }
        public void Show()
        {
            isActive = true;
        }
        private void Update()
        {
            if(isActive && Input.GetKeyDown(KeyCode.Escape))
            {
                isActive = false;
                OnHide.Invoke();
            }
        }
        public void Scatter()
        {
            for (int i = 0; i < circles.Count; i++)
            {
                circles[i].Scatter();
            }
        }

        public void CheckWin()
        {   
            for (int i = 0; i < circles.Count; i++)
            {
                float angle = circles[0].transform.rotation.eulerAngles.z;
                if (circles[i].transform.rotation.eulerAngles.z != angle)
                {
                    return;
                }
            }
            OnCompline.Invoke();
            if (MatchController.instance)
                MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
        }
    }
}





