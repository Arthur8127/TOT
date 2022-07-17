using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VotingButton : MonoBehaviour
{
    public MatchController currentMatch;

    public void UseButton()
    {
        if(currentMatch.matchState == MatchController.MatchState.GamePlay)
        {
            currentMatch.CmdGetVoting();
        }
    }
}
