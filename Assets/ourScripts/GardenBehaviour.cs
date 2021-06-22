using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GardenBehaviour : NetworkBehaviour {
    public static List<GardenBehaviour> gardenList = new List<GardenBehaviour>();
    private int gardenNr = -1;
    private List<PlantBehavior> plantsInside = new List<PlantBehavior>();

    private static int x_core_dist = 100;
    private static int y_core_dist = 100;
    private static int rowLength = 5;
    private static int colLength = 10;
    private static int section_dist = 150;

    void ServerStart() {
        // Find own gardenNr
        gardenList.Add(this);
        gardenNr = 0;
        while (true) {
            if (gardenList[gardenNr] == this) {
                break;
            } else {
                gardenNr++;
            }
        }

        findPosition();
    }

    [Server]
    private void findPosition() {
        int row_offset = gardenNr % rowLength;
        int col_offset = (gardenNr / 5) % colLength;
        int section_offset = gardenNr / (rowLength * colLength);
        Vector3 position = new Vector3(row_offset * x_core_dist, col_offset * y_core_dist + section_offset * section_dist, 0);
        transform.position = position;
    }

    public Boolean containsLocation(Vector2 loc) { //TDO pray that no transformationproblems occur
        Vector3 pos = gameObject.transform.position;
        if (loc.x < pos.x + x_core_dist / 2 &&
            loc.x > pos.x - x_core_dist / 2 &&
            loc.y < pos.y + y_core_dist / 2 &&
            loc.y > pos.y - y_core_dist / 2) {

            return true;
        } else {
            return false;
        }
    }

    public void addPlant(PlantBehavior plant) {
        plantsInside.Add(plant);
    }

    public int getGardenNr() {
        return gardenNr;
    }

    [Server]
    public List<PlantBehavior> getPlantsInside() {
        return plantsInside;
    }
}
