using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class playerController : NetworkBehaviour
{
    private float movespeed = 0.2f;
    private bool isBlocked = false;

    // Is outside of camera elsewise
    public void Start() {
        transform.position = new Vector3(0f, 0f, 0f);
    }

    public void changeBlock() {
        isBlocked = !isBlocked;
    }

    public void Update() {
        if (! isBlocked) {
            handleMovement();
        }
    }

    // Movement/Input behaviour
    void handleMovement()
    {
        if (isLocalPlayer) {
            float x_movement = Input.GetAxis("Horizontal");
            float y_movement = Input.GetAxis("Vertical");
            transform.position += new Vector3(x_movement*movespeed, y_movement*movespeed, 0f);
        }
    }
}
