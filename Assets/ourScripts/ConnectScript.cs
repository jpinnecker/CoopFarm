using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConnectScript : NetworkBehaviour
{

    public void OnConnectClick() {
        Debug.Log("Connect was pressed");
    }
}
