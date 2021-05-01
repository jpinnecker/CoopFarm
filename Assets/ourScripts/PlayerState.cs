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
    readonly SyncDictionary<NetworkIdentity, double> fertilizingCooldowns = new SyncDictionary<NetworkIdentity, double>();
    // Seed type numbers, -1 is empty, only unlocked slots
    readonly SyncList<int> seedInventory = new SyncList<int>();

    public override void OnStartServer()
    {
        UnlockSeedSlot();
    }
    public override void OnStartClient()
        wateringCooldowns.Callback += OnWateringCooldownUpdate;
        fertilizingCooldowns.Callback += OnFertilizingCooldownUpdate;
        seedInventory.Callback += OnInventoryUpdate;
    }

    private void OnWateringCooldownUpdate(SyncIDictionary<NetworkIdentity, double>.Operation op, NetworkIdentity key, double item)
    {
        // TODO: Update visuals?
    }
    private void OnFertilizingCooldownUpdate(SyncIDictionary<NetworkIdentity, double>.Operation op, NetworkIdentity key, double item)
    {
        // TODO: Update visuals?
    }

    private void OnInventoryUpdate(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        // TODO: Update UI
    }

    [Command]
    public void CmdSetName(string name) // May become a normal [Server] method
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

    private delegate void CooldownActionDelegate(PlantBehavior plant);
    private void PerformCooldownAction(PlantBehavior plant, SyncDictionary<NetworkIdentity, double> cooldowns, double cooldownDuration, CooldownActionDelegate action)
    {
        var netid = plant.gameObject.GetComponent<NetworkIdentity>() as NetworkIdentity;
        var time = NetworkTime.time;
        var cooldownEnd = time - cooldownDuration; // If we haven't watered this plant yet, pretend the cooldown just elapsed.
        cooldowns.TryGetValue(netid, out cooldownEnd);
        if (time + NetworkTime.timeStandardDeviation >= cooldownEnd) // Give some wiggle room for comparison to account for time variation
        {
            action(plant);
            cooldowns[netid] = time + cooldownDuration;
        }

    }

    [Command]
    public void CmdWaterPlant(PlantBehavior plant)
    {
        PerformCooldownAction(plant, wateringCooldowns, plant.GetWateringCooldown(), p => p.CareFor());
    }

    [Command]
    public void CmdFertilizePlant(PlantBehavior plant)
    {
        PerformCooldownAction(plant, fertilizingCooldowns, plant.GetFertilizingCooldown(), p => p.CareFor(2));
    }

    [Server]
    public void UnlockSeedSlot()
    {
        seedInventory.Add(-1);
    }

    [Server]
    public void AwardSeeds()
    {
        for (int i = 0; i < seedInventory.Count; i++)
        {
            seedInventory[i] = Random.Range(i * seedTypesPerSlot, (i + 1) * seedTypesPerSlot);
        }
    }

}
