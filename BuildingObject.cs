using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "buildingObject", menuName = "Cards/Building", order = 2)]
public class BuildingObject : ScriptableObject
{
    [Header("Settings")]
    public new string name;
    public string description;
    public Sprite icon;
    public Building.BuildingType? type;
    public TimeManager.YearMonth timeToBuild;
    public ResourceManager.Resources[] resourceCost;

    [Header("GameObjects")]
    public GameObject MainBuilding;
    public GameObject Hologram;
    public Sprite requiredResourceIcon;
}
