using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{



    [Header("Settings")]
    public string buildingName = "";
    public Building building;
    public BuildingObject buildingO;
    public Building.BuildingType buildingType;
    public Transform doorTransform;
    public bool selected = false;
    public GameObject[] speechBubbleParents;
    public GameObject whirlwind;

    private Collider col;
    [HideInInspector]
    public AudioSource buildingAudioSource;

    public void Respawn ()
    {
        transform.position = GetComponentInChildren<SquashHuman>().transform.position;
        GetComponentInChildren<SquashHuman>().transform.localPosition = Vector3.zero;
        if (buildingType == Building.BuildingType.Shrine)
        {
            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().PanCamera(106.5728f, -90.01769f, 55.12761f, true));
            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().Zoom(80));
            Volcano.instance.ShrineBuilt();
        }
        EZCameraShake.CameraShaker.Instance.ShakeOnce(6, 6, 0.1f, 1);
    }

    private void Awake()
    {
        col = this.GetComponent<Collider>();
        buildingAudioSource = GetComponent<AudioSource>();
        //don't want the building noise to play on the first building. Have tagged first house accordingly
    }

    // Use this for initialization
    void Start()
    {
        building = new Building(buildingName, this.gameObject, buildingType);
        GameController.instance.gameManager.buildingManager.AddBuilding(building);
        if (buildingType == Building.BuildingType.House)
        {
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.placedHouse);
            GameController.instance.lastHouse = gameObject;
            GameController.instance.gameManager.humanManager.AddMaxPopulation();
            building._age.AddMonth(30 * 12);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (speechBubbleParents.Length > 0)
        if (speechBubbleParents[0].transform.childCount == 0 && speechBubbleParents[1].transform.childCount == 0)
        {
            transform.GetComponentInChildren<HoverHouse>().ShowPersistent = false;
            if (GameController.instance.GetComponent<MovieController>().isActiveAndEnabled)
                transform.GetComponentInChildren<HoverHouse>().canvas.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0)) Select(false);
    }

    public void Select(bool s)
    {
        selected = s;
        foreach (Human h in building.humans)
        {
            h._age.SetBeingAged(selected);
        }
        building._age.SetBeingAged(selected);
        if (whirlwind != null)
        whirlwind.SetActive(selected);
        if (PlayerController.instance.previousSelection.selected == PlayerController.Selected.human && selected)
        {
            Select(false);
        }
    }

    IEnumerator LeaveHouse (Human h)
    {
        float timer = 0;
        while (timer < 3)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        h._age._go.GetComponent<HumanController>().LeaveHouse();
        yield return new WaitForEndOfFrame();
    }

    public void AddHuman(Human h)
    {
        if (building._buildingType == Building.BuildingType.House)
        {
            building.AddHuman(h);

            buildingAudioSource.PlayOneShot(GameController.instance.addToHouseSound, 1);
        }
    }

    public bool RemoveHuman(Human h)
    {
        if (selected)
        {
            Select(false);
        }
        if (building.RemoveHuman(h))
        {
            h._go.transform.position = doorTransform.position;
            if (h._go.GetComponent<HumanController>().humanState != HumanController.HumanState.movie)
                h._go.GetComponent<HumanController>().humanState = HumanController.HumanState.idle;
            h._go.GetComponent<HumanController>().buildingInside = null;
            h._performingTask = false;
            if (GetComponent<BuildingsUIScript>().humans[0] == h)
            {
                GetComponent<BuildingsUIScript>().TakeOut1(true);
            }
            else
            {
                GetComponent<BuildingsUIScript>().TakeOut2(true);
            }

            buildingAudioSource.PlayOneShot(GameController.instance.addToHouseSound, 1);
            return true;
        }
        return false;
    }

    public void BirthChild(Human father, Human mother)
    {
        GameObject child = Instantiate(GameController.instance.human, doorTransform.position, Quaternion.identity);
        child.GetComponent<HumanController>().CreateHuman(father, mother);
        building._buildingState = Building.BuildingState.idle;
        EventHandler.instance.AddHiddenEvent(EventHandler.EventType.childBorn);
        buildingAudioSource.PlayOneShot(GameController.instance.BabyBorn, 1);
    }

    public IEnumerator WaitForSpace(Human father, Human mother)
    {
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        building._age.AddTaskEvent(new TimeManager.YearMonth(0, 9));
    }
}