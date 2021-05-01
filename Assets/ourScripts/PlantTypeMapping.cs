using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTypeMapping : MonoBehaviour
{
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
}
