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
    public TMP_InputField chatInput;
    public Button chatButtonSend;
    
    public void SetNames() // <== Player -> OnStartAuthority
    {
        for (int i = 0; i < currentMatch.players.Count; i++)
        {
            players[i].nickName.text = currentMatch.players[i].NickName;
        }
    }
    
    public void OnShow()
    {
       
        chatButtonSend.interactable = !currentMatch.localPlayer.isDead;
        chatInput.interactable = !currentMatch.localPlayer.isDead;
        btnSkip.interactable = true;
        btnVoiting.interactable = false;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].togle.interactable = true;
            players[i].togle.isOn = false;
        }
        
        if(currentMatch.localPlayer.isDead) BlockAll();
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if(i > currentMatch.players.Count -1 || currentMatch.players[i].isDead || currentMatch.players[i].isLocalPlayer)
                {
                    players[i].togle.interactable = false;
                    continue;
                    
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
        BlockAll();
        currentMatch.CmdSendVotingValue(-1);
    }
    public void UseVoiting()
    {
        currentMatch.CmdSendVotingValue(value);
        BlockAll();
    }

}
[System.Serializable]
public class PlayerElementVoting
{
    public TextMeshProUGUI nickName;
    public Toggle togle;
   

}