using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager {

    public int _maxBuildings = 10000;

    public List<Building> buildings;

    public BuildingManager ()
    {
        buildings = new List<Building>();
    }

    public bool AddBuilding(Building building)
    {
        if (buildings.Count < _maxBuildings)
        {
            buildings.Add(building);
            return true;
        }
        return false;
    }

    public bool AddBuilding(string name, GameObject go, Building.BuildingType t) //returns true if can add person else returns false
    {
        if (buildings.Count < _maxBuildings)
        {
            buildings.Add(new Building(name, go, t));
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveBuilding(Building building)
    {
        buildings.Remove(building);
    }

    public void AddAge(float time)
    {
        foreach (Building b in buildings)
        {
            b._age.AddMonth(time);
            if (b._buildingState != Building.BuildingState.idle)
            {
                b.PerformingAction();
            }
        }
    }
}
