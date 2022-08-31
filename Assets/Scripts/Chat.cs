using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;

public class Chat : MonoBehaviour
{
    public RectTransform messageContent;
    public GameObject messagePrefab;
    public TMP_InputField inputField;
    public MatchController matchController;
    public AudioClip newMessageClip;
    public void SendMessageChat()
    {
        string message = inputField.text;
        if (inputField.text.Length > 0 && String.IsNullOrEmpty(message) == false)
        {
            string autor = MatchController.instance.playerTypeName[(int)MatchController.instance.localPlayer.playerType];
            MatchController.instance.CmdSendMessage(autor, message);
        }
        inputField.text = "";
    }

    public void GetMessage(string autor, string message)
    {
        matchController.PlaySound(newMessageClip);
        ChatMessagePrefab chatMessagePrefab = Instantiate(messagePrefab, messageContent).GetComponent<ChatMessagePrefab>();
        chatMessagePrefab.t_mess.text = "<color=#03244D>"+autor+"</color> "+message;
    }

    private void Update()
    {
        if (matchController.matchState == MatchController.MatchState.Voting)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageChat();
            }
        }

    }
}
