using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGame27
{
    public class GameManager : MonoBehaviour
    {

        public List<Item> items = new List<Item>();
        public UnityEvent OnCompline, OnExit;
        public bool isActive;

        public void CheckWin()
        {
            foreach (var item in items)
            {
                if (!item.isDone) return;
            }
            OnCompline.Invoke();
            if (MatchController.instance)
                MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
        }
        private void Update()
        {
            if (isActive && Input.GetKeyDown(KeyCode.Escape))
            {
                isActive = false;
                OnExit.Invoke();
            }
        }
        public void Show()
        {
            isActive = true;
        }

    }
}


