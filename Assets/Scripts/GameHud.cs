using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameHud : MonoBehaviour
{
    public Votiong voting;
    public Windows waitingStartMatch;
    public List<Windows> allWindow;
    public TextMeshProUGUI timerAttack, resultVitingText;
    public Windows votingWindows, resultVotingWindos;
    public GameObject btnKill;
    public TextMeshProUGUI displayMissionText;
    public Button btnAction, buttonKill;
    private MatchController currentMatch;
    public Image imgKd;

    private void Awake()
    {
        currentMatch = transform.root.GetComponent<MatchController>();
    }
    public void HideAll()
    {
        for (int i = 0; i < allWindow.Count; i++)
        {
            allWindow[i].Hide();
        }
    }

    public void ShowResultVoting(string value, bool isEnd, bool winner)
    {
        resultVotingWindos.Show();
        resultVitingText.text = value;
        if(isEnd)
        {
            if(winner == MatchController.instance.localPlayer.isRaider)
            {
                resultVitingText.text += "\n Победа!";
            }
            else
            {
                resultVitingText.text += "\n Поражение!";
            }
        }
    }

    public void UseAttack()
    {
        Killer killer = currentMatch.localPlayer.GetComponent<Killer>();
        killer.TryAttack();
    }

    public void BtnAction(bool value)
    {
        btnAction.interactable = value;
    }
    public void UseBtnAction()
    {
        currentMatch.localPlayer.UseAction();
    }
}
