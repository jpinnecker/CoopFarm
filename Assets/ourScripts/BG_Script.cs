using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BG_Script : NetworkBehaviour{

    public void isClicked() { // Initiates plant click handling in PlayerState
        GameObject playerObj = NetworkClient.localPlayer.gameObject;
        PlayerState plState = playerObj.GetComponent(typeof(PlayerState)) as PlayerState;
        plState.cmdBackgroundClick();
    }
}
