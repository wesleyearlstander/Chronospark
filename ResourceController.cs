using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour {

    [HideInInspector]
    public Resource resource;

    [Header("Settings")]
    public ResourceManager.ResourceType resourceType;
    [Range(1,5)]
    public int humanMax = 1;
    [Range(0,100)]
    public int ratePerMonthPP;
    [Range(1, 10)]
    public int manualSkill = 1;
    [Range(1, 10)]
    public int farmingSkill = 1;
    [Range(1,10)]
    public int artisanSkill = 1;
    public int maxResources;
    public Transform gatherTransform;
    public GameObject GroundMarker;
    public GameObject whirlwind;
    [HideInInspector]
    public AudioSource resourceAudioSource;


    public void ActivateGroundMarker (bool b)
    {
        if (!GroundMarker.activeInHierarchy && b == true)
            GroundMarker.SetActive(true);
        else if (GroundMarker.activeInHierarchy && b == false)
        {
            GroundMarker.SetActive(false);
        }
    }

    public void ActivateAgingMarker (bool b)
    {
        if (!whirlwind.activeInHierarchy && b == true)
            whirlwind.SetActive(true);
        else if (whirlwind.activeInHierarchy && b == false)
        {
            whirlwind.SetActive(false);
        }
    }

    private void Awake()
    {
        resourceAudioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        resource = new Resource(this.gameObject, humanMax, resourceType, ratePerMonthPP, maxResources);
        resource.SetSkills(manualSkill, farmingSkill, artisanSkill);
        if (resource.resourceT == ResourceManager.ResourceType.Food)
        {
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.placedFood);
            GameController.instance.lastCrops = gameObject;
            if (GameController.instance.gameManager.currentTime._time.month > 1)
            {
                resourceAudioSource.PlayOneShot(GameController.instance.cropPlace, 1);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerController.instance.currentSelection.go != gameObject)
            ActivateGroundMarker(false);
	}

    public bool AddHuman (GameObject go)
    {
        //Playing aduio for assigning humans to crops and trees
        if (resource.resourceT == ResourceManager.ResourceType.Food)
        {
            resourceAudioSource.PlayOneShot(GameController.instance.farmingAssign, 1);
            if (go.GetComponent<HumanController>().human._ageZone == Human.AgeZone.Teen)
            {
                EventHandler.instance.AddAchievement(Achievement.childrenCorn);
            }
        } else if (resource.resourceT == ResourceManager.ResourceType.Wood)
        {
            resourceAudioSource.PlayOneShot(GameController.instance.woodAssign, 1);
            bool biggest = true;
            foreach (Human h in GameController.instance.gameManager.humanManager.humans)
            {
                if (go.GetComponent<HumanController>().human._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount < h._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount)
                {
                    biggest = false;
                }
            }
            if (biggest && go.GetComponent<HumanController>().human._name != "Frase")
            {
                EventHandler.instance.AddAchievement(Achievement.bestWoodsman);
            }
        }
        go.transform.position = gatherTransform.position;
        go.transform.LookAt(transform);
        ActivateGroundMarker(false);
        return resource.AddHuman(go.GetComponent<HumanController>().human);
    }

    public void DestroyAfterSeconds (float sec)
    {
        Destroy(gameObject, sec);
    } 

    public bool AddHuman (Human h)
    { 
        return AddHuman (h._age._go);
    }
}
