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

    int wateringCounter = 0;
    int fertilizingCounter = 0;
    double fertilizingCooldown = 0;

    [SerializeField]
    double fertilizingCooldownDuration = 60;
    [SerializeField]
    int wateringsForSeedAward = 10;
    [SerializeField]
    int[] fertilizationsForSeedSlotUnlock = new int[] { 5, 15, 30 };

    public override void OnStartServer()
    {
        UnlockSeedSlot();
    }
    public override void OnStartClient()
    {
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
    [Server]
    private bool PerformCooldownAction(PlantBehavior plant, SyncDictionary<NetworkIdentity, double> cooldowns, double cooldownDuration, CooldownActionDelegate action, ref int counter)
    {
        var netid = plant.gameObject.GetComponent<NetworkIdentity>() as NetworkIdentity;
        var time = NetworkTime.time;
        var cooldownEnd = time - cooldownDuration; // If we haven't watered this plant yet, pretend the cooldown just elapsed.
        cooldowns.TryGetValue(netid, out cooldownEnd);
        if (time + NetworkTime.timeStandardDeviation >= cooldownEnd) // Give some wiggle room for comparison to account for time variation
        {
            action(plant);
            cooldowns[netid] = time + cooldownDuration;
            counter++;
            return true;
        }
        return false;
    }

    [Command]
    public void CmdWaterPlant(PlantBehavior plant)
    {
        var result = PerformCooldownAction(plant, wateringCooldowns, plant.GetWateringCooldown(), p => p.CareFor(), ref wateringCounter);
        if (result)
        {
            wateringCounter++;
            if (wateringCounter >= wateringsForSeedAward)
            {
                AwardSeeds();
                wateringCounter = 0;
            }
        }
    }

    [Command]
    public void CmdFertilizePlant(PlantBehavior plant)
    {
        var time = NetworkTime.time;
        if (time + NetworkTime.timeStandardDeviation >= fertilizingCooldown)
        {
            var result = PerformCooldownAction(plant, fertilizingCooldowns, plant.GetFertilizingCooldown(), p => p.CareFor(2), ref fertilizingCounter);
            if (result)
            {
                fertilizingCounter++;
                CheckSeedSlotUnlocks();
                fertilizingCooldown = time + fertilizingCooldownDuration;
            }
        }
    }

    [Server]
    private void CheckSeedSlotUnlocks()
    {
        for(int i = 0; i < fertilizationsForSeedSlotUnlock.Length; i++)
        {
            if(fertilizingCounter >= fertilizationsForSeedSlotUnlock[i])
            {
                if(seedInventory.Count < i + 2)
                {
                    UnlockSeedSlot();
                }
            }
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
        for (int i = 0; i < seedInventory.Count; i++)
        {
            seedInventory[i] = Random.Range(i * seedTypesPerSlot, (i + 1) * seedTypesPerSlot);
        }
    }

    [Command]
    public void CmdPlantSeed(int seedSlot, Vector2 location)
    {
        if (seedSlot >= seedInventory.Count)
        {
            Debug.LogWarning($"Atempt to plant a seed from slot {seedSlot}, but the player has only {seedInventory.Count} slots unlocked.");
            return;
        }
        if (seedInventory[seedSlot] == -1)
        {
            Debug.LogWarning($"Atempt to plant a seed from slot {seedSlot}, but that slot is empty.");
            return;
        }
        if (currentGarden.netId != ownGarden.netId)
        {
            Debug.LogWarning($"Atempt to plant a seed in a garden that doesn't belong to the acting player.");
            return;
        }
        PlantBehavior plantPrefab = null; // TODO: Get Prefab from plant type table
        var plant = Instantiate(plantPrefab, location, Quaternion.identity, currentGarden.transform);
        NetworkServer.Spawn(plant.gameObject);
    }
}
