using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePoint : MonoBehaviour
{
    public Windows windows;
    public bool isComplite;
    public enum PlayerType { Mechanic = 0, Scientist = 1, Security = 2, CivilianD = 3, CivilianM = 4 }
    public PlayerType playerType;
    public SpriteRenderer miniMapIcon;
    public void OnComplite()
    {
        isComplite = true;
        miniMapIcon.enabled = false;
        Invoke("Hide", 1f);
    }
    public void Hide()
    {
        windows.Hide();
    }

}
