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
    private Sprite audioOnSprite, audioOffSprite;

    public Button[] buttons;
    public Button audioButton;

    public void Start() {
        loadSprites();
    }

    public void Update() {
        mouseInputHandle();
    }

    // ============================================= Sprites

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
        sprites[5, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[5, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgew‰hlt");
        sprites[5, 2] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgegraut");
        sprites[6, 0] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_normal");
        sprites[6, 1] = Resources.Load<Sprite>(spritePath + "Reiter_1-Kohl_ausgew‰hlt");
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

    public void audioButtonPressed() {
        audioIsOn = !audioIsOn;
        if (audioIsOn) {
            audioButton.image.sprite = audioOnSprite;
        } else {
            audioButton.image.sprite = audioOffSprite;
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
        buttons[buttonNr].image.sprite = sprites[buttonNr, buttonStates[buttonNr]];
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

    public void setUnlockedSeeds(int amount) { // greying out items needs to be checked and done after
        for ( int offset = 0; offset < 4; offset++ ) {
            if (offset < amount) { // is unlocked
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
