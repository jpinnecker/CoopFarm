using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class GardenData : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    public string playerName="";
}
