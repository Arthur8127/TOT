using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGameBur
{
    public class Crystall : MonoBehaviour
    {
        public Vector3[] path;
        public Image img;
        void Start()
        {
            StartCoroutine(Animation());
        }
        IEnumerator Animation()
        {
            yield return new WaitForSeconds(1f);
            while (transform.localPosition != path[0])
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, path[0], Time.deltaTime * 1000f);
                yield return null;
            }
            while (transform.localPosition != path[1])
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, path[1], Time.deltaTime * 350f);
                yield return null;
            }
            while (transform.localPosition != path[2])
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, path[2], Time.deltaTime * 350f);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}


