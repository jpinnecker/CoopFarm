using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerState : NetworkBehaviour
{
    private const int seedTypesPerSlot = 5;
    [SyncVar]
    string playerName;
    [SyncVar]
    NetworkIdentity ownGarden;
    [SyncVar]
    NetworkIdentity currentGarden;
    // Plant Identity -> End time of cooldown
    readonly SyncDictionary<NetworkIdentity, double> wateringCooldowns = new SyncDictionary<NetworkIdentity, double>();
    // Seed type numbers, -1 is empty, only unlocked slots
    readonly SyncList<int> seedInventory = new SyncList<int>();

    public override void OnStartServer()
    {
        UnlockSeedSlot();
    }
    public override void OnStartClient() {
        wateringCooldowns.Callback += OnWateringCooldownUpdate;
        seedInventory.Callback += OnInventoryUpdate;
    }

    private void OnWateringCooldownUpdate(SyncIDictionary<NetworkIdentity, double>.Operation op, NetworkIdentity key, double item)
    {
        // TODO: Update visuals
    }

    private void OnInventoryUpdate(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        // TODO: Update UI
    }

    [Command]
    public void CmdSetName(string name)
    {
        this.name = name;
    }

    [Server]
    public void AssignGarden(NetworkIdentity garden)
    {
        ownGarden = garden;
    }
    [Command]
    public void CmdEnterGarden(NetworkIdentity garden)
    {
        currentGarden = garden;
        // TODO: Place player game object in this garden
    }

    [Command]
    public void CmdWaterPlant(PlantBehavior plant) {
        var netid = plant.gameObject.GetComponent<NetworkIdentity>() as NetworkIdentity;
        var time = NetworkTime.time;
        var cooldownEnd = time - plant.GetWateringCooldown(); // If we haven't watered this plant yet, pretend the cooldown just elapsed.
        wateringCooldowns.TryGetValue(netid, out cooldownEnd);
        if(time + NetworkTime.timeStandardDeviation >= cooldownEnd) // Give some wiggle room for comparison to account for time variation
        {
            plant.Water();
            wateringCooldowns[netid] = time + plant.GetWateringCooldown();
        }
    }

    [Server]
    public void UnlockSeedSlot()
    {
        seedInventory.Add(-1);
    }

    [Server]
    public void AwardSeeds()
    {
        for(int i = 0; i < seedInventory.Count; i++)
        {
            seedInventory[i] = Random.Range(i * seedTypesPerSlot, (i + 1) * seedTypesPerSlot);
        }
    }

}
