using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class uiBtnCircle : MonoBehaviour
{
    public Vector2 defPos, pressPos;
    private RectTransform txt;
    private void Start()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>().rectTransform;
    }

    public void OnDown()
    {
        txt.localPosition = pressPos;
    }
    public void OnUp()
    {
        txt.localPosition = defPos;
    }
}
