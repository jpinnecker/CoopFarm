using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class PlantBehavior : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnGrowth))]
    int growthStage = 0;
    [SerializeField]
    double wateringCooldownSeconds = 60;
    [SerializeField]
    double fertilizingCooldownSeconds = 60;
    [SerializeField]
    int caresForGrowth = 7;

    int currentCares = 0;

    public double GetWateringCooldown()
    {
        return wateringCooldownSeconds;
    }

    public double GetFertilizingCooldown()
    {
        return fertilizingCooldownSeconds;
    }

    // For testing in editor
    [ContextMenu("Care for")]
    public void Care()
    {
        CareFor();
    }
    [ContextMenu("Care for (7)")]
    public void Care7()
    {
        CareFor(7);
    }

    [Server]
    public void CareFor(int amount = 1) {
        currentCares += amount;
        if(currentCares >= caresForGrowth)
        {
            growthStage++;
            currentCares = 0;
        }
    }

    void OnGrowth(int oldStage, int newStage)
    {
        var body = transform.Find("body");
        body.GetComponent<Animator>().SetInteger("growthStage",newStage);
    }
}
