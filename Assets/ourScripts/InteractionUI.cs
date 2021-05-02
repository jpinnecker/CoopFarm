using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InteractionUI : NetworkBehaviour {

    // ============================================== Vars

    // What seeds are in the Interface
    private int[] SeedContent = new int[4];
    private int[] SeedCounter = new int[4];
    private int[] buttonStates = new int[7];

    private String spritePath = "UI/Reiter/";
    private bool audioIsOn = true;

    private PlayerState locPlayer;

    public int currentlySelected = -1; // For easy mouse interactions
    public bool inOwnGarden = true;

    private Sprite[,] sprites; // First index is Button Slot, second index is: 0 available, 1 selected, 2 greyed out, 3 locked 
    private Sprite[,] seedSprites; // First index is Button Slot, second index is: 0 available, 1 selected, 2 greyed out, 3 locked 
    private Sprite audioOnSprite, audioOffSprite, lockSprite;

    public Button[] buttons;
    public Button audioButton;

    public void Start() {
        loadSprites();
        int[] int_arr = { 0, 0, 0, 0, 3, 3, 3};
        buttonStates = int_arr;
        updateButtons();
    }

    public void Update() {
        mouseInputHandle();
    }

    // ============================================= Sprites

    public void loadSprites() {
        sprites = new Sprite[7, 4];
        seedSprites = new Sprite[20, 3];
        audioOnSprite = Resources.Load<Sprite>("UI/Ton_an");
        audioOffSprite = Resources.Load<Sprite>("UI/Ton_aus");
        lockSprite = Resources.Load<Sprite>(spritePath + "Reiter_locked");

        sprites[0, 0] = Resources.Load<Sprite>(spritePath + "Reiter_entfernen_normal");
        sprites[0, 1] = Resources.Load<Sprite>(spritePath + "Reiter_entfernen_ausgewählt");
        sprites[0, 2] = Resources.Load<Sprite>(spritePath + "Reiter_entfernen_ausgegraut");
        sprites[1, 0] = Resources.Load<Sprite>(spritePath + "Reiter_Gießkanne_normal");
        sprites[1, 1] = Resources.Load<Sprite>(spritePath + "Reiter_Gießkanne_ausgewählt");
        sprites[1, 2] = Resources.Load<Sprite>(spritePath + "Reiter_ausgegraut");
        sprites[2, 0] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_normal");
        sprites[2, 1] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgewählt");
        sprites[2, 2] = Resources.Load<Sprite>(spritePath + "Reiter_ausgegraut");

        String[] plantNames = { "4-Apfel", "5-Pilz", "6-Kartoffel", "7-Busch", "8-Baum", "9-Birne", "10-Blätter",
            "11-Kaktus", "12-Kobold","13-Lotus", "14-Löwenzahn", "15-Männchen", "16-Orchidee", "17-PinkerPuschel", "18-Radiesschen", "19-Stein", "20-Topfpflanze" };

        seedSprites[0, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        seedSprites[0, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgewählt");
        seedSprites[0, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");
        seedSprites[1, 0] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_normal");
        seedSprites[1, 1] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgewählt");
        seedSprites[1, 2] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgegraut");

        for (int i = 3; i < 20; i++) { // Numbers shifted by 1 - array
            seedSprites[i, 0] = Resources.Load<Sprite>(spritePath + plantNames[i-3] + "Reiter_" + "_normal");
            seedSprites[i, 1] = Resources.Load<Sprite>(spritePath + plantNames[i-3] + "Reiter_" + "_ausgewählt");
            seedSprites[i, 2] = Resources.Load<Sprite>(spritePath + plantNames[i-3] + "Reiter_" + "_ausgegraut");
        }

        sprites[3, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[3, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgewählt");
        sprites[3, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");
        sprites[4, 0] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_normal");
        sprites[4, 1] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgewählt");
        sprites[4, 2] = Resources.Load<Sprite>(spritePath + "Reiter_2-Haufen_ausgegraut");
        sprites[5, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[5, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgewählt");
        sprites[5, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");
        sprites[6, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[6, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgewählt");
        sprites[6, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");

    }

    // ====================================================== Clicked Events

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

    // ============================================== Utility

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

    private void updateButtonSprite(int buttonNr) {
        if (buttonStates[buttonNr] == 3 ) {
            buttons[buttonNr].image.sprite = lockSprite;
        } else {
            buttons[buttonNr].image.sprite = sprites[buttonNr, buttonStates[buttonNr]];
        }
    }

    // ============================================== PlayerState Interaction

    public void greyItem(int nr) {
        buttonStates[nr] = 2;
        updateButtonSprite(nr);
    }

    public void ungreyItem(int nr) {
        buttonStates[nr] = 0;
        updateButtonSprite(nr);
    }

    public void setUnlockedSeeds(SyncList<int> li ) { // greying out items needs to be checked and done after
        int amount = li.Count;
        for ( int offset = 0; offset < 4; offset++ ) {
            if (offset < amount) { // is unlocked

                int plantTypeNr = 0;
                sprites[3 + offset, 0] = seedSprites[plantTypeNr, 0];
                sprites[3 + offset, 1] = seedSprites[plantTypeNr, 1];
                sprites[3 + offset, 2] = seedSprites[plantTypeNr, 2];

                buttonStates[3 + offset] = 0;
            } else {
                buttonStates[3 + offset] = 3;
            }
        }

    }

    public void claimUI(PlayerState refObj) {
        if (locPlayer != null) {
            locPlayer = refObj;
        }
    }

    // ============================================= irregular Buttons

    public void audioButtonPressed() {
        audioIsOn = !audioIsOn;
        if (audioIsOn) {
            audioButton.image.sprite = audioOnSprite;
        } else {
            audioButton.image.sprite = audioOffSprite;
        }
    }


    public void randomGardenPressed() {
        locPlayer.randomGarden();
    }

    // ============================================== Mouse Behaviour

    private void mouseInputHandle() {

        if (Input.GetMouseButton(1) ) { // Right mouse button
            deselectButtons();
            currentlySelected = -1;
            updateButtons();
            Debug.Log("currentlySelected is " + currentlySelected);
        }
    }
    // Left mousebutton is handled by onCLicked events

}
