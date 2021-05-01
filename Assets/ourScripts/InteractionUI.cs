using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InteractionUI : NetworkBehaviour
{

    // What plants have been watered
    //public SyncDictionary<UnityEngine.Object, DateTime> WateringTimes = new SyncDictionary<UnityEngine.Object, DateTime>();

    // What seeds are in the Interface
    private int[] SeedContent = new int[4];
    private int[] SeedCounter = new int[4];
    private int[] buttonStates = new int[7];

    private String spritePath = "UI/Reiter/";
    private bool audioIsOn = true;


    public int currentlySelected = -1; // For easy mouse interactions
    public bool inOwnGarden = true;

    private Sprite[,] sprites; // First index is Button Slot, second index is: 0 available, 1 selected, 2 greyed out, 3 locked 
    private Sprite audioOnSprite, audioOffSprite;

    public Button[] buttons;
    public Button audioButton;

    public void Start() {
        loadSprites();
    }

    public void Update() {
        if (Input.GetMouseButton(1) ) { // Right mouse button
            deselectButtons();
            currentlySelected = -1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    // ============================================= Button selection management

    public void loadSprites() {
        sprites = new Sprite[7, 4];
        audioOnSprite = Resources.Load<Sprite>("UI/Ton_an");
        audioOffSprite = Resources.Load<Sprite>("UI/Ton_aus");

        sprites[0, 0] = Resources.Load<Sprite>(spritePath + "Reiter_entfernen_normal");
        sprites[0, 1] = Resources.Load<Sprite>(spritePath + "Reiter_entfernen_ausgew‰hlt");
        sprites[0, 2] = Resources.Load<Sprite>(spritePath + "Reiter_entfernen_ausgegraut");
        sprites[1, 0] = Resources.Load<Sprite>(spritePath + "Reiter_Gieﬂkanne_normal");
        sprites[1, 1] = Resources.Load<Sprite>(spritePath + "Reiter_Gieﬂkanne_ausgew‰hlt");
        sprites[1, 2] = Resources.Load<Sprite>(spritePath + "Reiter_ausgegraut");
        sprites[2, 0] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_normal");
        sprites[2, 1] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgew‰hlt");
        sprites[2, 2] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgegraut");

        sprites[3, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[3, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgew‰hlt");
        sprites[3, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");
        sprites[4, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[4, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgew‰hlt");
        sprites[4, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");

    }

    public void onShovelClicked() { // button index 0
        if (buttonStates[0] == 0) {
            deselectButtons();
            currentlySelected = 0;
            buttonStates[0] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    public void onCanClicked() { // button index 1
        if (buttonStates[1] == 0) {
            deselectButtons();
            currentlySelected = 1;
            buttonStates[1] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    public void onDungClicked() { // button index 2
        if (buttonStates[2] == 0) {
            deselectButtons();
            currentlySelected = 2;
            buttonStates[2] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    public void onSeed1() { // button index 3
        if (buttonStates[3] == 0) {
            deselectButtons();
            currentlySelected = 3;
            buttonStates[3] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }
     
    public void onSeed2() { // button index 4
        if (buttonStates[4] == 0) {
            deselectButtons();
            currentlySelected = 4;
            buttonStates[4] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    public void onSeed3() { // button index 5
        if (buttonStates[5] == 0) {
            deselectButtons();
            currentlySelected = 5;
            buttonStates[5] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    public void onSeed4() { // button index 6
        if (buttonStates[6] == 0) {
            deselectButtons();
            currentlySelected = 6;
            buttonStates[6] = 1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }

    private void deselectButtons() {
        for (int i = 0; i < 6; i++) {
            if (buttonStates[i] == 1) {
                buttonStates[i] = 0;
            }
        }
    }

    private void updateButtons() {
        for (int i = 0; i < 6; i++) {
            updateButtonSprite(i);
        }
    }

    private void updateButtonSprite( int buttonNr) {
        buttons[buttonNr].image.sprite = sprites[buttonNr, buttonStates[buttonNr] ];
    }

    public void audioButtonPressed() {
        audioIsOn = !audioIsOn;
        if (audioIsOn) {
            audioButton.image.sprite = audioOnSprite;
        } else {
            audioButton.image.sprite = audioOffSprite;
        }
    }

    /*
    private void tryWatering(UnityEngine.Object plantID) {
        DateTime timestamp = DateTime.UtcNow;

        try { // In case there is no entry for this plant yet

            if (DateTime.Compare(timestamp, WateringTimes[plantID]) < 0) {  // not enough time has passed = cannot be watered again

                return;
            } // no return -> watering triggers after try block
        
        } catch (ArgumentException) { // No entry yet
            WateringTimes.Add(plantID, timestamp); // Just to have a new entry, time will be adjusted
        }


        // TODO increase water status
        WateringTimes[plantID] = timestamp.AddHours(24); // Next possible watering time
        return;
    }
    */
}
