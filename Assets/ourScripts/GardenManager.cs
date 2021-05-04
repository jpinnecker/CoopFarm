using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class GardenManager : MonoBehaviour
{
    [SerializeField]
    GardenData gardenPrefab;

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
        garden.playerName = playerName;
        NetworkServer.Spawn(garden.gameObject);
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
}
