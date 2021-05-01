using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTypeMapping : MonoBehaviour
{
    [SerializeField]
    int seedTypesPerSlot = 5;
    [SerializeField]
    PlantBehavior[] plantTypePrefabs = new PlantBehavior[20];

    public PlantBehavior GetPlantTypePrefab(int type)
    {
        if (type < plantTypePrefabs.Length)
        {
            return plantTypePrefabs[type];
        }
        else
        {
            Debug.LogError($"Non-existent plant type {type} requested.");
            return null;
        }
    }

    public int GetSeedTypesPerSlot()
    {
        return seedTypesPerSlot;
    }
}
