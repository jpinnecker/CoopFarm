using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script is based on the following Tutorial : https://www.youtube.com/watch?v=p-2QFmCMBt8
// Be warned that I relocated the \n addition to the message.

using Mirror;
using System;
using TMPro;

public class ChatBehaviour : NetworkBehaviour {

    [SerializeField] private GameObject chatUI = null; // This is containing the elements and can be hidden.
    [SerializeField] private TMP_Text chatText = null; // Here the received/sent messages are displayed
    [SerializeField] private TMP_InputField inputField = null; // Here new messages are typed

    private static event Action<string> OnMessage;

    //When this (not any other) ChatBehaviour is initialized, register to chat feed
    public override void OnStartAuthority() {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
    }

    // unregister from chat feed
    [ClientCallback] // only on Clients
    private void OnDestroy() { 
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    // Add new message to chat
    private void HandleNewMessage(string message) {
        chatText.text += "\n" + message;
    }

    //Handle input field
    [Client]
    public void Send(string message) {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }

        if (string.IsNullOrWhiteSpace(message)) { return; }

        CmdSendMessage(message);

        inputField.text = string.Empty;
    }

    //Send own message
    [Command]
    private void CmdSendMessage(string message) {
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
    }

    //Receive message
    [ClientRpc]
    private void RpcHandleMessage(string message) {
        OnMessage?.Invoke(message);
    }

}