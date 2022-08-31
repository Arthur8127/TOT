using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BricksGame
{
    public class GameManger : MonoBehaviour
    {
        public bool isAcive = false, isComplite;
        public DragObject drag;
        public GameObject[] itemPrefabs;
        public List<Cell> mainCells;
        public List<Cell> leftCells;
        [Header("Combination")]
        public List<Combination> allCombinations;
        public int currentCombination;
        public Image displayImgage;
        public Sprite succsess, error;
        public Button btn;
        [Header("Sound")]
       
        public AudioClip dropClip;
        public AudioClip useBtn;
        public AudioClip errSound;
        
        [Header("Events")]
        public UnityEvent OnComplite;
        public UnityEvent OnExit;

        public void OnShow()
        {
            if (isAcive) return;
            isAcive = true;
            if (isComplite) return;
            SpawnItems();
        }
        private void Update()
        {
            if(isAcive && Input.GetKeyDown(KeyCode.Escape))
            {
                isAcive = false;
                OnExit.Invoke();
            }
        }
        public void SpawnItems()
        {
            // удаляем соществующие 
            for (int i = 0; i < leftCells.Count; i++)
            {
                if (leftCells[i].currentItem)
                {
                    Destroy(leftCells[i].currentItem.gameObject);
                    leftCells[i].currentItem = null;
                }
            }
            for (int i = 0; i < mainCells.Count; i++)
            {
                if (mainCells[i].currentItem)
                {
                    Destroy(mainCells[i].currentItem.gameObject);
                    mainCells[i].currentItem = null;
                }
            }

            // спавним новые
            currentCombination = Random.Range(0, allCombinations.Count);
            for (int i = 0; i < allCombinations[currentCombination].items.Count; i++)
            {
                Item item = Instantiate(allCombinations[currentCombination].items[i], mainCells[i].transform).GetComponent<Item>();
                mainCells[i].currentItem = item;
                item.transform.localPosition = Vector2.zero;

            }
            displayImgage.sprite = allCombinations[currentCombination].sprite;
            Randomiz();
        }
        public void Randomiz()
        {
            for (int i = 0; i < leftCells.Count; i++)
            {
                while (true)
                {
                    int randomIndex = Random.Range(0, mainCells.Count);
                    if (mainCells[randomIndex].currentItem)
                    {
                        mainCells[randomIndex].currentItem.transform.SetParent(leftCells[i].transform);
                        mainCells[randomIndex].currentItem.transform.localPosition = Vector2.zero;
                        leftCells[i].currentItem = mainCells[randomIndex].currentItem;
                        mainCells[randomIndex].currentItem = null;
                        break;
                    }
                    continue;
                }
            }
            btn.interactable = true;
        }
        public void CheckComplite()
        {
            MatchController.instance.PlaySound(useBtn);
            btn.interactable = false;
            for (int i = 0; i < mainCells.Count; i++)
            {
                if (!mainCells[i].currentItem || mainCells[i].currentItem.itemType != allCombinations[currentCombination].items[i].itemType)
                {
                    ShowError();
                    return;
                }
            }
            ShowComplite();

        }
        public void ShowError()
        {
            displayImgage.sprite = error;
            StartCoroutine(Waiting());
        }
        public void ShowComplite()
        {
            displayImgage.sprite = succsess;
            if (MatchController.instance)
                MatchController.instance.PlaySound(MatchController.instance.miniGameComplite);
            isComplite = true;
            OnComplite.Invoke();
        }
        private IEnumerator Waiting()
        {
            yield return new WaitForSeconds(0.5f);
            if (MatchController.instance)
                MatchController.instance.PlaySound(errSound);
            yield return new WaitForSeconds(0.5f);
            displayImgage.sprite = allCombinations[currentCombination].sprite;
            SpawnItems();
        }
    }
    [System.Serializable]
    public class Combination
    {
        public Sprite sprite;
        public List<Item> items;

    }
}

