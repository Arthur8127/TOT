using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace MiniGameFish
{
    public class GameManager : MonoBehaviour
    {

        public Transform spawnPoint;
        public List<Cell> allCell;
        public List<Sprite> allFish;
        public GameObject itemPrefab;
        public int currentIdnex;
        public UnityEvent OnComplite, OnHide;
        public bool isActive;
        public Sprite greenFish;
        public Image[] indicators;

        public void Show()
        {
            isActive = true;
            CheckComplite();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isActive)
            {
                isActive = false;
                OnHide.Invoke();
            }
        }
        public void CheckComplite()
        {
            if (currentIdnex >= allFish.Count)
                return;
            else
            {
                SpawnItem();
                
            }


        }
        public void SpawnItem()
        {
            Item item = Instantiate(itemPrefab, spawnPoint).GetComponent<Item>();
            item.gameManager = this;
            item.Setup(currentIdnex);
            item.transform.localPosition = Vector3.up * 100f;
            currentIdnex++;

        }
        

        public void UseButtnComplite()
        {
            if (currentIdnex >= allFish.Count)
            {
                OnComplite.Invoke();
                if (MatchController.instance)
                    MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
            }
        }
    }
}

