using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "action", menuName = "Cards/Movie Action", order = 4)]
public class MovieActionObject : ScriptableObject {
    public string[] humanName;
    public ActionType action;
    public Transform walkTo;
    public ConversationObject conversation;
    public EventHandler.EventType eventType;
    public Behaviour[] toggle;
    public Behaviour[] toggleAfter;
    public bool inheritNextAction;
    public string contextPopUp;
    public Vector3 panPos;
    public float zoomValue;
}

public enum ActionType { Wander, Talk, WalkTo, WaitForEvent, releaseHuman, idle, walkToLastCrop, leavehouse, leaveResource, walkToLastHouse }
public enum Behaviour { MoveHuman, BuildingAge, WoodUI, FoodUI, PlayerInfo, BuildingInfo, Zoom, DragEarth, buildingList, AgeHumanInResource, FoodUpKeep, AgeTree, population }