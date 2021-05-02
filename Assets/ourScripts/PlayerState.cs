using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerState : NetworkBehaviour
{
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
    
    private InteractionUI interacUI;

    [SerializeField]
    double fertilizingCooldownDuration = 60;
    [SerializeField]
    int wateringsForSeedAward = 10;
    [SerializeField]
    int[] fertilizationsForSeedSlotUnlock = new int[] { 5, 15, 30 };

    // Cached references:
    private PlantTypeMapping plantTypeMapping;

    void Start()
    {
        plantTypeMapping = GameObject.FindObjectOfType<PlantTypeMapping>();
        if (plantTypeMapping == null)
        {
            Debug.LogError("Couldn't find PlantTypeMapping object.");
        }
    }

    public override void OnStartServer()
    {
        UnlockSeedSlot();
    }
    public override void OnStartClient()
    {
        wateringCooldowns.Callback += OnWateringCooldownUpdate;
        fertilizingCooldowns.Callback += OnFertilizingCooldownUpdate;
        seedInventory.Callback += OnInventoryUpdate;

        interacUI = GameObject.FindWithTag("InteracUI").GetComponent(typeof(InteractionUI)) as InteractionUI;
        interacUI.claimUI(this);

        /* TEST VARS
        GameObject playerObj = NetworkClient.localPlayer.gameObject;
        ownGarden = playerObj.GetComponent(typeof(NetworkIdentity)) as NetworkIdentity;
        currentGarden = ownGarden;
        */
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
        // TODO: Place player game object and their camera object in this garden
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
        var seedTypesPerSlot = plantTypeMapping.GetSeedTypesPerSlot();
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
        PlantBehavior plantPrefab = plantTypeMapping.GetPlantTypePrefab(seedInventory[seedSlot]);
        var plant = Instantiate(plantPrefab, location, Quaternion.identity, currentGarden.transform);
        NetworkServer.Spawn(plant.gameObject);
    }

    [Command]
    public void cmdPlantInteraction(PlantBehavior plant) { // Does NOT cover new plant
        int toolNr = interacUI.currentlySelected;
        switch (toolNr) { 
            case -1: // No tool selected
                return;

            case 0: //SHOVEL
                if (currentGarden.netId != ownGarden.netId) {
                    Debug.LogWarning("Tried to use shovel on foreign soil!");
                } else {
                    Destroy(plant.gameObject);
                }
                break;

            case 1: //WATERING
                if (currentGarden.netId == ownGarden.netId) {
                    Debug.LogWarning("Tried to wet their plants!");
                } else {
                    PerformCooldownAction(plant, wateringCooldowns, 0, CmdWaterPlant, ref wateringCounter);
                    UpdateInteracUI();
                }

                break;

            case 2: //MANURE
                if (currentGarden.netId == ownGarden.netId) {
                    Debug.LogWarning("Tried to dung their plants");
                } else {
                    PerformCooldownAction(plant, wateringCooldowns, 0, CmdFertilizePlant, ref wateringCounter);
                    UpdateInteracUI();
                }
                break;

            case 3: //SEED 1
            case 4: //SEED 2
            case 5: //SEED 3
            case 6: //SEED 4
                Debug.LogWarning("Cannot plant here - there is already another plant.");
                break;

            default:
                Debug.Log("An Error has occured - currentlySelected int from InteractionUI is wrong");
                break;
        }
    }

    [Command]
    public void cmdBackgroundClick() { // Used to plant new plants
        int slotNr = interacUI.currentlySelected;
        switch (slotNr) {
            case -1: // No tool selected
                return;

            case 0: //SHOVEL
            case 1: //WATERING
            case 2: //MANURE
                Debug.LogWarning("Cannot use thsi tool here - there is nothing to interact with");
                return;

            case 3: //SEED 1
            case 4: //SEED 2
            case 5: //SEED 3
            case 6: //SEED 4

                int seedNr = slotNr - 3;
                if (seedInventory[slotNr - 3] <= 0) {
                    Debug.LogWarning("There are no seeds left");
                    return;
                }

                Vector3 position = Input.mousePosition;
                CmdPlantSeed(slotNr - 3, position);
                seedInventory[slotNr] -= 1;
                UpdateInteracUI();
                return;

            default:
                Debug.Log("An Error has occured - currentlySelected int from InteractionUI is wrong");
                break;
        }
    }

    public void UpdateInteracUI() {

        if (currentGarden.netId == ownGarden.netId) {
            interacUI.greyItem(0);
            interacUI.ungreyItem(1);
        } else {
            interacUI.greyItem(1);
            interacUI.ungreyItem(0);
        }
        interacUI.greyItem(2);

        interacUI.setUnlockedSeeds(seedInventory.Count);
        for (int i = 0; i < 4; i++) {
            if (seedInventory[i] <= 0 ) {
                interacUI.greyItem(i+3);
            }
        }
    }
}
