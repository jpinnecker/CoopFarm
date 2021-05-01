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

    // Update is called once per frame
    void Update()
    {
        // TODO: Maybe needed growing logic
    }

    public double GetWateringCooldown()
    {
        return wateringCooldownSeconds;
    }

    public double GetFertilizingCooldown()
    {
        return fertilizingCooldownSeconds;
    }

    [Server]
    public void CareFor(int amount = 1) {
        // TODO: Exact Behavior, when to grow
    }

    void OnGrowth(int oldStage, int newStage)
    {
        // TODO: Notify Animation
    }
}
