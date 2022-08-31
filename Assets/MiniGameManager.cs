using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MiniGameManager : MonoBehaviour
{
    public List<Windows> allGames = new List<Windows>();

   
    public void OpenGame(int index)
    {
        for (int i = 0; i < allGames.Count; i++)
        {
            allGames[i].Hide();
            if (i == index) allGames[i].Show();           
        }
        
    }

    public void RestartScene()
    {
        OpenGame(-1);
    }

}
