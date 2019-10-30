using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public TimeManager currentTime;
    public HumanManager humanManager;
    public ResourceManager resourceManager;
    public BuildingManager buildingManager;

    //-----------constant-settings--------------------
    public const int secondsPerMonth = 5;

    public GameManager (GameObject go) 
    {
        currentTime = new TimeManager(go);
        humanManager = new HumanManager();
        resourceManager = new ResourceManager();
        buildingManager = new BuildingManager();
    }

    public void AddTime (float time)
    {
        time /= secondsPerMonth;
        currentTime.AddMonth(time);
        humanManager.AddAge(time);
        buildingManager.AddAge(time);
        resourceManager.AddTime(time);
    }
}
