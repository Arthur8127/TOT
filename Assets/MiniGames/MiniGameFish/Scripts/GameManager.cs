using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGameFish
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public Transform spawnPoint;
        public List<Cell> allCell;
        public List<Sprite> allFish;
        public GameObject itemPrefab;
        public int currentIdnex;
        public UnityEvent OnCompliteEvent;
        public UnityEvent OnExitEvent;
        private bool isOpen = false;
        public Windows windows;
        private void Awake()
        {
            instance = this;
        }

        public void Open()
        {
            isOpen = true;
            CheckComplite();
        }
        public void Close()
        {
            isOpen = false;
            windows.Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                Close();
            }
        }
        public void CheckComplite()
        {
            if (currentIdnex >= allFish.Count)
                return;
            else
                SpawnItem();
        }
        public void SpawnItem()
        {
            Item item = Instantiate(itemPrefab, spawnPoint).GetComponent<Item>();
            item.Setup(currentIdnex);
            item.transform.localPosition = Vector3.up * 100f;
            currentIdnex++;

        }
        public void OnComplite()
        {
            OnCompliteEvent.Invoke();
        }

        public void UseButtnComplite()
        {
            if (currentIdnex >= allFish.Count)
            {
                OnComplite();
            }
        }
    }
}

