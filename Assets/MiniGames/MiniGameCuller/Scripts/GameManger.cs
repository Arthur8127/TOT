using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MiniGameKuller
{
    public class GameManger : MonoBehaviour
    {
        public List<Item> items;
        public UnityEvent OnComplite, OnExit;
        public bool isActive;
        public void RemoveItem(Item item)
        {
            items.Remove(item);
            Destroy(item.gameObject);
            if (items.Count == 0)
            {
                OnComplite.Invoke();
                if (MatchController.instance)
                    MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
            }
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
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Show();
            }
            isActive = true;
        }
    }
}


