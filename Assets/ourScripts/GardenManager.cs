using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Mirror;

public class GardenManager : NetworkBehaviour {

    private static int updateCount = 0;

    [Server]
    public void FixedUpdate() {

        if (hasAuthority) {
            Debug.Log("Why tho?");
        }

        updateCount++;
        if (updateCount == 25) {
            saveByJSON();
        } else if (updateCount == 150)  {
            loadByJSON();
        }
    }

    [SerializeField]
    GardenData gardenPrefab;

    private static List<GameObject> gardenList = new List<GameObject>();

    private static int gardenCounter = 0;
    private static int curLayerSize = 1;
    private static int garden_width = 5; // TODO adjust
    private static int garden_height = 3; // TODO adjust

    public GardenData LookupPlayerGarden(string playerName)
    {
        return transform.Cast<Transform>().FirstOrDefault(c => c.gameObject.GetComponent<GardenData>().playerName == playerName)?.gameObject?.GetComponent<GardenData>();
    }

    public GardenData CreatePlayerGarden(string playerName)
    {
        gardenCounter++;
        var garden = Instantiate(gardenPrefab, GenerateGardenPosition(), Quaternion.identity, transform);
        gardenList.Add( garden.gameObject );
        garden.playerName = playerName;
        NetworkServer.Spawn(garden.gameObject);
        GardenBehaviour.gardenList.Add(garden.GetComponent<GardenBehaviour>());
        return garden;
    }

    public GardenData PickRandomGarden()
    {
        if (transform.childCount == 0) return null;
        var randomIndex = Random.Range(0, transform.childCount - 1);
        return transform.GetChild(randomIndex).gameObject.GetComponent<GardenData>();
    }

    private Vector3 GenerateGardenPosition()
    {
        // gardenCounter includes new garden
        if ( gardenCounter >  curLayerSize * curLayerSize ) {
            curLayerSize += 2;
        }

        // Used to navigate around in layer
        int posInLayer = gardenCounter - curLayerSize * curLayerSize; // Position of the new Garden in the layer
        if (curLayerSize == 1) { // 0 division error 
            return new Vector3(0, 0, 25); 
        }
        int halfSize = (curLayerSize - 1) / 2;
        int loopCounter = 0; // Keeps track of our position in one direction/vector of the layer
        int positionCounter = 0; // We look at this position in the layer

        // Used to keep track of current position
        int x_position = halfSize;
        int y_position = 0;

        // We just walk all paths of the layer until we stop
        while (positionCounter < posInLayer && loopCounter < halfSize) {
            y_position++;
            loopCounter++;
        }
        loopCounter = 0;

        while (positionCounter < posInLayer && loopCounter < curLayerSize) {
            x_position--;
            loopCounter++;
        }
        loopCounter = 0;

        while (positionCounter < posInLayer && loopCounter < curLayerSize) {
            y_position--;
            loopCounter++;
        }
        loopCounter = 0;

        while (positionCounter < posInLayer && loopCounter < halfSize) {
            x_position++;
            loopCounter++;
        }
        //We made a full square around gardens before

        x_position *= garden_width;
        y_position *= garden_height;

        return new Vector3(x_position, y_position, 25);
    }

    private static string directory = "/SaveData/";
    private static string nameBase = "garden_saves";

    //[Command]
    public void loadByJSON() {

        //Check directory and file existence
        string dir = Application.persistentDataPath + directory;
        if (!Directory.Exists(dir)) {
            Debug.LogError("Save Path invalid");
        }

        int nrElementsInFolder = 1; // TODO implement

        string filePath;
        if (nrElementsInFolder > 9 ) {
            filePath = dir + nameBase + nrElementsInFolder + ".json";
        } else {
            filePath = dir + nameBase + "0" + nrElementsInFolder + ".json";
        }

        if (!File.Exists(filePath) ) {
            Debug.LogError("save file not detected. Are there more elements than save files in the folder?");
            return;
        }

        //Load SaveObject and apply changes
        SaveObject so = JsonUtility.FromJson<SaveObject>(dir);

        //GardenManager.gardenList = so.gardenList;
        PlayerState.setSaveData(so);

        foreach (GameObject go in gardenList) {
            go.transform.parent = this.gameObject.transform; // Necessary?
        } 
    }

    //[Command]
    public void saveByJSON() {
        //Make directory if necessary
        string dir = Application.persistentDataPath + directory;
        Debug.Log("Save directory is " + dir);

        if (!Directory.Exists(dir)) {
            Debug.Log("Created new");
            Directory.CreateDirectory(dir);
        }

        //For calculating name
        int nrElementsInFolder = 0; // TODO implement

        //Save as SaveObject
        SaveObject so = PlayerState.getSaveData();
        //so.gardenList = GardenManager.gardenList;

        Debug.Log(so.ToString());
        //Debug.Log(so.gardenList.ToString());
        //Debug.Log(so.secretStrings.ToString());
        //Debug.Log(so.saltsStrings.ToString());

        Debug.Log("Temmies Secret is " + so.getSecrets()["Temmie"].ToString());
        Debug.Log("Temmies Salt is " + so.getSalts()["Temmie"].ToString());
        //Debug.Log("Temmies garden is " + so.saltsStrings["Temmie"].ToString());

        string json = JsonUtility.ToJson(so);
        Debug.Log(json);
        File.WriteAllText(dir + nameBase + nrElementsInFolder+1  + ".json", json);
    }
}
