using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniGame2
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public SpawnHole spawner;
        public DragObject dragObject;
        public Sprite[] capsulSprites;
        public Cell[] AllCells;
        public Image rDisplay;
        public Sprite spriteComplite;
        [Header("Events")]
        public UnityEvent OnComplite;
        public UnityEvent OnExit;
        public bool isAcive = false;
       
        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (isAcive && Input.GetKeyDown(KeyCode.Escape))
            {
                isAcive = false;
                OnExit.Invoke();
            }
        }

        public void OnStartMiniGame()
        {
            isAcive = true;

            for (int i = 0; i < AllCells.Length; i++)
            {
                if(AllCells[i].currentItem)
                {
                    Destroy(AllCells[i].currentItem.gameObject);
                    AllCells[i].currentItem = null;
                }
                Item item = CreateRandomItem(AllCells[i].transform);
                item.transform.SetSiblingIndex(0);
                item.InSlot = true;
                item.cell = AllCells[i];
                item.transform.localPosition = new Vector2(0, 66);
                AllCells[i].currentItem = item;
            }
        }
        public Item CreateRandomItem(Transform parrent)
        {
            Item item = Instantiate(spawner.itemPrefab, parrent).GetComponent<Item>();
            int random = Random.Range(0, 2);
            if (random == 1) item.isGreen = true;
            else item.isGreen = false;
            item.image.sprite = capsulSprites[random];
            return item;
        }

        public void CheckComplite()
        {
            for (int i = 0; i < AllCells.Length; i++)
            {
                if (AllCells[i].currentItem == null) return;
                if (AllCells[i].currentItem.isGreen == false) return;
            }
            rDisplay.sprite = spriteComplite;            
            OnComplite.Invoke();
            if (MatchController.instance)
                MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
        }
    }
}


