using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    public event Action<NetworkConnection> OnPlayerDisconnected;
    internal static readonly Dictionary<NetworkConnection, Guid> playerMatches = new Dictionary<NetworkConnection, Guid>();
    internal static readonly Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();
    internal static readonly Dictionary<Guid, HashSet<NetworkConnection>> matchConnections = new Dictionary<Guid, HashSet<NetworkConnection>>();
    internal static readonly Dictionary<NetworkConnection, PlayerInfo> playerInfos = new Dictionary<NetworkConnection, PlayerInfo>();
    internal static readonly List<NetworkConnection> waitingConnections = new List<NetworkConnection>();
    public Dictionary<Guid, MatchController> matches = new Dictionary<Guid, MatchController>();
    internal Guid localPlayerMatch = Guid.Empty;
    internal Guid localJoinedMatch = Guid.Empty;
    internal Guid selectedMatch = Guid.Empty;
    int playerIndex = 1;
    private bool isOwner;
    public Button btnCancelmatch;
    public GameObject matchControllerPrefab;
    public Toggle[] searchPlayers;
    public Windows memuWindws, WaitingWindows, loginWindows, lobbyWindows, characterWindows, dialog;
    public TextMeshProUGUI balanceText;
    public AudioSource sourceSounds, sourceMusic;
    public Slider sliderSound, sliderMusic;
    public AudioClip[] lobbyClips;
    public bool isFanMode;

    private void Awake()
    {
        instance = this;
        LoadSoundValue();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetStatics()
    {
        playerMatches.Clear();
        openMatches.Clear();
        matchConnections.Clear();
        playerInfos.Clear();
        waitingConnections.Clear();
    }
    internal void InitializeData()
    {
        playerMatches.Clear();
        openMatches.Clear();
        matchConnections.Clear();
        waitingConnections.Clear();
        localPlayerMatch = Guid.Empty;
        localJoinedMatch = Guid.Empty;
    }
    void ResetCanvas()
    {
        InitializeData();
    }


    #region Button Calls

    public void SelectMatch(Guid matchId)
    {
        if (!NetworkClient.active) return;

        if (matchId == Guid.Empty)
        {
            selectedMatch = Guid.Empty;
        }
        else
        {
            if (!openMatches.Keys.Contains(matchId)) return;
            selectedMatch = matchId;
            MatchInfo infos = openMatches[matchId];
        }
    }
    public void RequestCreateMatch()
    {
        if (!NetworkClient.active) return;
        ServerMatchMessage msg = new ServerMatchMessage();
        msg.serverMatchOperation = ServerMatchOperation.Create;
        msg.modeIsFree = isFanMode;
        NetworkClient.connection.Send(msg);
    }

    public void JoinOrCreateMatch(bool mode)
    {
        isFanMode = mode;
        if (!mode && UserData.balance < 8)
        {
            dialog.Show();
            return;
        }

        foreach (var item in openMatches)
        {
            if (item.Value.players < item.Value.maxPlayers && isFanMode == item.Value.isFree)
            {
                SelectMatch(item.Value.matchId);
                RequestJoinMatch();
                return;
            }
        }
        RequestCreateMatch();
    }
    public void LevengOrCancelMatch()
    {
        if (isOwner) RequestCancelMatch();
        else RequestLeaveMatch();
    }
    public void RequestJoinMatch()
    {
        if (!NetworkClient.active || selectedMatch == Guid.Empty) return;

        ServerMatchMessage msg = new ServerMatchMessage();
        msg.serverMatchOperation = ServerMatchOperation.Join;
        msg.info.balance = UserData.balance;
        msg.info.email = UserData.email;
        msg.matchId = selectedMatch;
        NetworkClient.connection.Send(msg);
    }
    public void RequestLeaveMatch()
    {
        if (!NetworkClient.active || localJoinedMatch == Guid.Empty) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, matchId = localJoinedMatch });
    }
    public void RequestCancelMatch()
    {
        if (!NetworkClient.active || localPlayerMatch == Guid.Empty) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Cancel });
    }
    public void RequestReadyChange()
    {
        if (!NetworkClient.active || (localPlayerMatch == Guid.Empty && localJoinedMatch == Guid.Empty)) return;

        Guid matchId = localPlayerMatch == Guid.Empty ? localJoinedMatch : localPlayerMatch;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Ready, matchId = matchId });
    }
    public void RequestStartMatch()
    {
        if (!NetworkClient.active || localPlayerMatch == Guid.Empty) return;
        isOwner = false;
        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Start });
    }
    public void OnMatchEnded()
    {
        if (!NetworkClient.active) return;

        localPlayerMatch = Guid.Empty;
        localJoinedMatch = Guid.Empty;
        ShowLobbyView();
    }
    internal void SendMatchList(NetworkConnection conn = null)
    {
        if (!NetworkServer.active) return;

        if (conn != null)
        {
            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
        }
        else
        {
            foreach (var waiter in waitingConnections)
            {
                waiter.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
            }
        }
    }

    #endregion

    #region Server & Client Callbacks
    internal void OnStartServer()
    {
        if (!NetworkServer.active) return;

        InitializeData();
        NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
    }
    internal void OnServerReady(NetworkConnection conn)
    {
        if (!NetworkServer.active) return;

        waitingConnections.Add(conn);
        playerInfos.Add(conn, new PlayerInfo { playerIndex = this.playerIndex });
        playerIndex++;

        SendMatchList();
    }
    internal void OnServerDisconnect(NetworkConnection conn)
    {
        if (!NetworkServer.active) return;

        // Invoke OnPlayerDisconnected on all instances of MatchController
        OnPlayerDisconnected?.Invoke(conn);

        Guid matchId;
        if (playerMatches.TryGetValue(conn, out matchId))
        {
            playerMatches.Remove(conn);
            openMatches.Remove(matchId);

            foreach (NetworkConnection playerConn in matchConnections[matchId])
            {
                PlayerInfo _playerInfo = playerInfos[playerConn];
                //_playerInfo.ready = false;
                _playerInfo.matchId = Guid.Empty;
                playerInfos[playerConn] = _playerInfo;
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
            }
        }

        foreach (KeyValuePair<Guid, HashSet<NetworkConnection>> kvp in matchConnections)
        {
            kvp.Value.Remove(conn);
        }

        PlayerInfo playerInfo = playerInfos[conn];
        if (playerInfo.matchId != Guid.Empty)
        {
            MatchInfo matchInfo;
            if (openMatches.TryGetValue(playerInfo.matchId, out matchInfo))
            {
                matchInfo.players--;
                openMatches[playerInfo.matchId] = matchInfo;
            }

            HashSet<NetworkConnection> connections;
            if (matchConnections.TryGetValue(playerInfo.matchId, out connections))
            {
                PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

                foreach (NetworkConnection playerConn in matchConnections[playerInfo.matchId])
                {
                    if (playerConn != conn)
                    {
                        playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
                    }
                }
            }
        }

        SendMatchList();
    }
    internal void OnStopServer()
    {
        ResetCanvas();
    }
    internal void OnClientConnect()
    {
        playerInfos.Add(NetworkClient.connection, new PlayerInfo { playerIndex = this.playerIndex });
        loginWindows.Hide();
        lobbyWindows.Show();
    }
    internal void OnStartClient()
    {
        if (!NetworkClient.active) return;
        InitializeData();
        ShowLobbyView();
        NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
    }
    internal void OnClientDisconnect()
    {
        if (!NetworkClient.active) return;
        InitializeData();
    }
    internal void OnStopClient()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Server Match Message Handlers
    void OnServerMatchMessage(NetworkConnection conn, ServerMatchMessage msg)
    {
        if (!NetworkServer.active) return;

        switch (msg.serverMatchOperation)
        {
            case ServerMatchOperation.None:
                {

                    break;
                }

            case ServerMatchOperation.Create:
                {
                    OnServerCreateMatch(conn, msg.modeIsFree);
                    break;
                }
            case ServerMatchOperation.Cancel:
                {
                    OnServerCancelMatch(conn);
                    break;
                }
            case ServerMatchOperation.Start:
                {
                    OnServerStartMatch(conn);
                    break;
                }
            case ServerMatchOperation.Join:
                {
                    OnServerJoinMatch(conn, msg);
                    break;
                }
            case ServerMatchOperation.Leave:
                {
                    OnServerLeaveMatch(conn, msg.matchId);
                    break;
                }
            case ServerMatchOperation.Ready:
                {
                    OnServerPlayerReady(conn, msg.matchId);
                    break;
                }

        }
    }
    void OnServerPlayerReady(NetworkConnection conn, Guid matchId)
    {
        if (!NetworkServer.active) return;

        PlayerInfo playerInfo = playerInfos[conn];
        // playerInfo.ready = !playerInfo.ready;
        playerInfos[conn] = playerInfo;

        HashSet<NetworkConnection> connections = matchConnections[matchId];
        PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

        foreach (NetworkConnection playerConn in matchConnections[matchId])
        {
            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }
    }
    void OnServerLeaveMatch(NetworkConnection conn, Guid matchId)
    {
        if (!NetworkServer.active) return;

        MatchInfo matchInfo = openMatches[matchId];
        matchInfo.players--;
        openMatches[matchId] = matchInfo;

        PlayerInfo playerInfo = playerInfos[conn];
        //playerInfo.ready = false;
        playerInfo.matchId = Guid.Empty;
        playerInfos[conn] = playerInfo;

        foreach (KeyValuePair<Guid, HashSet<NetworkConnection>> kvp in matchConnections)
        {
            kvp.Value.Remove(conn);
        }

        HashSet<NetworkConnection> connections = matchConnections[matchId];
        PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

        foreach (NetworkConnection playerConn in matchConnections[matchId])
        {
            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }

        SendMatchList();

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
    }
    void OnServerCreateMatch(NetworkConnection conn, bool mode)
    {
        if (!NetworkServer.active || playerMatches.ContainsKey(conn)) return;

        Guid newMatchId = Guid.NewGuid();
        matchConnections.Add(newMatchId, new HashSet<NetworkConnection>());
        matchConnections[newMatchId].Add(conn);
        playerMatches.Add(conn, newMatchId);
        openMatches.Add(newMatchId, new MatchInfo { matchId = newMatchId, maxPlayers = NetManager.instance.game.PlayerNedeetForMatch, players = 1, isFree = mode });

        PlayerInfo playerInfo = playerInfos[conn];
        //playerInfo.ready = true;
        playerInfo.matchId = newMatchId;
        playerInfos[conn] = playerInfo;

        PlayerInfo[] infos = matchConnections[newMatchId].Select(playerConn => playerInfos[playerConn]).ToArray();

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Created, matchId = newMatchId, playerInfos = infos });

        SendMatchList();
    }
    void OnServerCancelMatch(NetworkConnection conn)
    {
        if (!NetworkServer.active || !playerMatches.ContainsKey(conn)) return;

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Cancelled });

        Guid matchId;
        if (playerMatches.TryGetValue(conn, out matchId))
        {
            playerMatches.Remove(conn);
            openMatches.Remove(matchId);

            SendMatchList();
            foreach (NetworkConnection playerConn in matchConnections[matchId])
            {

                PlayerInfo playerInfo = playerInfos[playerConn];
                //playerInfo.ready = false;
                playerInfo.matchId = Guid.Empty;
                playerInfos[playerConn] = playerInfo;
                if (playerConn == conn)
                    conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
                else
                    playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.ReConnect });
            }


        }
    }
    void OnServerStartMatch(NetworkConnection conn)
    {
        if (!NetworkServer.active || !playerMatches.ContainsKey(conn)) return;

        Guid matchId;
        if (playerMatches.TryGetValue(conn, out matchId))
        {
            GameObject matchControllerObject = Instantiate(matchControllerPrefab);
            matchControllerObject.GetComponent<NetworkMatch>().matchId = matchId;
            MatchController currentMatch = matchControllerObject.GetComponent<MatchController>();
            currentMatch.isFreeGame = openMatches[matchId].isFree;
            NetworkServer.Spawn(matchControllerObject);

            MatchController matchController = matchControllerObject.GetComponent<MatchController>();
            matches.Add(matchId, matchController);
            int indexSpawn = 0;
            foreach (NetworkConnection playerConn in matchConnections[matchId])
            {
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Started });

                GameObject player = Instantiate(NetManager.instance.playerPrefab, currentMatch.spawnManger.spawns[indexSpawn].position, Quaternion.identity);
                Player p = player.GetComponent<Player>();
                p.playerType = (Player.PlayerType)indexSpawn;
                p.currentMatch = matchController;
                player.GetComponent<NetworkMatch>().matchId = matchId;
                NetworkServer.AddPlayerForConnection(playerConn, player);
                p.moveControll.anim.runtimeAnimatorController = NetManager.instance.game.animations[indexSpawn];
                matchController.players.Add(player.GetComponent<Player>());

                /* Reset ready state for after the match. */
                PlayerInfo playerInfo = playerInfos[playerConn];
                //playerInfo.ready = false;
                playerInfos[playerConn] = playerInfo;
                indexSpawn++;
            }
            int playerId = Random.Range(0, matchController.players.Count);

            // ===== ADD KILLER =======
            matchController.players[playerId].isRaider = true;
            Killer k = matchController.players[playerId].GetComponent<Killer>();
            k.enabled = true;


            //========================
            if (!openMatches[matchId].isFree)
            {
                PlayerInfo[] infos = matchConnections[matchId].Select(playerConn => playerInfos[playerConn]).ToArray();
                float take = -2f;
                for (int i = 0; i < infos.Length; i++)
                {
                    currentMatch.raiderID = playerId;
                    currentMatch.emails.Add(infos[i].email);
                    WWWForm form = new WWWForm();
                    form.AddField("action", "balance");
                    form.AddField("email", infos[i].email);
                    if (i == playerId) take = -8f;
                    form.AddField("take", take.ToString());
                    WWW www = new WWW(NetManager.instance.game.urlDataBase, form);

                }
                foreach (NetworkConnection playerConn in matchConnections[matchId])
                {
                    playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateBalance });
                }
            }

            //matchController.players[playerId].timerAttack = NetManager.instance.game.timerAttack;
            memuWindws.Hide();
            playerMatches.Remove(conn);
            openMatches.Remove(matchId);
            matchConnections.Remove(matchId);
            SendMatchList();
            OnPlayerDisconnected += matchController.OnPlayerDisconnected;

        }
    }
    void OnServerJoinMatch(NetworkConnection conn, ServerMatchMessage msg)
    {
        if (!NetworkServer.active || !matchConnections.ContainsKey(msg.matchId) || !openMatches.ContainsKey(msg.matchId)) return;

        MatchInfo matchInfo = openMatches[msg.matchId];
        matchInfo.players++;
        openMatches[msg.matchId] = matchInfo;
        matchConnections[msg.matchId].Add(conn);

        PlayerInfo playerInfo = playerInfos[conn];
        // playerInfo.ready = true;
        playerInfo.matchId = msg.matchId;
        playerInfos[conn] = playerInfo;
        playerInfo.balance = msg.info.balance;
        playerInfo.email = msg.info.email;
        PlayerInfo[] infos = matchConnections[msg.matchId].Select(playerConn => playerInfos[playerConn]).ToArray();
        SendMatchList();

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Joined, matchId = msg.matchId, playerInfos = infos });

        foreach (NetworkConnection playerConn in matchConnections[msg.matchId])
        {
            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }
    }
    #endregion

    #region Client Match Message Handler

    void OnClientMatchMessage(ClientMatchMessage msg)
    {
        if (!NetworkClient.active) return;

        switch (msg.clientMatchOperation)
        {
            case ClientMatchOperation.None:
                {

                    break;
                }

            case ClientMatchOperation.List:
                {
                    openMatches.Clear();
                    foreach (MatchInfo matchInfo in msg.matchInfos)
                    {
                        openMatches.Add(matchInfo.matchId, matchInfo);
                    }
                    RefreshMatchList();
                    break;
                }
            case ClientMatchOperation.Created:
                {

                    localPlayerMatch = msg.matchId;
                    OnClientUpdateRoom(1);
                    isOwner = true;
                    break;
                }
            case ClientMatchOperation.Cancelled:
                {
                    isOwner = false;
                    localPlayerMatch = Guid.Empty;
                    ShowLobbyView();
                    break;
                }
            case ClientMatchOperation.Joined:
                {
                    localJoinedMatch = msg.matchId;
                    OnClientUpdateRoom((byte)msg.playerInfos.Length);

                    break;
                }
            case ClientMatchOperation.Departed:
                {

                    localJoinedMatch = Guid.Empty;
                    ShowLobbyView();
                    break;
                }
            case ClientMatchOperation.UpdateRoom:
                {
                    OnClientUpdateRoom((byte)msg.playerInfos.Length);
                    break;
                }
            case ClientMatchOperation.Started:
                {
                    lobbyWindows.Hide();
                    WaitingWindows.Hide();
                    break;
                }
            case ClientMatchOperation.ReConnect:
                {

                    localJoinedMatch = Guid.Empty;
                    JoinOrCreateMatch(isFanMode);
                    break;
                }
            case ClientMatchOperation.UpdateBalance:
                StartCoroutine(UpdateBalanceClient());
                break;
        }
    }
    void ShowLobbyView()
    {
        lobbyWindows.Show();
        memuWindws.Show();
        WaitingWindows.Hide();
    }



    public IEnumerator UpdateBalanceClient()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "getBalance");
        form.AddField("email", UserData.email);
        WWW www = new WWW(NetManager.instance.game.urlDataBase, form);
        yield return www;
        balanceText.text = www.text;
        UserData.balance = Convert.ToSingle(www.text);
    }

    void RefreshMatchList()
    {

    }

    void OnClientUpdateRoom(byte playerCount)
    {
        if (playerCount == 5) btnCancelmatch.interactable = false;
        else btnCancelmatch.interactable = true;
        PlayeSound(lobbyClips[0]);
        WaitingWindows.Show();
        for (int i = 0; i < 5; i++)
        {
            searchPlayers[i].isOn = i < playerCount;
        }
        WaitingWindows.textContent.text = "Найденно игроков: " + playerCount + "/" + NetManager.instance.game.PlayerNedeetForMatch;
        if (playerCount == NetManager.instance.game.PlayerNedeetForMatch)
        {
            if (isOwner)
            {
                RequestStartMatch();
            }

        }

    }




    #endregion

    #region Sound
    public void LoadSoundValue()
    {
        sliderSound.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVoume", 1f);
        sourceSounds.volume = sliderSound.value;
        sourceMusic.volume = sliderMusic.value;
    }
    public void SaveSoundValue()
    {
        PlayerPrefs.SetFloat("SoundVolume", sliderSound.value);
        PlayerPrefs.SetFloat("MusicVoume", sliderMusic.value);
        sourceSounds.volume = sliderSound.value;
        sourceMusic.volume = sliderMusic.value;
    }

    public void PlayeSound(AudioClip clip)
    {
        if (clip != null)
            sourceSounds.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        sourceMusic.Stop();
        if (clip != null)
        {
            sourceMusic.clip = clip;
            sourceMusic.Play();
        }

    }

    #endregion


}
