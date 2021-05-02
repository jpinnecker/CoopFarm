using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class GardenManager : MonoBehaviour
{
    [SerializeField]
    GardenData gardenPrefab;

    public GardenData LookupPlayerGarden(string playerName)
    {
        return transform.Cast<Transform>().FirstOrDefault(c => c.gameObject.GetComponent<GardenData>().playerName == playerName)?.gameObject?.GetComponent<GardenData>();
    }

    public GardenData CreatePlayerGarden(string playerName)
    {
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
        // TODO: Implement spiral positioning from current child count.
        return new Vector3(0, 0, 25);
    }
}
