using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ConnectScript : NetworkBehaviour
{

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField serverAdressInput;

    public void OnConnectClick() {
        string username = usernameInput.text;
        Debug.Log("Connect was pressed, username is " + username);
    }

}
