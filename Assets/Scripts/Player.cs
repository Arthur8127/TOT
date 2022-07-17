using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using System.Linq;

public class Player : NetworkBehaviour
{
    public SpriteRenderer sprite;
    public LayerMask layerPlayer;
    public MatchController currentMatch;
    public MoveControll moveControll;
    public enum PlayerType { None = -1, Mechanic = 0, Scientist = 1, Security = 2, CivilianD = 3, CivilianM = 4 }
    [SyncVar(hook = (nameof(OnType)))]
    public PlayerType playerType;
    [SyncVar(hook = (nameof(OnRaider)))]
    public bool isRaider;
    [SyncVar(hook = nameof(OnDead))]
    public bool isDead;
    [SyncVar]
    public float timerAttack = 0;
    [SyncVar] public string NickName;
    private MiniGamePoint currentMiniGame;
    private VotingButton currentVoting;
    
    private void Start()
    {
        if (isClient)
        {
            MatchController match = FindObjectOfType<MatchController>();
            match.players.Add(this);
        }
    }

    private void Update()
    {
        if (timerAttack > 0 && isRaider && isServer)
        {
            timerAttack -= Time.deltaTime;
        }
        if (isLocalPlayer && isRaider)
        {
            if (timerAttack > 0)
            {
                currentMatch.hud.timerAttack.text = "Откат убийства: " + timerAttack.ToString("F0");
                currentMatch.hud.buttonKill.interactable = false;
            }

            else
            {
                currentMatch.hud.buttonKill.interactable = true;
                currentMatch.hud.timerAttack.text = "";

            }

        }
        if (isLocalPlayer && isRaider && timerAttack <= 0)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                currentMatch.hud.UseAttack();
            }
        }
        if (isLocalPlayer)
        {
            if (currentMatch.hud.btnAction.isActiveAndEnabled)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UseAction();
                }
            }

        }
    }
    private void OnDead(bool _, bool value)
    {
        if (isLocalPlayer)
        {
            moveControll.SetControlActive = false;
            if (currentMatch.matchState == MatchController.MatchState.GamePlay)
            {
                currentMatch.hud.HideAll();
            }
        }
        sprite.color = new Color(0, 0, 0, 0.5f);
    }
    private void OnType(PlayerType _, PlayerType value)
    {

        Animator anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = NetManager.instance.game.animations[(int)value];

    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        currentMatch = FindObjectOfType<MatchController>();
        currentMatch.localPlayer = this;
        Camera.main.GetComponent<CameraControll>().target = transform;
        currentMatch.hud.timerAttack.text = playerType.ToString();
        CmdSetNickName(PlayerPrefs.GetString("NickName"));

    }


    [Command]
    private void CmdSetNickName(string nick)
    {
        NickName = nick;
    }

    public void OnRaider(bool _, bool value)
    {
        if (isLocalPlayer)
        {
            currentMatch.hud.btnKill.SetActive(value);
            currentMatch.hud.displayMissionText.text = "Убей всех";
        }


    }

    [Command]
    public void CmdAttack()
    {

        Guid matchID = GetComponent<NetworkMatch>().matchId;
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 2, layerPlayer);
        for (int i = 0; i < col.Length; i++)
        {
            if (transform != col[i].transform)
            {
                Player targetPlayer = col[i].GetComponent<Player>();
                if (col[i].GetComponent<NetworkMatch>().matchId == matchID && targetPlayer.isDead == false)
                {
                    timerAttack = NetManager.instance.game.timerAttack;
                    targetPlayer.isDead = true;
                    currentMatch.MiniGameCheneType((int)targetPlayer.playerType);
                    int livePlayes = currentMatch.GetLivePlayers();

                    if (livePlayes < 3)
                    {
                        currentMatch.StartCoroutine(currentMatch.ResultVotingWhite("Предатель совершил очередное убийство", MatchController.MatchState.End, true));
                    }
                    return;
                }
            }

        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;


        if (collision.tag == "MiniGame")
        {
            MiniGamePoint miniGame = collision.GetComponent<MiniGamePoint>();
            if (miniGame.isComplite || playerType.ToString() != miniGame.playerType.ToString()) return;
            currentMatch.hud.BtnAction(true);
            currentMiniGame = miniGame;
        }
        else if (collision.tag == "Viting")
        {
            currentVoting = collision.GetComponent<VotingButton>();
            currentMatch.hud.BtnAction(true);
        }
       
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;
        if (collision.tag == "MiniGame" || collision.tag == "Viting")
        {
            currentMatch.hud.BtnAction(false);
            currentMiniGame = null;
            currentVoting = null;
        }
        
    }
    public void UseAction()
    {
        if (currentMiniGame)
            currentMiniGame.windows.Show();
        else if (currentVoting)
            currentVoting.UseButton();
        currentMatch.hud.BtnAction(false);
    }


   
}

