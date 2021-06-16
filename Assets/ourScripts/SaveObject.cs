using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class SaveObject {
    public List<GameObject> gardenList;
    public Dictionary<string, string> secretsStrings;
    public Dictionary<string, string> saltsStrings;

    private List<Vector3> gardenPos;
    private List<string> gardenOwners;

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

    public void SetGardens() {

        //Iterate over gardens
        foreach (GardenBehaviour gb in GardenBehaviour.gardenList) {

            //Save gardens
            gardenPos.Add(gb.gameObject.transform.position);
            GardenData gd = (GardenData)gb.gameObject.GetComponent(typeof(GardenData));
            gardenOwners.Add(gd.playerName);
        }

        
        //Iterate over all plants
        //Assign the plants to gardens
        //Save plants as list of transformation, position of garden first list element
        //Add List of Plant names
        //Save this positionList
    }

    public List<GameObject> GetGardens() {
        //reverse SetGardens
        return null;
    }

    //Ninces are not saved, as they are only for runtime uses.
}
