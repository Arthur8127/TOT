using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameInfo", menuName ="Game/Game Information")]
public class GameInformation : ScriptableObject
{
    public string urlDataBase = "http://ibtgame.store/Data/login.php";
    public int nedeedMiniGameCompliteToWin = 5;
    public byte PlayerNedeetForMatch = 4;
    public int TimeStartingMatch = 5;
    public int TimerVoting = 30;
    public float timerAttack = 20f;
    [Header("Players Animations")]
    public RuntimeAnimatorController[] animations;

}
