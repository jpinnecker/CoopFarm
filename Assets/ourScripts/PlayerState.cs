using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Text;
using System.Security.Cryptography;

public class PlayerState : NetworkBehaviour
{
    [SerializeField]
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
    private GardenManager gardenManager;

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
        if (seedInventory[0] < 1) {
            seedInventory[0] = 1;
        }
        
        gardenManager = GameObject.FindObjectOfType<GardenManager>();
        if(gardenManager == null) {
            Debug.LogError("Couldn't find GardenManager object.");
        } else {
            gardenManager.gameObject.SetActive(false); // Enabled after Login
        }
    }

    [Server]
    public void findGarden() {
        // TODO: Check when playerName is initialized.

        var myGarden = gardenManager.LookupPlayerGarden(playerName);
        if (myGarden == null) {
            myGarden = gardenManager.CreatePlayerGarden(playerName);

        }
        var myGardenId = myGarden.gameObject.GetComponent<NetworkIdentity>();
        AssignGarden(myGardenId);
        EnterGarden(myGardenId);
    }

    public override void OnStartClient()
    {
        wateringCooldowns.Callback += OnWateringCooldownUpdate;
        fertilizingCooldowns.Callback += OnFertilizingCooldownUpdate;
        seedInventory.Callback += OnInventoryUpdate;

        interacUI = GameObject.FindWithTag("InteracUI").GetComponent(typeof(InteractionUI)) as InteractionUI;
        interacUI.claimUI(this);
        interacUI.gameObject.SetActive(false); // Enabled after Login

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

    public string GetName()
    {
        return playerName;
    }

    [Server]
    public void AssignGarden(NetworkIdentity garden)
    {
        ownGarden = garden;
    }
    [Server]
    public void EnterGarden(NetworkIdentity garden)
    {
        currentGarden = garden;
        // TODO: Place player game object and their camera object in this garden
    }
    [Command]
    public void CmdEnterGarden(NetworkIdentity garden)
    {
        EnterGarden(garden);
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
        seedInventory[seedSlot] = -1;
        UpdateInteracUI();
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
                    SoundManager sm = SoundManager.self;
                    sm.playRandomAudio(0, 3); // Harke 1 - 4
                    Destroy(plant.gameObject);
                }
                break;

            case 1: //WATERING
                //if (currentGarden.netId == ownGarden.netId) {
                if ( false ) { // Showcase/Debug
                    // Debug.LogWarning("Tried to wet their plants!");
                } else {
                    SoundManager sm = SoundManager.self;
                    sm.playRandomAudio(8, 11); // Wasser 1 -4 

                    PerformCooldownAction(plant, wateringCooldowns, 0, CmdWaterPlant, ref wateringCounter);
                    UpdateInteracUI();
                }

                break;

            case 2: //MANURE
                PerformCooldownAction(plant, wateringCooldowns, 0, CmdFertilizePlant, ref wateringCounter);
                UpdateInteracUI();
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
                //Debug.Log(slotNr + " and " + seedInventory.Count + " to " + (slotNr - 3) );
                SoundManager sm = SoundManager.self;
                sm.playRandomAudio(4,7); // Saat 1 - 4
                int seedNr = slotNr - 3;
                if (seedInventory[seedNr] == -1) {
                    Debug.LogWarning("There are no seeds left");
                    return;
                }
                // TODO: Fix transform to world pos
                //Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 25));
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 25));
                float d;
                xy.Raycast(ray, out d);
                Vector3 position = ray.GetPoint(d);
                position.x *= 0.25f; 
                position.y *= 0.25f;
                CmdPlantSeed(seedNr, position);
                seedInventory[seedNr] = -1;
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

        interacUI.setUnlockedSeeds(seedInventory);
        for (int i = 0; i < seedInventory.Count; i++) {
            if (seedInventory[i] == -1 ) {
                interacUI.lockItem(i+3);
            }
        }
        for(int i = seedInventory.Count; i < 4; i++)
        {
            interacUI.greyItem(i + 3);
        }
    }

    public void bgClickDelegate() {
        cmdBackgroundClick();
    }

    public void randomGarden() {
        int max = GardenBehaviour.gardenList.Count;
        int index = Random.Range(0, max ); // Max is exclusive => no index overflow
        gotoGarden(index);
        return; // TODO
    }

    private void gotoGarden(int gardenNr) {
        GardenBehaviour newGarden = GardenBehaviour.gardenList[gardenNr];
        NetworkIdentity netwID = newGarden.gameObject.GetComponent(typeof(NetworkIdentity)) as NetworkIdentity;
        Camera cam = Camera.main;
        cam.transform.position = newGarden.transform.position + new Vector3(0, 0, -17);

        GameObject playerObj = NetworkClient.localPlayer.gameObject;
        playerObj.transform.position = newGarden.transform.position;

    }

    //TODO make server vars:
    private Dictionary<string, string> passwordHashDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> saltHashDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> NonceHashDictionary = new Dictionary<string, string>();


    [Command]
    public void CMDtryLogin(string username, ConnectScript loginScript) {

        byte[] nonce = getChallengeFor(username); //also links the challenge to the username so it can be solved
        byte[] salt = getSalt(username); //getSalt(username); ?
        Debug.Log("CMDtryLogin done");
        loginScript.receiveChallenge(nonce, salt);
    }

    // TODO: make CMD getSalt & TrylogIn use a ClinetCall to change data there, let them use tpye void to avoid weaver error

    [Command]
    public void CMDAnswerChallenge(string username, byte[] challengeAnswer, ConnectScript loginScript) {
        // special thanks to http://csharphelper.com/blog/2014/08/use-a-cryptographic-random-number-generator-in-c/

        byte[] theThing = getPasswordhashEntry(username);
        byte[] challengeBytes = getChallengeFor(username);
        byte[] challengeSolution = ConnectScript.combineByteArrays(theThing, challengeBytes);

        // Compare challenge Answer with solution
        if (challengeAnswer.Length != challengeSolution.Length) {
            return;
        }
        for (int i = 0; i < challengeAnswer.Length; i++) {
            if (challengeAnswer[i] != challengeSolution[i]) {
                return;
            }
        }

        // Accept Login and register as proper user:
        Debug.Log("CMDAnswerChallenge done");
        loginScript.ChallengeAccepted();

        this.gameObject.SetActive(true); //other things to do on successfull login here
    }

    [Server]
    private byte[] getChallengeFor(string username) {
        // The random number provider.
        RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

        byte[] four_bytes = new byte[4];
        Rand.GetBytes( four_bytes );
        
        return four_bytes;
    }

    [Server]
    private byte[] getPasswordhashEntry(string username) { // TODO: replace with actual entrys and stuff
        byte[] returnEntry = new byte[] { 42, 16, 254 };
        return returnEntry;
    }

    private byte[] getSalt(string username) { //TODO: implement
        //ASCIIEncoding.ASCII.GetBytes( string obj ) might be helpful
        return new byte[] { 254, 16, 42 };
    }
}
