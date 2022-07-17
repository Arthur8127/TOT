using System;
using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class Auth : NetworkAuthenticator
{
    public InputField input;

    #region Messages

    public struct AuthRequestMessage : NetworkMessage
    {

        public string msg;

    }

    public struct AuthResponseMessage : NetworkMessage
    {
        public string msg;
    }

    #endregion

    #region Server


    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }
    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<AuthRequestMessage>();
    }

    public override void OnServerAuthenticate(NetworkConnection conn)
    {

    }


    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
    {
        AuthResponseMessage _msg = new AuthResponseMessage
        {
            msg = "msg"
        };


        conn.Send(_msg);
        ServerAccept(conn);
        //for (int i = 0; i < NetManager.instance.allPlayerObjecs.Count; i++)
        //{
        //    if (NetManager.instance.allPlayerObjecs[i].identity == msg.msg)
        //    {
        //        PlayerBehaviour pb = NetManager.instance.allPlayerObjecs[i];
        //        SaveData sd = NetManager.instance.GetSaveData(pb);
        //        NetworkServer.RemovePlayerForConnection(pb.connectionToClient, false);                
        //        NetworkServer.ReplacePlayerForConnection(conn, pb.gameObject, true);
        //        NetManager.instance.Load(pb, sd);

        //        return;
        //    }
        //}

        StartCoroutine(SpawnPlayer(conn, msg.msg));
    }


    IEnumerator SpawnPlayer(NetworkConnection conn, string msg)
    {
        yield return null;
        //WWWForm form = new WWWForm();
        //form.AddField("action", "login");
        //form.AddField("identity", msg);
        //WWW www = new WWW(NetManager.instance.dataBase, form);
        //yield return www;
        //NetManager.instance.SpawnPlayer(conn, www.text);
    }

    #endregion

    #region Client


    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthResponseMessage>((Action<AuthResponseMessage>)OnAuthResponseMessage, false);
    }

    public override void OnStopClient()
    {
        NetworkClient.UnregisterHandler<AuthResponseMessage>();
    }

    public override void OnClientAuthenticate()
    {

    }
    public void SignIn()
    {
        AuthRequestMessage auth = new AuthRequestMessage
        {
            msg = input.text
        };
        NetworkClient.connection.Send(auth);
    }



    [Obsolete("OLD")]
    public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg) => OnAuthResponseMessage(msg);

    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        ClientAccept();

    }

    #endregion

}

