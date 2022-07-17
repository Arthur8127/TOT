using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Votiong : MonoBehaviour
{
    public MatchController currentMatch;
    public List<PlayerElementVoting> players;
    public int value = -1;
    public Button btnVoiting;
    public Button btnSkip;
    public TextMeshProUGUI textTimer;
    
    public void OnShow()
    {
        btnSkip.interactable = true;
        btnVoiting.interactable = false;
        for (int i = 0; i < players.Count; i++) players[i].togle.isOn = false;
        
        if(currentMatch.localPlayer.isDead)
        {
            BlockAll();
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if(i > currentMatch.players.Count -1)
                {
                    //players[i].nickName.text += "[Disconnect]";
                    players[i].togle.interactable = false;
                    continue;
                }
                // меняем ник на профессию
                //players[i].nickName.text = currentMatch.players[i].NickName;
                if (currentMatch.players[i].isDead)
                {
                    //players[i].nickName.text += " [ТРУП]";
                    players[i].togle.interactable = false;
                }
                if (currentMatch.players[i].isLocalPlayer)
                {
                    players[i].togle.interactable = false;
                   // players[i].nickName.text += " [Я]";
                }
            }
        }
        
    }

    private void BlockAll()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].togle.interactable = false;
        }
        btnSkip.interactable = false;
        btnVoiting.interactable = false;
    }

    private void Update()
    {
        if(currentMatch.matchState == MatchController.MatchState.Voting)
        {
            textTimer.text = currentMatch.votingTimer.ToString();
        }
    }



    public void ClickPlayer(int v)
    {
        btnVoiting.interactable = false;
        if(players[v].togle.isOn) value = v;        
        else value = -1;        
        if (value > -1) btnVoiting.interactable = true;
    }

    public void UseSkip()
    {
        btnSkip.interactable = false;
        btnVoiting.interactable = false;
        currentMatch.CmdSendVotingValue(-1);
    }
    public void UseVoiting()
    {
        currentMatch.CmdSendVotingValue(value);
        btnSkip.interactable = false;
        btnVoiting.interactable = false;
    }

}
[System.Serializable]
public class PlayerElementVoting
{
    public TextMeshProUGUI nickName;
    public Toggle togle;
   

}