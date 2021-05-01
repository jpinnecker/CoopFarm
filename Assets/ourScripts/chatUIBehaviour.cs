using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class chatUIBehaviour : NetworkBehaviour {

    GameObject playerObj;
    ChatBehaviour playerChat;
    [SerializeField] private TMP_InputField inputField = null;

    private void checkPlayerObj() {
        playerObj = ClientScene.localPlayer.gameObject;
        //Debug.Log(playerObj);
        playerChat = playerObj.GetComponent(typeof(ChatBehaviour)) as ChatBehaviour;
        //Debug.Log(playerChat);
    }

    public void blockInput() {
        checkPlayerObj();
        playerChat.BlockInput();
    }

    public void sendMyMessage(string message) {
        checkPlayerObj();
        playerChat.Send(message);
        inputField.text = string.Empty;
    }
}
