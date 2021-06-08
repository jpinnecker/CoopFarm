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
    private InteractionUI interactionUI;
    private PlayerState plState;
    private string username;

    private void Start() {
        interactionUI = GameObject.FindObjectOfType<InteractionUI>();
        //interactionUI.gameObject.SetActive(false); // will be done by PlayerState
    }

    public void OnConnectClick() {

        // Read in Info
        string username = usernameInput.text;
        Debug.Log("Connect was pressed, username is " + username);
        JoinGame();

    }
    public void JoinGame() {
        // PlayerPref save mechanic here TODO
        string username = "MustermanUser";
        // string username = PlayerPrefs.GetString("username"); // save further up?

        plState = interactionUI.locPlayer;

        // Challenge is requested here 
        Debug.Log("JoinGame done");
        plState.CMDtryLogin(username, this); 
    }

    [TargetRpc] //target is used by mirror for identification of the function call target.
    public void receiveChallenge(NetworkConnection target, byte[] nonce, byte[] salt) {
        Debug.Log("receiveChallenge begins");

        string secret = passwordInput.text;
        //PlayerPref mechanic here TODO
        byte[] secretBytes = ASCIIEncoding.ASCII.GetBytes(secret);
        byte[] challengeAnswer = cryptoHash(combineByteArrays(secretBytes, salt));
        challengeAnswer = cryptoHash(combineByteArrays(challengeAnswer, nonce));

        // Check for success
        Debug.Log("receiveChallenge done");
        plState.CMDAnswerChallenge(username, challengeAnswer, this);
    }

    [TargetRpc]
    public void ChallengeAccepted(NetworkConnection target) {
        interactionUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
        Debug.Log("finished joining?");
    }

    [TargetRpc]
    public void getNewUserData(byte[] salt) {
        byte[] secretBytes = ASCIIEncoding.ASCII.GetBytes(passwordInput.text);
        PlayerState.CMDRegisterNewUser(,usernameInput.text, cryptoHash(combineByteArrays(secretBytes, salt)));
    }

    //CLientcallback newUserData ? 


    // Based on:
    // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/compute-hash-values
    public static byte[] cryptoHash(byte[] input) {
        // Generate Hash
        byte[] byteString = new MD5CryptoServiceProvider().ComputeHash(input);
        //Debug.Log("cryptoHashDone");
        return byteString;
    }

    public static byte[] combineByteArrays(byte[] a, byte[] b) {
        //Debug.Log("a, b, c length: " + a.Length + ","+b.Length+","+c.Length);
        //System.Buffer.BlockCopy(b, 0, c, a.Length ,b.Length);

        // So uncivilised
        var myList = new List<byte>();
        myList.AddRange(a);
        myList.AddRange(b);
        byte[] c = myList.ToArray();

        //Debug.Log("CombineArrayDone");
        return c;
    }
}
