using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class SaveObject {
    public List<string> secretStrings;
    public List<string> saltStrings;
    public List<string> usernameStrings;

    public List<string> gardenOwners;
    public List<PlantBehavior> gardenPlants;
    public List<int> plantDistribution;

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

        // Information about garden position is implicit - order of the Lists

        gardenOwners = new List<string>();
        plantDistribution = new List<int>();

        //Iterate over gardens
        Debug.Log("Setgardens - gardenCount is " + GardenBehaviour.gardenList.Count);
        foreach (GardenBehaviour gb in GardenBehaviour.gardenList) {

            //Save gardens
            GardenData gd = (GardenData)gb.gameObject.GetComponent(typeof(GardenData));
            gardenOwners.Add(gd.playerName);
            int plantCounter = 0;
            if ( gb.getPlantsInside().Count == 0 ) {
                plantDistribution.Add(plantCounter);
                continue;
            }
            foreach (PlantBehavior pbh in gb.getPlantsInside()) {
                plantCounter++;
                gardenPlants.Add(pbh);
            }
            plantDistribution.Add(plantCounter);
        }
    }

    //Nonces are not saved, as they are only for runtime uses.
}
