using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject : MonoBehaviour { 
    public List<GameObject> gardenList;
    public Dictionary<string, byte[]> secrets;
    public Dictionary<string, byte[]> salts;
}
