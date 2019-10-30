using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController instance;
    public GameManager gameManager;

    [Header("Prefabs")]
    public List<TraitObject> humanTraits;
    public GameObject human;
    public List<BuildingObject> buildingObjects;

    [Header("Settings")]
    public Material[] eyeColours;
    public Material[] hairColours;
    public Material[] skinColours;
    public GameObject speechBubble;
    public GameObject[] tombStone;
    public GameObject stump;
    public Material greyHair;
    public GameObject bloodSplatter;
    [HideInInspector]
    public Vector3 placementSpot;
    public AchievementCard[] achievements;
    [HideInInspector]
    public int cropDeadCounter = 0, raisedChildren = 0;

    [Header("Sounds")]
    public AudioClip BabyBorn;
    public AudioClip deathSound, treeDeathSound, woodAssign, farmingAssign, breedingAssign, learningAssign, addToHouseSound, removeFromHouseSound, removeFromWork, buildingPlace, cropPlace, ageChange, buildingDrop, buildingSquash, personLand, lightSwitch;

    public GameObject button;
    public GameObject lavaExplosion;

    public AudioClip saltMash;
    public AudioClip normalClip;
    public AudioClip finalClip;

    //Game Variables
    [HideInInspector]
    public bool playerPaused = false, gamePaused = false; //pause settings - extend as necessary

    [HideInInspector]
    public bool canMoveHuman = true, canBuildingAge = true, canWoodUI = true, canFoodUI = true, canPlayerInfo = true, canBuildingInfo = true, canZoom = true, canDragEarth = true, canBuildingList = true, canAgeInResource = true, canFoodUpKeep = true, canAgeTree = true, canpopulationLimit = true, panning = false, zooming = false, tutorialCheck = false, canZoomPop = false;

    [HideInInspector]
    public GameObject lastCrops, lastHouse, lastHumanPopUp;
    void Awake()
    {
        if (instance == null)
            instance = this;
        gameManager = new GameManager(this.gameObject);
        canFoodUpKeep = true;
    }

    // Use this for initialization
    void Start () {
        
    }

    float cropTimer = 0;
    public GameObject dialogueBox;
    public Text text;
	
	// Update is called once per frame
	void Update () {
        Timers();
        if (!GetComponent<MovieController>().isActiveAndEnabled) {
            if (gameManager.resourceManager.peopleOnCrops == 0)
            {
                cropTimer += Time.deltaTime;
                if (cropTimer > 5)
                {
                    if (!gameManager.humanManager.starving)
                    {
                        dialogueBox.GetComponent<Animator>().SetBool("Show", true);
                        text.text = "Assign your farmers to crops to sustain your food upkeep.";
                    } else
                    {
                        dialogueBox.GetComponent<Animator>().SetBool("Show", true);
                        text.text = "Assign your best farmers to crops to prevent your people from starving!";
                    }
                }
            }
            else
            {
                if (gameManager.humanManager.starving)
                {
                    dialogueBox.GetComponent<Animator>().SetBool("Show", true);
                    text.text = "Assign your best farmers to crops to prevent your people from starving!";
                } else
                {
                    cropTimer = 0;
                    dialogueBox.GetComponent<Animator>().SetBool("Show", false);
                }
            }
        }
	}

    //timer variables
    void Timers ()
    {
        float time = Time.deltaTime;
        if (!playerPaused && !gamePaused)
        {
            gameManager.AddTime(time);
        }
    }
}