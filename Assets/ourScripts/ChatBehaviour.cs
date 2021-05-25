using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// This Script is based on the following Tutorial : https://www.youtube.com/watch?v=p-2QFmCMBt8
// Be warned that I relocated the \n addition to the message.

using System;
using TMPro;

public class ChatBehaviour : NetworkBehaviour {

    private GameObject chatUI = null; // This is containing the elements and can be hidden.
    private TMP_Text chatText = null; // Here the received/sent messages are displayed
    // private TMP_InputField inputField = null; // Here new messages are typed

    [SerializeField] private playerController plControl = null; // needed to block Input

    private static event Action<string> OnMessage;

    //When this (not any other) ChatBehaviour is initialized, register to chat feed
    public override void OnStartAuthority() {

        chatUI = GameObject.FindWithTag("ChatUI");
        chatText = GameObject.FindWithTag("ChatText").GetComponent(typeof(TMP_Text)) as TMP_Text ;

        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
        gameObject.SetActive(false); // Enabled after Login
    }

    // unregister from chat feed
    [ClientCallback]
    private void OnDestroy() {
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    // Add new message to chat
    [Client]
    private void HandleNewMessage(string message) {
        Debug.Log(message);
        Debug.Log(chatText);
        chatText.text += "\n" + message;
    }

    //Handle input field
    [Client] // Client only
    public void Send(string message) {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }

        if (string.IsNullOrWhiteSpace(message)) { return; }

        CmdSendMessage(message);
    }

    //Send own message
    [Command] // Client calls, server runs
    private void CmdSendMessage(string message) {
        RpcHandleMessage($"[{(connectionToClient?.identity?.GetComponent<PlayerState>()?.GetName()) ?? "<Unnamed>"}]: {message}");
    }

    //Receive message
    [ClientRpc] // Server calls, all clients run
    private void RpcHandleMessage(string message) {
        OnMessage?.Invoke(message);
    }

    //Block Movement while in Input
    [Client]
    public void BlockInput() {
        plControl.changeBlock();
    }
}