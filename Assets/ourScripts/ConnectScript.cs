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
    private GameObject interactionUI;
    private string name;

    public void OnConnectClick() {

        // Read in Info
        string username = usernameInput.text;
        Debug.Log("Connect was pressed, username is " + username);

        interactionUI = GameObject.FindGameObjectsWithTag("InteracUI")[0]; // only one exists
        bool isSuccessfull = JoinGame();

        // Adjust interface, Server handles Character activation
        if (isSuccessfull) {
            interactionUI.SetActive(true);
            gameObject.SetActive(false);
            Debug.Log("Joined?");
        }
    }
    public bool JoinGame() {
        // PlayerPref save mechanic here TODO
        string secret = "PlsNoLook"; //placeholder
        string username = "MustermanUser";
        // string secret = PlayerPrefs.GetString("secret");
        // string username = PlayerPrefs.GetString("username"); // save further up?

        PlayerState plState = ((InteractionUI) interactionUI.GetComponent(typeof (InteractionUI))).locPlayer;

        // Challenge is solved here 
        string challenge = plState.CMDtryLogin(username); 
        byte[] salt = plState.CMDgetSalt(username);
        challenge = secret + challenge;
        byte[] challengeAnswer = cryptoHash(challenge);

        // Check for success
        bool isSuccessful = plState.CMDAnswerChallenge(username, challengeAnswer);
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

    public static byte[] combineByteArrays(byte[] a, byte[] b) {
        byte[] c = new byte[a.Length+b.Length];
        System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
        System.Buffer.BlockCopy(b, 0, c, a.Length, a.Length + b.Length);
        return c;
    }
}
