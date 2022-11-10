using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetManager : NetworkManager
{
    public static NetManager instance;
    public GameInformation game;



    #region Unity Callbacks
    public override void Awake()
    {
        instance = this;
        base.Awake();
        

    }

    public override void Start()
    {
        LobbyManager.instance.InitializeData();
        base.Start();
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        StartClient();
    }

    #endregion


    #region Server System Callbacks

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        LobbyManager.instance.OnServerReady(conn);

    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        LobbyManager.instance.OnServerDisconnect(conn);
        base.OnServerDisconnect(conn);
    }
    #endregion

    #region Client System Callbacks    
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        LobbyManager.instance.OnClientConnect();

    }
    public override void OnClientDisconnect()
    {
        LobbyManager.instance.OnClientDisconnect();
        base.OnClientDisconnect();

    }

    #endregion

    #region Start & Stop Callbacks   
    public override void OnStartServer()
    {
        LobbyManager.instance.OnStartServer();
    }
    public override void OnStartClient()
    {
        LobbyManager.instance.OnStartClient();
    }
    public override void OnStopServer()
    {
        LobbyManager.instance.OnStopServer();
    }
    public override void OnStopClient()
    {
        LobbyManager.instance.OnStopClient();
    }
    #endregion


}
