using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class SaveObject {
    //public List<GameObject> gardenList;
    //public Dictionary<string, string> saltsStrings;
    public List<string> secretStrings;
    public List<string> saltStrings;
    public List<string> usernameStrings;

    public List<Vector3> gardenPos;
    public List<string> gardenOwners;
    public List<GardenBehaviour> gardenListing;
    public List<PlantBehavior> gardenPlants;

    public void setSecretsAndSalts(Dictionary<string, byte[]> newSecrets, Dictionary<string, byte[]> newSalts ) {
        usernameStrings = new List<string>();
        secretStrings = new List<string>();
        saltStrings = new List<string>();
        foreach (string username in newSalts.Keys) {
            usernameStrings.Add(username);
            secretStrings.Add(System.BitConverter.ToString(newSecrets[username]));
            saltStrings.Add(System.BitConverter.ToString(newSalts[username]));
        }
    }

    public Dictionary<string, byte[]> getSecrets() {
        Dictionary<string, byte[]>  secrets = new Dictionary<string, byte[]>();
        for (int it = 0; it < usernameStrings.Count; it++) {
            string username = usernameStrings[it];
            secrets.Add(username, ASCIIEncoding.ASCII.GetBytes(secretStrings[it]));
        }
        return secrets;
    }

    public Dictionary<string, byte[]> getSalts() {
        Dictionary<string, byte[]>  salts = new Dictionary<string, byte[]>();
        for (int it = 0; it < usernameStrings.Count; it++) {
            string username = usernameStrings[it];
            salts.Add(username, ASCIIEncoding.ASCII.GetBytes(saltStrings[it]));
        }
        return salts;
    }

    public void SetGardens() {
        gardenPos = new List<Vector3>();
        gardenOwners = new List<string>();
        gardenListing = new List<GardenBehaviour>();

        //Iterate over gardens
        Debug.Log("Setgardens - gardenCount is " + GardenBehaviour.gardenList.Count);
        foreach (GardenBehaviour gb in GardenBehaviour.gardenList) {

            //Save gardens
            gardenPos.Add(gb.gameObject.transform.position);
            GardenData gd = (GardenData)gb.gameObject.GetComponent(typeof(GardenData));
            gardenOwners.Add(gd.playerName);
            gardenListing.Add(gb); //Plants are part of gb
        }
    }

    public List<GameObject> GetGardens() { 
        //reverse SetGardens
        //TODO also reset gardenCounter for new gardens!
        return null;
    }

    //Nonces are not saved, as they are only for runtime uses.
}
