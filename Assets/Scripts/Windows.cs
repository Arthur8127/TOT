using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class Windows : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI textContent;
    public UnityEvent OnShow, OnHide;

    public void Show()
    {        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        OnShow.Invoke();
        
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        OnHide.Invoke();
        
    }
}
