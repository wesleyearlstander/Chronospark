using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class MovieController : MonoBehaviour {

    public string movieName;
    public List<MovieActionObject> list;
    public Queue<MovieActionObject> actions;

    private List<MovieActionObject> currentActions;
    private bool waitingForAction = false;
    private List<bool> waiting = new List<bool> ();

    public GameObject dialogueBox;
    public Text text;
    public GameObject groundMarkerBuild;

    public PostProcessingProfile nonVignette, vignette;

	// Use this for initialization
	void Start () {
        currentActions = new List<MovieActionObject>();
        actions = new Queue<MovieActionObject>();
        foreach (MovieActionObject m in list)
        {
            actions.Enqueue(m);
        }
        GameController.instance.tutorialCheck = true;
        GetComponent<AudioSource>().clip = GameController.instance.saltMash;
        GetComponent<AudioSource>().Play();
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = vignette;
        UIScript.instance.achievementPanel.SetActive(false);
    }

    float timer = 0;
    float waitTime = 12;
    string context;
	
	// Update is called once per frame
	void Update () {
		if (actions.Count > 0)
        {
            if (!waitingForAction)
            {
                currentActions.Clear();
                context = "";
                MovieActionObject temp;
                do
                {
                    temp = actions.Peek();
                    currentActions.Add(temp);
                    actions.Dequeue();
                } while (temp.inheritNextAction);
                waitingForAction = true;
                int count = 0;
                foreach (MovieActionObject m in currentActions)
                {
                    if (m.contextPopUp != "")
                    {
                        context = m.contextPopUp;
                    } 
                    if (m.panPos != Vector3.zero)
                    {
                        if (m.action == ActionType.Talk && m.eventType == EventHandler.EventType.cameraPanned)
                            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().PanCamera(m.panPos.x, m.panPos.y, m.panPos.z, true));
                        else
                            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().PanCamera(m.panPos.x, m.panPos.y, m.panPos.z, false));
                    }
                    if (m.zoomValue != 0)
                    {
                        StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().Zoom(m.zoomValue));
                    }
                    switch (m.action)
                    {
                        case ActionType.Wander:
                            {
                                Wander(ConvertNamesToHumans(m.humanName), m);
                                break;
                            }
                        case ActionType.WalkTo:
                            {
                                count += WalkTo(ConvertNamesToHumans(m.humanName), m);
                                int rewind = 1;
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    StartCoroutine(WaitForHumanToGoIdle(h, count - rewind));
                                    rewind++;
                                }
                                break;
                            }
                        case ActionType.WaitForEvent:
                            {
                                count++;
                                StartCoroutine(WaitForEvent(m.eventType,count-1));
                                if (m.eventType == EventHandler.EventType.placedFood || m.eventType == EventHandler.EventType.placedHouse)
                                {
                                    GameController.instance.placementSpot = m.walkTo.position;
                                    if (!groundMarkerBuild.activeInHierarchy)
                                    {
                                        groundMarkerBuild.SetActive(true);
                                        groundMarkerBuild.transform.position = GameController.instance.placementSpot;
                                    }
                                }
                                break;
                            }
                        case ActionType.Talk:
                            {
                                count += TalkTo(ConvertNamesToHumans(m.humanName), m);
                                int rewind = 1;
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    StartCoroutine(WaitForHumanToGoIdle(h, count - rewind));
                                    rewind++;
                                }
                                break;
                            }
                        case ActionType.releaseHuman:
                            {
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    h._go.GetComponent<HumanController>().humanState = HumanController.HumanState.idle;
                                    Camera.main.GetComponent<PostProcessingBehaviour>().profile = nonVignette;
                                }
                                break;
                            }
                        case ActionType.walkToLastCrop:
                            {
                                count += WalkTo(ConvertNamesToHumans(m.humanName), m);
                                int rewind = 1;
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    StartCoroutine(WaitForHumanToGoIdle(h, count - rewind));
                                    rewind++;
                                }
                                break;
                            }
                        case ActionType.walkToLastHouse:
                            {
                                count += WalkTo(ConvertNamesToHumans(m.humanName), m);
                                int rewind = 1;
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    StartCoroutine(WaitForHumanToGoIdle(h, count - rewind));
                                    rewind++;
                                }
                                break;
                            }
                        case ActionType.leavehouse:
                            {
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    h._age._go.GetComponent<HumanController>().LeaveHouse();
                                }
                                break;
                            }
                        case ActionType.leaveResource:
                            {
                                foreach (Human h in ConvertNamesToHumans(m.humanName))
                                {
                                    h._age._go.GetComponent<HumanController>().LeaveResource();
                                }
                                break;
                            }
                    }
                    Toggle(m.toggle);
                }
                for (int i = 0; i < count; i++)
                {
                    waiting.Add(false);
                }
            } else
            {
                bool done = false;
                timer += Time.deltaTime;
                bool negative = false;
                if (waiting.Count == 0)
                    done = true;
                foreach(bool b in waiting)
                {
                    if (b && !negative)
                    {
                        done = true;
                    } else
                    {
                        negative = true;
                        done = false;
                    }
                }
                if (timer > waitTime && context != "")
                {
                    dialogueBox.GetComponent<Animator>().SetBool("Show", true);
                    if (dialogueBox.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "DialogueBoxAnim")
                    text.text = context;
                    waitTime = 0.5f;
                }
                if (done)
                {
                    dialogueBox.GetComponent<Animator>().SetBool("Show", false);
                    timer = 0;
                    Debug.Log("finished");
                    waitingForAction = false;
                    waiting.Clear();
                    foreach (MovieActionObject m in currentActions)
                        Toggle(m.toggleAfter);
                }
            }
        } else
        {
            EventHandler.instance.AddAchievement(Achievement.tutorialDone);
            GetComponent<AudioSource>().clip = GameController.instance.normalClip;
            GetComponent<AudioSource>().Play();
            UIScript.instance.achievementPanel.SetActive(true);
            enabled = false;
        }
	}

    void Toggle(Behaviour[] behaviours)
    {
        foreach (Behaviour b in behaviours)
        {
            switch (b)
            {
                case Behaviour.BuildingAge:
                    {
                        GameController.instance.canBuildingAge = !GameController.instance.canBuildingAge;
                        break;
                    }
                case Behaviour.BuildingInfo:
                    {
                        GameController.instance.canBuildingInfo = !GameController.instance.canBuildingInfo;
                        break;
                    }
                case Behaviour.DragEarth:
                    {
                        GameController.instance.canDragEarth = !GameController.instance.canDragEarth;
                        break;
                    }
                case Behaviour.FoodUI:
                    {
                        GameController.instance.canFoodUI = !GameController.instance.canFoodUI;
                        break;
                    }
                case Behaviour.WoodUI:
                    {
                        GameController.instance.canWoodUI = !GameController.instance.canWoodUI;
                        break;
                    }
                case Behaviour.Zoom:
                    {
                        GameController.instance.canZoom = !GameController.instance.canZoom;
                        break;
                    }
                case Behaviour.MoveHuman:
                    {
                        GameController.instance.canMoveHuman = !GameController.instance.canMoveHuman;
                        break;
                    }
                case Behaviour.PlayerInfo:
                    {
                        GameController.instance.canPlayerInfo = !GameController.instance.canPlayerInfo;
                        break;
                    }
                case Behaviour.buildingList:
                    {
                        GameController.instance.canBuildingList = !GameController.instance.canBuildingList;
                        break;
                    }
                case Behaviour.AgeHumanInResource:
                    {
                        GameController.instance.canAgeInResource = !GameController.instance.canAgeInResource;
                        break;
                    }
                case Behaviour.FoodUpKeep:
                    {
                        GameController.instance.canFoodUpKeep = !GameController.instance.canFoodUpKeep;
                        break;
                    }
                case Behaviour.AgeTree:
                    {
                        GameController.instance.canAgeTree = !GameController.instance.canAgeTree;
                        break;
                    }
                case Behaviour.population:
                    {
                        GameController.instance.canpopulationLimit = !GameController.instance.canpopulationLimit;
                        break;
                    }
            }
        }
    }

    IEnumerator WaitForHumanToGoIdle (Human h, int waitingIndex)
    {
        bool done = false;
        do
        {
            if (h._go.GetComponent<HumanController>().movieAction == ActionType.idle)
            {
                done = true;
            }
            yield return new WaitForEndOfFrame();
        } while (!done);
        waiting[waitingIndex] = true;
        yield return new WaitForEndOfFrame();
    }

    IEnumerator WaitForEvent (EventHandler.EventType t, int waitingIndex)
    {
        bool done = false;
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = nonVignette;
        if (t == EventHandler.EventType.placedFood)
        {
            GameController.instance.gameManager.resourceManager.AddResources(new ResourceManager.Resources(600, ResourceManager.ResourceType.Food));
            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().PanCamera(51.78f,131,-51.4f,false));
        } else if (t == EventHandler.EventType.placedHouse)
        {
            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().PanCamera(51.33f,139,-23.12f,false));
        }
        float timer = 3;
        
        while (!done)
        {
            timer += Time.deltaTime;
            if (timer >= 3 && t == EventHandler.EventType.populationClicked)
            {
                timer = 0;
                StartCoroutine(UIScript.instance.FlashPopulationPanel(UIScript.Option.population, Color.white));
            }
            if (timer >= 3 && (t == EventHandler.EventType.placedHouse || t == EventHandler.EventType.placedFood))
            {
                timer = 0;
                StartCoroutine(UIScript.instance.FlashPopulationPanel(UIScript.Option.building, Color.white));
            }

            if (EventHandler.instance.CheckForHiddenEvent(t))
            {
                done = true;
            }
            yield return new WaitForEndOfFrame();
        }
        waiting[waitingIndex] = true;
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = vignette;
        yield return new WaitForEndOfFrame();
    }

    Human[] ConvertNamesToHumans (string[] names)
    {
        Human[] humans = new Human[names.Length];
        int count = 0;
        foreach (string s in names)
        {
            humans[count] = GameController.instance.gameManager.humanManager.FindHumanByName(s);
            count++;
        }
        return humans;
    }

    int Wander (Human[] humans, MovieActionObject m)
    {
        int count = 0;
        foreach (Human h in humans)
        {
            h._go.GetComponent<HumanController>().humanState = HumanController.HumanState.movie;
            h._go.GetComponent<HumanController>().StartCoroutine("Wander", m);
            count++;
        }
        return count;
    }

    int WalkTo (Human[] humans, MovieActionObject m)
    {
        int count = 0;
        foreach (Human h in humans)
        {
            h._go.GetComponent<HumanController>().humanState = HumanController.HumanState.movie;
            h._go.GetComponent<HumanController>().StartCoroutine("WalkTo", m);
            count++;
        }
        return count;
    }

    int TalkTo (Human[] humans, MovieActionObject m)
    {
        int count = 0;
        foreach (Human h in humans)
        {
            h._go.GetComponent<HumanController>().humanState = HumanController.HumanState.movie;
            h._go.GetComponent<HumanController>().StartCoroutine("TalkTo", m);
            count++;
        }
        return count;
    }
}
