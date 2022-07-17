using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame2
{
    public class SpawnHole : MonoBehaviour
    {
        public GameObject itemPrefab;
        public RectTransform content;
        public Vector3 readyPos, spawnPos;
        public void SpawnPrefab()
        {

            Item item = GameManager.instance.CreateRandomItem(content);
            StartCoroutine(ItemAnim(item));
        }

        IEnumerator ItemAnim(Item item)
        {
            item.transform.localPosition = spawnPos;
            while (item.transform.localPosition != readyPos)
            {
                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, readyPos, Time.deltaTime * 900f);
                yield return null;
            }
        }
    }
}


