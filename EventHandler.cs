using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour {

    public static EventHandler instance;

    public Queue<TimeEvent> events;
    [HideInInspector]
    public List<EventType> hiddenEvents;
    public List<EventType> bufferEvents;

    public List<Achievement> currentAchievements;
    
    public enum EventType { panCamera, viewHumanInfo, ZoomCameraInfull, ZoomCameraOut, doneTeaching, placedFood, GatheredWoodForHouse, placedHouse, childBorn, cameraPanned, populationClicked }

    public void AddHiddenEvent (EventType t)
    {
        bufferEvents.Add(t);
    }

    public bool CheckForHiddenEvent (EventType t)
    {
        if (!hiddenEvents.Contains(t))
        {
            return bufferEvents.Contains(t);
        } else
        {
            return true;
        }
    }

    public void AddAchievement (Achievement a)
    {
        if (currentAchievements.Contains(a))
        {
            
        } else
        {
            currentAchievements.Add(a);
            AchievementDisplayer.instance.displayAchievement(a);
        }
    }

    public void RemoveAllHiddenEventType (EventType t)
    {
        while (hiddenEvents.Remove(t)) ;
    }

    public struct TimeEvent
    {
        public TimeManager.TimeEvent timeEvent;
        public GameObject go;

        public TimeEvent (TimeManager.TimeEvent t, GameObject g)
        {
            timeEvent = t;
            go = g;
        }

        public override string ToString()
        {
            return go.name + " " + timeEvent.ToString();
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        events = new Queue<TimeEvent>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        HandlePublicEvents();
        AddBufferToHidden();
        if (frameBuffer == 1)
        {
            bufferEvents.Clear();
        } else if (frameBuffer == 2)
        {
            hiddenEvents.Clear();
            frameBuffer = 0;
        }
        frameBuffer++;
        
    }

    int frameBuffer = 0;

    void AddBufferToHidden ()
    {
        foreach (EventType e in bufferEvents)
        {
            hiddenEvents.Add(e);
        }
    }
 
    void HandlePublicEvents()
    {
        while (events.Count > 0)
        {
            TimeEvent tevent = events.Peek();
            GameObject go = tevent.go;
            bool eventHandled = false;
            if (go.GetComponent<HumanController>() != null) //human event called
            { 
                Human human = go.GetComponent<HumanController>().human;
                if (tevent.timeEvent.type == TimeManager.EventType.Created)
                {
                    //Debug.Log(human.ToString() + " born");
                    AddHiddenEvent(EventType.childBorn);
                    eventHandled = true;
                }
                else if (tevent.timeEvent.type == TimeManager.EventType.End)
                {
                    tevent.go.GetComponent<HumanController>().Die(true);
                    Debug.Log(human.ToString() + " died");
                    eventHandled = true;
                    //Destroy(go);
                } else if (tevent.timeEvent.type == TimeManager.EventType.Error)
                {
                    Debug.LogError(human.ToString() + " error occured");
                    eventHandled = true;
                } else if (tevent.timeEvent.type == TimeManager.EventType.AgeZoneUp)
                {
                    human._ageZone = (Human.AgeZone)((int)human._ageZone + 1);
                    if (GameController.instance.gameManager.currentTime._time > new TimeManager.YearMonth(0,1))
                    human._age._go.GetComponent<AudioSource>().PlayOneShot(GameController.instance.ageChange);
                    eventHandled = true;
                } else if (tevent.timeEvent.type == TimeManager.EventType.TaskCompleted)
                {
                    human._taskDone = true;
                    human.multiplier = human._age.multiplier;
                    eventHandled = true;
                }
            }
            else if (go.GetComponent<GameController>() != null) //main time event called
            {
                if (tevent.timeEvent.type == TimeManager.EventType.Created)
                {
                    //Debug.Log("Game Started");
                    eventHandled = true;
                }
            } else if (go == null)
            {
                Debug.LogError("game object missing");
                eventHandled = false;
            } else if (go.GetComponent<ResourceController>() != null) //resource event called
            {
                if (tevent.timeEvent.type == TimeManager.EventType.Created)
                {
                    eventHandled = true;
                }
            } else if (go.GetComponent<BuildingController>() != null) //building event called
            {
                if (tevent.timeEvent.type == TimeManager.EventType.Created)
                {
                    eventHandled = true;
                } else if (tevent.timeEvent.type == TimeManager.EventType.TaskCompleted)
                {
                    tevent.go.GetComponent<BuildingController>().building._taskDone = true;
                    eventHandled = true;
                }
            }
            //event handled or not, it will be removed
            if (eventHandled == false)
            {
                Debug.LogError("Event not handled: EventHandler: " + tevent.ToString());
            }
            events.Dequeue();
        }
    }

    public void AddPublicEvent (TimeEvent timeEvent)
    {
        events.Enqueue(timeEvent);
    }

    public void AddPublicEvent(TimeManager.TimeEvent timeEvent, GameObject go)
    {
        events.Enqueue(new TimeEvent(timeEvent, go));
    }

    public void AddPublicEvent(TimeManager.EventType eventType, TimeManager.YearMonth yearMonth, bool isPublic, GameObject go)
    {
        AddPublicEvent(new TimeManager.TimeEvent(eventType, yearMonth, isPublic), go);
    }

    public void AddPublicEvent(TimeManager.EventType eventType, int year, float month, bool isPublic, GameObject go)
    {
        AddPublicEvent(eventType, new TimeManager.YearMonth(year, month), isPublic, go);
    }
}
