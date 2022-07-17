using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField input;
    public Button btn;
    string nick;

    private void Start()
    {
        nick = "Player" + Random.Range(1111, 9999);
        input.text = nick;
    }


    public void ClickConnect()
    {
        if(!String.IsNullOrEmpty(input.text) || input.text.Length > 4)
        {
            nick = input.text;
            PlayerPrefs.SetString("NickName", nick);
            NetManager.instance.StartClient();
            LobbyManager.instance.memuWindws.Show();
            LobbyManager.instance.loginWindows.Hide();
            LobbyManager.instance.WaitingWindows.Hide();
        }
    }
}
