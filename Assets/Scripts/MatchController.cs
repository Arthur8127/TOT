using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class MatchController : NetworkBehaviour
{
    public List<int> votingList = new List<int>();
    [SyncVar] public int votingTimer;
    public static MatchController instance;
    public AudioSource sourceSound;
    public AudioClip timerClip, startMatchClip, startVotingClip, miniGameComplite;
    public GameHud hud;
    public Player localPlayer;
    public SpawnsManager spawnManger;
    public List<Player> players = new List<Player>();
    public enum MatchState { Starting = 0, GamePlay = 1, Voting = 2, End = 3 }
    [SyncVar(hook = (nameof(OnChengeState)))]
    public MatchState matchState;
    [SyncVar(hook = nameof(OnTimer))]
    public int currentTimer;
    public List<MiniGamePoint> miniGamePoints;
    public string[] playerTypeName;
    public Chat chat;
    public Slider sliderSound, sliderMusic;
    public AudioSource[] mapSources;
    private void Start()
    {
        if (!isServer)
        {
            instance = this;
            LoadSoundValue();
        }
        else
        {
            StartCoroutine(WaitingMatch());
        }

    }


    private IEnumerator WaitingMatch()
    {

        matchState = MatchState.Starting;
        currentTimer = NetManager.instance.game.TimeStartingMatch;
        while (currentTimer >= 0)
        {
            yield return new WaitForSeconds(1f);
            currentTimer--;
        }
        matchState = MatchState.GamePlay;

    }


    public void OnChengeState(MatchState _, MatchState value)
    {
        if (value != MatchState.End)
        {
            hud.HideAll();
        }


        localPlayer.moveControll.SetControlActive = false;
        switch (value)
        {
            case MatchState.Starting:

                hud.waitingStartMatch.Show();
                break;
            case MatchState.GamePlay:
                if (localPlayer.isDead == false)
                {
                    localPlayer.moveControll.SetControlActive = true;
                    if (localPlayer.isRaider)
                    {
                        localPlayer.GetComponent<Killer>().StartReload();
                    }
                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].moveControll.SetControlActive = true;
                    }
                }

                break;
            case MatchState.Voting:
                if (localPlayer)
                {
                    PlaySound(startVotingClip);
                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].moveControll.SetControlActive = false;
                    }
                    localPlayer.transform.position = spawnManger.spawns[(int)localPlayer.playerType].position;
                }
                hud.votingWindows.Show();
                break;
            case MatchState.End:
                break;
        }

    }

    public void OnTimer(int _, int value)
    {
        if (value < 0) return;
        hud.waitingStartMatch.textContent.text = "ИГРА НАЧНЕТСЯ ЧЕРЕЗ " + value + " СЕК.";
        if (value == 0) PlaySound(startMatchClip);
        else PlaySound(timerClip);
    }

    public void OnPlayerDisconnected(NetworkConnection conn)
    {

        Player playerDisconnect = players.Where(x => x.connectionToClient == conn).FirstOrDefault();
        if (!playerDisconnect) return;


        int playerIndex = players.IndexOf(playerDisconnect);
        if (playerDisconnect.isRaider)
        {
            StartCoroutine(ResultVotingWhite("Предатель добровольно сдался.", MatchState.End, false));
            return;
        }
        else
        {
            int playeCount = players.Select(x => x).Where(x => x.isDead == false && x.connectionToClient != conn).Count();
            if (playeCount < 3)
            {
                StartCoroutine(ResultVotingWhite("Один из мирных покинул игру.", MatchState.End, true));
                return;
            }

        }
        MiniGameCheneType((int)playerDisconnect.playerType);
        players.Remove(playerDisconnect);
        RpcRemovePlayer(playerIndex);

        #region comments
        //for (int i = 0; i < players.Count; i++)
        //{
        //    if (players[i].connectionToClient == conn)
        //    {
        //        if (players[i].isRaider == false)
        //        {
        //            if (players.Where(x => x.isDead == false).Count() <= 3)
        //            {
        //                StartCoroutine(ResultVotingWhite("Один из мирных покинул игры в команде мало мирных", MatchState.End, true));
        //                return;
        //            }
        //            MiniGameCheneType((int)players[i].playerType);
        //            players.RemoveAt(i);
        //            RpcRemovePlayer(i);
        //            return;
        //        }
        //        else
        //        {

        //            StartCoroutine(ResultVotingWhite("Предатель вышел из игры", MatchState.End, false));
        //        }
        //    }
        //}
        #endregion
    }
    [ClientRpc]
    public void RpcRemovePlayer(int index)
    {
        players.RemoveAt(index);
    }
    public void MiniGameCheneType(int oldType)
    {
        for (int i = 0; i < miniGamePoints.Count; i++)
        {
            if ((int)miniGamePoints[i].playerType == oldType)
            {
                List<int> list = GetPlayersID(oldType);
                int value = Random.Range(0, list.Count);
                miniGamePoints[i].playerType = (MiniGamePoint.PlayerType)list[value];
                players[value].RpcTargetSetMiniGame(i);
            }
        }
    }
    private List<int> GetPlayersID(int oldID)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < players.Count; i++)
        {
            if ((int)players[i].playerType == oldID || players[i].isDead)
                continue;
            else
                list.Add((int)players[i].playerType);
        }
        return list;
    }
    [Command(requiresAuthority = false)]
    public void CmdGetVoting()
    {
        votingList = new List<int>();
        matchState = MatchState.Voting;
        StartCoroutine(VotingTime());
    }
    private IEnumerator VotingTime()
    {
        votingTimer = NetManager.instance.game.TimerVoting;
        while (votingTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            votingTimer--;

        }
        AnalisVoiting();
    }
    [Command(requiresAuthority = false)]
    public void CmdSendVotingValue(int value)
    {
        votingList.Add(value);
        int p = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isDead == false) p++;
        }
        if (p == votingList.Count)
        {
            StopAllCoroutines();
            AnalisVoiting();
        }
    }
    public void AnalisVoiting()
    {
        string msg = "";
        votingList.RemoveAll(x => x == -1);
        if (votingList.Count < 1) // после удаление скпиов если нет хотябы 2 голоса то считаем как скип
        {
            msg = "Сегодня никто не умер.";
            StartCoroutine(ResultVotingWhite(msg, MatchState.GamePlay, false));
            return;
        }

        var d = votingList.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        var m = d.FirstOrDefault(x => x.Value == d.Values.Max()).Key;

        if (d[m] <= 1)
        {
            msg = "Сегодня никто не умер.";
            StartCoroutine(ResultVotingWhite(msg, MatchState.GamePlay, false));
            return;
        }
        if (d.Count >= 1)
        {
            foreach (var item in d)
            {
                if (item.Value == d[m] && item.Key != m)
                {
                    msg = "Результат голосования оказался спорным.";
                    StartCoroutine(ResultVotingWhite(msg, MatchState.GamePlay, false));
                    return;
                }
            }
            msg = "Решили избавиться от " + players[m].NickName;
            players[m].isDead = true;
            CheckWinner(players[m]);
        }

    }
    #region comment
    //MatchState state = MatchState.GamePlay;
    //string msg = "";
    //bool winner = false;
    //if (condidat > -1)
    //{
    //    if (condidat != secondCondidat)
    //    {

    //        players[condidat].isDead = true;
    //        if (players[condidat].isRaider)
    //        {
    //            state = MatchState.End;
    //            msg = "Проголосовали протов предателя";
    //            winner = false;
    //        }
    //        else
    //        {
    //            if(players.Select(x=>x.isDead == false).Count() < 3)
    //            {
    //                state = MatchState.End;
    //                msg = "Проголосовали протв мирного";
    //                winner = true;
    //            }
    //            else
    //            {
    //                state = MatchState.GamePlay;
    //                msg = "Проголосовали против мирного";

    //            }
    //        }

    //    }
    //    else
    //    {
    //        state = MatchState.GamePlay;
    //        msg = "Спорное голосование";
    //    }
    //}
    //else
    //{
    //    state = MatchState.GamePlay;
    //    msg = "Все скипнули";
    //}

    //StartCoroutine(ResultVotingWhite(msg, state, winner));

    #endregion


    private void CheckWinner(Player player)
    {
        if (player.isRaider)
        {
            StartCoroutine(ResultVotingWhite("Вы нашли предателя: " + player.NickName + ".", MatchState.End, false));
        }
        else
        {
            // выкинули мирного
            if (GetLivePlayers() > 2)
            {
                // если более 2х живых можно продложать
                StartCoroutine(ResultVotingWhite("Избавились от мирного: " + player.NickName + ".", MatchState.GamePlay, false));
            }
            else
            {
                // предатель победил надо заканчивать
                StartCoroutine(ResultVotingWhite("Избавились от мирного: " + player.NickName + ".", MatchState.End, true));
            }
        }
    }

    public int GetLivePlayers()
    {
        return players.Where(x => x.isDead == false).Count();
    }

    public IEnumerator ResultVotingWhite(string value, MatchState state, bool winner)
    {
        yield return new WaitForSeconds(0.5f);
        bool isEnd = false;
        if (state == MatchState.End) isEnd = true;
        RpcVotingResult(value, isEnd, winner);
        float waitingTimer = 3f;
        if (state == MatchState.End) waitingTimer = 5f;
        yield return new WaitForSeconds(waitingTimer);
        if (isEnd)
            ExitMatch();
        matchState = state;
    }
    [ClientRpc]
    public void RpcVotingResult(string value, bool IsEnd, bool winner)
    {
        hud.ShowResultVoting(value, IsEnd, winner);
    }

    private void ExitMatch()
    {
        Guid matchID = GetComponent<NetworkMatch>().matchId;
        LobbyManager.instance.matches.Remove(matchID);
        RpcExitMatch();
        LobbyManager.instance.OnPlayerDisconnected -= OnPlayerDisconnected;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].connectionToClient != null)
            {
                NetworkServer.RemovePlayerForConnection(players[i].connectionToClient, true);
            }
        }

        LobbyManager.instance.SendMatchList();
        NetworkServer.Destroy(gameObject);
    }

    public void BtnClickExit()
    {
        CmdExitMatchPlayer();
        LobbyManager.instance.memuWindws.Show();
        LobbyManager.instance.lobbyWindows.Show();
        LobbyManager.instance.WaitingWindows.Hide();

    }
    [Command(requiresAuthority = false)]
    public void CmdExitMatchPlayer(NetworkConnectionToClient sender = null)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].connectionToClient == sender)
            {
                OnPlayerDisconnected(players[i].connectionToClient);
                NetworkServer.RemovePlayerForConnection(sender, true);

            }
        }


    }


    [ClientRpc]
    private void RpcExitMatch()
    {
        LobbyManager.instance.memuWindws.Show();
        LobbyManager.instance.lobbyWindows.Show();
        LobbyManager.instance.WaitingWindows.Hide();
    }

    [ClientRpc]
    public void RpcGetMessage(string autor, string message)
    {
        chat.GetMessage(autor, message);
    }
    [Command(requiresAuthority = false)]
    public void CmdSendMessage(string autor, string message)
    {
        RpcGetMessage(autor, message);
    }

    public void ActivControl(bool value)
    {

        localPlayer.moveControll.SetControlActive = value;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            sourceSound.PlayOneShot(clip);
    }

    public void LoadSoundValue()
    {
        sliderSound.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVoume", 1f);
        sourceSound.volume = sliderSound.value;
        for (int i = 0; i < mapSources.Length; i++)
        {
            mapSources[i].volume = sliderSound.value;
        }
    }

    public void SaveSounValue()
    {
        PlayerPrefs.SetFloat("SoundVolume", sliderSound.value);
        PlayerPrefs.SetFloat("MusicVoume", sliderMusic.value);
        sourceSound.volume = sliderSound.value;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].moveControll)
            {
                players[i].moveControll.footStapsSource.volume = sliderSound.value;
            }
        }
        for (int i = 0; i < mapSources.Length; i++)
        {
            mapSources[i].volume = sliderSound.value;
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdAttack(uint id)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].netId == id)
            {
                int playerTypeToInt = (int)players[i].playerType;
                MiniGameCheneType(playerTypeToInt);
                players[i].isDead = true;
                if (GetLivePlayers() < 3)
                {
                    StartCoroutine(ResultVotingWhite("Предатель совершил очередное убийство", MatchController.MatchState.End, true));
                }
            }
        }
    }
}
