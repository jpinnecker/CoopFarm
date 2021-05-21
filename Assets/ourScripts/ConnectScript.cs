using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;
using Mirror;
using System.Text;

public class ConnectScript : NetworkBehaviour {

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [SerializeField] private NetworkManager netMan;
    private string name;

    public void OnConnectClick() {

        // Read in Info
        string username = usernameInput.text;
        Debug.Log("Connect was pressed, username is " + username);
        bool isSuccessfull = JoinGame();

        // Adjust interface, Server handles Character activation
        if (isSuccessfull) {
            GameObject interactionUI = GameObject.FindGameObjectsWithTag("InteracUI")[0]; // only one exists
            interactionUI.SetActive(true);
            gameObject.SetActive(false);
            Debug.Log("Joined?");
        }
    }
    public bool JoinGame() {
        // PlayerPref save mechanic here TODO
        string secret = "PlsNoLook"; //placeholder

        PlayerState plState = NetworkManager.client.connection.playerControllers; //? get isLocal? easier function?


        // Challenge is solved here 
        string challenge = plState.CMDtryLogin(username); // salt is already in front of challenge?
        challenge = secret + challenge;
        challenge = cryptoHash(challenge);

        // Check for success
        bool isSuccessful = plState.CMDAnswerChallenge(usernameInput, challenge);
        return isSuccessful;
    }


    // Based on:
    // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/compute-hash-values
    public static byte[] cryptoHash(string input) {

        // Generate Hash
        byte[] byteString = ASCIIEncoding.ASCII.GetBytes(input);
        byteString = new MD5CryptoServiceProvider().ComputeHash(byteString);

        return byteString;
    }
}
