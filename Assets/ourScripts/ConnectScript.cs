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
    private PlayerState plState;
    private string username;

    public void OnConnectClick() {

        // Read in Info
        string username = usernameInput.text;
        Debug.Log("Connect was pressed, username is " + username);

        interactionUI = GameObject.FindGameObjectsWithTag("InteracUI")[0]; // only one exists
        JoinGame();

    }
    public void JoinGame() {
        // PlayerPref save mechanic here TODO
        string username = "MustermanUser";
        // string username = PlayerPrefs.GetString("username"); // save further up?

        plState = ((InteractionUI) interactionUI.GetComponent(typeof (InteractionUI))).locPlayer;

        // Challenge is requested here 
        Debug.Log("JoinGame done");
        plState.CMDtryLogin(username, this); 
    }

    [ClientCallback] //proper method indicator?
    public void receiveChallenge(byte[] nonce, byte[] salt) {
        //PlayerPref mechanic here TODO
        string secret = PlayerPrefs.GetString("secret");
        secret = "plsNoTell"; //TEST CODE PLS DELETE
        byte[] secretBytes = ASCIIEncoding.ASCII.GetBytes(secret);
        byte[] challengeAnswer = cryptoHash(combineByteArrays(secretBytes, salt));
        challengeAnswer = cryptoHash(combineByteArrays(challengeAnswer, nonce));

        // Check for success
        Debug.Log("receiveChallenge done");
        plState.CMDAnswerChallenge(username, challengeAnswer, this);
    }

    [ClientCallback] //proper method indicator?
    public void ChallengeAccepted() {
        interactionUI.SetActive(true);
        //TODO chatUI SetActive(true);
        //gardenManager SetActive(true);
        gameObject.SetActive(false);
        Debug.Log("finished joining?");
    }

    //CLientcallback newUserData ? 


    // Based on:
    // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/compute-hash-values
    public static byte[] cryptoHash(byte[] input) {

        // Generate Hash
        byte[] byteString = new MD5CryptoServiceProvider().ComputeHash(input);

        return byteString;
    }

    public static byte[] combineByteArrays(byte[] a, byte[] b) {
        byte[] c = new byte[a.Length+b.Length];
        System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
        System.Buffer.BlockCopy(b, 0, c, a.Length, a.Length + b.Length);
        return c;
    }
}
