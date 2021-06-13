using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class SaveObject {
    public List<GameObject> gardenList;
    public Dictionary<string, string> secretsStrings;
    public Dictionary<string, string> saltsStrings;

    public void setSecrets(Dictionary<string, byte[]> bytes) {
        secretsStrings = new Dictionary<string, string>();
        foreach (string username in bytes.Keys) {
            secretsStrings.Add(username, System.BitConverter.ToString(bytes[username]));
        }
    }

    public Dictionary<string, byte[]> getSecrets() {
        Dictionary<string, byte[]>  secrets = new Dictionary<string, byte[]>();
        foreach (string username in secretsStrings.Keys) {
            secrets.Add(username, ASCIIEncoding.ASCII.GetBytes(secretsStrings[username])   );
        }
        return secrets;
    }

    public void setSalts(Dictionary<string, byte[]> bytes) {
        saltsStrings = new Dictionary<string, string>();
        foreach (string username in bytes.Keys) {
            saltsStrings.Add(username, System.BitConverter.ToString(bytes[username]));
        }
    }

    public Dictionary<string, byte[]> getSalts() {
        Dictionary<string, byte[]>  salts = new Dictionary<string, byte[]>();
        foreach (string username in saltsStrings.Keys) {
            salts.Add(username, ASCIIEncoding.ASCII.GetBytes(saltsStrings[username]));
        }
        return salts;
    }

    //Ninces are not saved, as they are only for runtime uses.
}
