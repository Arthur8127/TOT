using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGameLines
{
    public class GameManager : MonoBehaviour
    {
        public List<Conector> conectors;
        public UnityEvent OnComplite, OnExit;
        public bool isActive;
        public void CheckWin()
        {
            for (int i = 0; i < conectors.Count; i++)
            {
                if (!conectors[i].isDone) return;
            }
            OnComplite.Invoke();
            if (MatchController.instance)
                MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
        }
        private void Update()
        {
            if(isActive && Input.GetKeyDown(KeyCode.Escape))
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


