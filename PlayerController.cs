using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public enum Selected { human = 2, building = 1, planet = 0, resource = 3, construction = 4, planetVoid = 5, tombstone = 6, lava = 7 } // extend as necessary
    
    public enum PlayerState { normal, building }
    [HideInInspector]
    public PlayerState playerState = PlayerState.normal;
    [HideInInspector]
    public BuildingObject currentConstructionO;
    [HideInInspector]
    public GameObject currentConstruction;

    public bool placingConstruct = false;
    
    public struct Selection
    {
        public Selected? selected;
        public GameObject go;
    }

    public Selection currentSelection, previousSelection;

    [Header("Prefabs")]
    public Color GroundMarkerGreen;
    public Color GroundMarkerRed;
    public Color GroundMarkerBlue;
    public GameObject GroundMarker;

    public GameObject currentGroundMarker;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        currentSelection.selected = null;
        
    }

    IEnumerator FadeCurrentGroundMarker ()
    {
        float timer = 0;
        Color col = currentGroundMarker.GetComponentInChildren<RFX4_EffectSettingColor>().GetComponent<ParticleSystem>().main.startColor.color;
        GameObject fadeMarker = currentGroundMarker;
        currentGroundMarker = null;
        while (timer < 2.5f)
        {
            Color temp = Color.Lerp(col, new Color(0, 0, 0, 0), timer / 2.5f);
            ParticleSystem.MainModule c = fadeMarker.GetComponentInChildren<RFX4_EffectSettingColor>().GetComponent<ParticleSystem>().main;
            ParticleSystem.MainModule t = fadeMarker.GetComponentInChildren<RFX4_Turbulence>().GetComponent<ParticleSystem>().main;
            c.startColor = temp;
            t.startColor = temp;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (timer  < 5f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(fadeMarker);
        yield return new WaitForEndOfFrame();
    }
    // Update is called once per frame
    void Update()
    {
        MouseOver();
        CanClick();
        Clicked();
    }

    public void ConstructionCancel ()
    {
        Destroy(currentConstruction); 
        playerState = PlayerState.normal;
        placingConstruct = false;
        foreach (Human h in GameController.instance.gameManager.humanManager.humans)
        {
            h._age._go.GetComponent<HumanController>().SetLastPositionForTombstone(true);
        }
        foreach (ResourceManager.Resources r in currentConstructionO.resourceCost)
            GameController.instance.gameManager.resourceManager.AddResources(r);
        clickCheck = false;
        clickCheck2 = false;
    }

    public void ConstructionClick (BuildingObject b)
    {
        if (GameController.instance.gameManager.resourceManager.EnoughResources(b.resourceCost) && playerState == PlayerState.normal)
        {
            GameController.instance.gameManager.resourceManager.DeductResources(b.resourceCost);
            currentConstructionO = b;
            currentConstruction = Instantiate(b.Hologram, Vector3.zero, Quaternion.identity);
            Vector3 gravityUp = (Vector3.zero - currentConstruction.transform.position).normalized; //up vector from centre of earth
            Vector3 bodyUp = currentConstruction.transform.up;
            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp * -1) * currentConstruction.transform.rotation; //corrects up vector for location on planet
            currentConstruction.transform.rotation = targetRotation;
            foreach (Human h in GameController.instance.gameManager.humanManager.humans)
            {
                h._age._go.GetComponent<HumanController>().SetLastPositionForTombstone(false);
            }
            playerState = PlayerState.building;
            placingConstruct = true;
            if (UIScript.instance.GetComponentInChildren<BuildingMenu>().GetComponent<RectTransform>().position.x < -20) {
                clickCheck = false;
            }
            else
                clickCheck = true;
            clickCheck2 = false;
        }
    }

    void MouseOver()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
        if (playerState != PlayerState.building)
        {
            if (previousSelection.selected == Selected.human)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Humans")))
                {
                    SetCurrentSelection(hit.collider.gameObject);
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    SetCurrentSelection(hit.collider.gameObject);
                }
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                SetCurrentSelection(hit.collider.gameObject);
            }
        }
    }

    void SetCurrentSelection(GameObject go)
    {
        if (currentSelection.go != null && go.GetComponent<ResourceController>() == null)
        {
            if (currentSelection.go.GetComponent<ResourceController>() != null)
            {
                currentSelection.go.GetComponent<ResourceController>().ActivateGroundMarker(false);
            }
        }
        if (go.GetComponent<HumanController>() != null ) //human
        {
            if (!go.GetComponent<HumanController>().drowning)
            {
                currentSelection.go = go;
                currentSelection.selected = Selected.human;
            } else
            {
                currentSelection.go = null;
                currentSelection.selected = null;
            }
        }
        else if (go.transform.root.GetComponent<BuildingController>() != null) //building
        {
            currentSelection.go = go.transform.root.gameObject;
            currentSelection.selected = Selected.building;
        }
        else if (go.GetComponent<GameController>() != null) //planet
        {
            currentSelection.go = go;
            currentSelection.selected = Selected.planet;
        }
        else if (go.transform.root.GetComponent<ResourceController>() != null) //resource deposit
        {
            currentSelection.go = go.transform.root.gameObject;
            currentSelection.selected = Selected.resource;
        }
        else if (go.GetComponent<tombstoneController>() != null)
        {
            currentSelection.go = go;
            currentSelection.selected = Selected.tombstone;
        }
        else
        {
            currentSelection.go = null;
            currentSelection.selected = null;
            if (go.transform.name == "LavaCollider")
            {
                currentSelection.go = go;
                currentSelection.selected = Selected.lava;
            }
            else if (go.transform.parent.name == "volcano")
            {
                currentSelection.go = go;
                currentSelection.selected = Selected.planetVoid;
            } else if (go.transform.name == "PlanetVoidCollider")
            {
                currentSelection.go = go.transform.gameObject;
                currentSelection.selected = Selected.planetVoid;
            }
            else if (go.transform.parent != null)
            {
                if (go.transform.parent.name == "PlanetVoidCollider")
                {
                    currentSelection.go = go.transform.gameObject;
                    currentSelection.selected = Selected.planetVoid;
                }
            } 
        }
    }

    public void ClearPreviousSelection ()
    {
        previousSelection.selected = null;
        previousSelection.go = null;
    }

    public void SetPreviousSelection (GameObject go)
    {
        previousSelection.selected = Selected.human;
        previousSelection.go = go;
        previousSelection.go.GetComponent<HumanController>().Select(true);
        if (previousSelection.go.GetComponent<HumanController>().resourceBeingGathered != null)
        {
            previousSelection.go.GetComponent<HumanController>().resourceBeingGathered._age._go.GetComponent<ResourceController>().ActivateAgingMarker(true);
        }
        lastValidLocation = previousSelection.go.transform.position;
    }

    void CanClick() //pointing at object
    {
        if (!GameController.instance.panning)
        {
            switch (currentSelection.selected)
            {

                case Selected.human:
                    {
                        //mousing over human
                        if (Input.GetMouseButtonDown(0)) //clicked on human
                        {
                            if (GameController.instance.canMoveHuman || GameController.instance.canAgeInResource)
                                currentSelection.go.transform.root.GetComponent<HumanController>().Select(true);
                            previousSelection = currentSelection;
                            currentSelection.go.GetComponent<HumanController>().startingPoint = currentSelection.go.transform.position;
                            lastValidLocation = currentSelection.go.transform.position;
                        }
                        break;
                    }
                case Selected.building:
                    {
                        if (GameController.instance.canBuildingAge)
                        {
                            if (Input.GetMouseButtonDown(0)) //clicked on building
                            {
                                currentSelection.go.transform.root.GetComponent<BuildingController>().Select(true);
                                previousSelection = currentSelection;
                            }
                        }
                        break;
                    }
                case Selected.resource:
                    {
                        if (GameController.instance.canAgeTree)
                        {
                            if (Input.GetMouseButtonDown(0)) //clicked on resource
                            {
                                if (currentSelection.go.transform.root.GetComponentInChildren<TreeController>() != null) //tree
                                {
                                    previousSelection = currentSelection;
                                }
                                else //food
                                {
                                    previousSelection = currentSelection;
                                }
                            }
                        }
                        break;
                    }
                case Selected.planet:
                    {

                        break;
                    }
                case Selected.tombstone:
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            currentSelection.go.GetComponent<tombstoneController>().selected = true;
                            previousSelection = currentSelection;
                        }
                        break;
                    }
            }
        }
    }

    bool clickCheck = false;
    bool clickCheck2 = false;

    void Clicked ()
    {
        if (playerState == PlayerState.building)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layer_mask = LayerMask.GetMask("Planet"); //raycasting on the planet
            if (currentConstruction != null)
            {
                currentConstruction.transform.localEulerAngles = new Vector3(currentConstruction.transform.localEulerAngles.x, -Camera.main.transform.root.localEulerAngles.z, currentConstruction.transform.localEulerAngles.z);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
                {
                    currentConstruction.transform.position = hit.point;
                    currentConstruction.transform.localEulerAngles = new Vector3(currentConstruction.transform.localEulerAngles.x, -Camera.main.transform.root.eulerAngles.z, currentConstruction.transform.localEulerAngles.z);
                    if (Input.GetMouseButtonUp(0) && clickCheck) clickCheck2 = true;
                    if (Input.GetMouseButtonUp(0)) clickCheck = true;
                    if (Input.GetMouseButtonUp(0) && currentConstruction.GetComponent<BuildingHelper>().canPlace && clickCheck2)
                    {
                        GameObject temp;
                        if (currentConstructionO.MainBuilding.GetComponent<BuildingController>() != null)
                        {
                            temp = Instantiate(currentConstructionO.MainBuilding, currentConstruction.transform.position + ((currentConstruction.transform.position - GameController.instance.transform.position) * 1.2f), currentConstruction.transform.rotation);
                        }
                        else
                            temp = Instantiate(currentConstructionO.MainBuilding, currentConstruction.transform.position, currentConstruction.transform.rotation);
                        Destroy(currentConstruction);
                        currentConstruction = temp;
                        temp.GetComponent<AudioSource>().PlayOneShot(GameController.instance.buildingPlace, 1);
                        currentConstructionO = null;
                        playerState = PlayerState.normal;
                        placingConstruct = false;
                        if (GameController.instance.GetComponent<MovieController>().groundMarkerBuild.activeInHierarchy)
                        {
                            GameController.instance.GetComponent<MovieController>().groundMarkerBuild.SetActive(false);
                        }
                    }

                }
            }
        }
        else
        {
            switch (previousSelection.selected) //clicked already
            {
                case Selected.human:
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        int layer_mask = LayerMask.GetMask("Planet"); //raycasting on the planet
                        if (previousSelection.go.GetComponent<HumanController>().resourceBeingGathered != null)
                            SetCurrentSelection(previousSelection.go.GetComponent<HumanController>().resourceBeingGathered._age._go);
                        if (currentSelection.selected == Selected.building || currentSelection.selected == Selected.planetVoid || currentSelection.selected == Selected.lava)
                        {
                            Physics.Raycast(ray, out hit, Mathf.Infinity);
                        } else Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask);
                        if (currentGroundMarker != null) 
                            currentGroundMarker.transform.position = hit.point;
                        previousSelection.go.GetComponent<HumanController>().followPoint = hit.point;
                        if (!GameController.instance.canMoveHuman)
                        {
                            if (currentGroundMarker != null)
                            {
                                StartCoroutine(FadeCurrentGroundMarker());
                            }
                        }
                        if (currentSelection.selected != Selected.resource && currentSelection.selected != Selected.building)
                        {
                            if (hit.transform != null && GameController.instance.canMoveHuman)
                            {
                                if (hit.collider.gameObject.GetComponent<GameController>() != null)
                                {
                                    SwitchMarker(MarkerColour.Green);
                                }
                                else
                                {
                                    SwitchMarker(MarkerColour.Red);
                                }
                            }
                        }
                        else //human being dragged but mouse leaves the edges of the planet
                        {

                        }
                        if (currentSelection.selected == Selected.lava && GameController.instance.canMoveHuman)
                        {
                            SwitchMarker(MarkerColour.Green);
                            if (Input.GetMouseButtonUp(0) )
                            {
                                previousSelection.go.GetComponent<HumanController>().Select(false);
                                previousSelection.selected = null;
                                if (currentGroundMarker != null)
                                {
                                    StartCoroutine(FadeCurrentGroundMarker());
                                }
                            }
                        } else if (currentSelection.selected == Selected.planetVoid && GameController.instance.canMoveHuman)
                        {
                            SwitchMarker(MarkerColour.Red);
                            if (Input.GetMouseButtonUp(0)) //lift mouse on planet void
                            {
                                autoDrop = true;
                            }
                        } else if (currentSelection.selected == Selected.tombstone && GameController.instance.canMoveHuman)
                        {
                            SwitchMarker(MarkerColour.Red);
                            if (Input.GetMouseButtonUp(0)) //lift mouse on tombstone
                            {
                                autoDrop = true;
                            }
                        }
                        else if (currentSelection.selected == Selected.human && GameController.instance.canMoveHuman)
                        {
                            SwitchMarker(MarkerColour.Red);
                            if (Input.GetMouseButtonUp(0)) //lift mouse on human
                            {
                                autoDrop = true;
                            }
                        } else if (currentSelection.selected == Selected.planet && GameController.instance.canMoveHuman)
                        {
                            SwitchMarker(MarkerColour.Green);
                            if (Input.GetMouseButtonUp(0)) //lift mouse on planet
                            {
                                previousSelection.go.GetComponent<HumanController>().Select(false);
                                previousSelection.selected = null;
                                if (currentGroundMarker != null)
                                {
                                    StartCoroutine(FadeCurrentGroundMarker());
                                }
                            }
                        }
                        else if (currentSelection.selected == Selected.resource)
                        {
                            if (currentSelection.go.transform.root.GetComponent<ResourceController>().resource.CanAddHuman(previousSelection.go.GetComponent<HumanController>().human) || currentSelection.go.transform.root.GetComponent<ResourceController>().resource.humans.Contains(previousSelection.go.GetComponent<HumanController>().human))
                            {
                                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                                {
                                    float checkRange = 2.8f;
                                    if (currentSelection.go.transform.root.GetComponent<TreeController>() != null)
                                        checkRange *= currentSelection.go.transform.root.GetComponent<ResourceController>().GroundMarker.transform.GetChild(0).localScale.x / 2.8f;
                                    if (hit.collider.gameObject == currentSelection.go)
                                        currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateGroundMarker(true);
                                    if ((hit.point - currentSelection.go.transform.position).magnitude < checkRange || hit.collider.gameObject == previousSelection.go)
                                    {
                                        if (previousSelection.go.GetComponent<HumanController>().resourceBeingGathered != null)
                                        {
                                            if (previousSelection.go.GetComponent<HumanController>().resourceBeingGathered._age._go == currentSelection.go)
                                            {
                                                if (currentGroundMarker != null)
                                                {
                                                    StartCoroutine(FadeCurrentGroundMarker());
                                                }
                                                if (Input.GetMouseButtonDown(0) && GameController.instance.canAgeInResource && hit.collider.gameObject == previousSelection.go)
                                                {
                                                    currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateGroundMarker(true);
                                                    currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateAgingMarker(true);
                                                }
                                                if ((Input.GetMouseButtonUp(0) && GameController.instance.canAgeInResource) || !GameController.instance.canAgeInResource) //lift mouse on resource
                                                {
                                                    if (previousSelection.go.GetComponent<HumanController>().selected)
                                                        previousSelection.go.GetComponent<HumanController>().Select(false);
                                                    currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateAgingMarker(false);
                                                    previousSelection.selected = null;
                                                }
                                            }
                                        }
                                        else if(GameController.instance.canMoveHuman) //placing of humans in ring
                                        {
                                            SwitchMarker(MarkerColour.Blue);
                                            if (Input.GetMouseButtonUp(0)) //lift mouse on resource
                                            {
                                                if (currentSelection.go.transform.root.GetComponent<ResourceController>().resource.CanAddHuman(previousSelection.go.GetComponent<HumanController>().human))
                                                {
                                                    StartCoroutine(previousSelection.go.GetComponent<HumanController>().DescendToResource(currentSelection.go.transform.root.GetComponent<ResourceController>().resource));
                                                    if (currentSelection.go.GetComponent<TreeController>() != null)
                                                        previousSelection.go.GetComponent<HumanController>().axe.SetActive(true);
                                                    else
                                                        previousSelection.go.GetComponent<HumanController>().farmingTool.SetActive(true);
                                                    currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateGroundMarker(false);
                                                    previousSelection.selected = null;
                                                    if (currentGroundMarker != null)
                                                    {
                                                        StartCoroutine(FadeCurrentGroundMarker());
                                                    }
                                                }
                                            }
                                        }
                                    } else if (GameController.instance.canMoveHuman || GameController.instance.canAgeInResource)
                                    {
                                        if (GameController.instance.canMoveHuman)
                                        {
                                            SwitchMarker(MarkerColour.Red);
                                            if (previousSelection.go.GetComponent<HumanController>().resourceBeingGathered != null)
                                            {
                                                currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateGroundMarker(false);
                                                previousSelection.go.GetComponent<HumanController>().resourceBeingGathered._age._go.GetComponent<ResourceController>().ActivateAgingMarker(false);
                                                previousSelection.go.GetComponent<HumanController>().LeaveResource();
                                            }
                                        }
                                        if (GameController.instance.canAgeInResource && previousSelection.go.GetComponent<HumanController>().selected && GameController.instance.GetComponent<MovieController>().isActiveAndEnabled)
                                        {
                                            previousSelection.go.GetComponent<HumanController>().Select(false);
                                            currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateAgingMarker(false);

                                            previousSelection.selected = null;
                                        }
                                        if (Input.GetMouseButtonUp(0))
                                        {
                                            if (previousSelection.go.GetComponent<HumanController>().selected)
                                                previousSelection.go.GetComponent<HumanController>().Select(false);
                                            currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateAgingMarker(false);
                                            previousSelection.selected = null;
                                            if (currentGroundMarker != null)
                                            {
                                                StartCoroutine(FadeCurrentGroundMarker());
                                            }
                                        }
                                    }
                                }
                                else if (GameController.instance.canMoveHuman)
                                {
                                    SwitchMarker(MarkerColour.Red);
                                    if (Input.GetMouseButtonUp(0))
                                    {
                                        previousSelection.go.GetComponent<HumanController>().Select(false);
                                        currentSelection.go.transform.root.GetComponent<ResourceController>().ActivateGroundMarker(false);
                                        if (previousSelection.go.GetComponent<HumanController>().resourceBeingGathered != null)
                                            previousSelection.go.GetComponent<HumanController>().LeaveResource();
                                        previousSelection.selected = null;
                                        if (currentGroundMarker != null)
                                        {
                                            StartCoroutine(FadeCurrentGroundMarker());
                                        }
                                    }
                                }
                            }
                            else if (GameController.instance.canMoveHuman && previousSelection.go.GetComponent<HumanController>().resourceBeingGathered == null)
                            {
                                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                                {
                                    if ((hit.point - currentSelection.go.transform.position).magnitude < 3f)
                                        SwitchMarker(MarkerColour.Red);
                                    else
                                        SwitchMarker(MarkerColour.Green);
                                    if (hit.collider.GetComponent<ResourceController>()!=null)
                                    {
                                        hit.collider.GetComponent<ResourceController>().ActivateGroundMarker(true);
                                    }
                                }
                                if (Input.GetMouseButtonUp(0))
                                {
                                    previousSelection.go.GetComponent<HumanController>().Select(false);
                                    previousSelection.selected = null;
                                    if (currentGroundMarker != null)
                                    {
                                        StartCoroutine(FadeCurrentGroundMarker());
                                    }
                                }
                            }
                        }
                        else if (currentSelection.selected == Selected.building && GameController.instance.canMoveHuman)
                        {
                            if (currentSelection.go.transform.root.GetComponent<BuildingController>().building.CanAddHuman())
                            {
                                SwitchMarker(MarkerColour.Blue);
                                if (Input.GetMouseButtonUp(0))
                                {
                                    currentSelection.go.transform.root.GetComponent<BuildingController>().AddHuman(previousSelection.go.GetComponent<HumanController>().human);
                                    previousSelection.go.GetComponent<HumanController>().buildingInside = currentSelection.go.transform.root.GetComponent<BuildingController>().building;
                                    previousSelection.go.GetComponent<HumanController>().humanState = HumanController.HumanState.inBuilding;
                                    previousSelection.go.GetComponent<HumanController>().Select(false);
                                    previousSelection.selected = null;
                                    if (currentGroundMarker != null)
                                    {
                                        StartCoroutine(FadeCurrentGroundMarker());
                                    }
                                }
                            }
                            else
                            {
                                SwitchMarker(MarkerColour.Red);
                                if (Input.GetMouseButtonUp(0))
                                {
                                    previousSelection.go.GetComponent<HumanController>().Select(false);
                                    previousSelection.selected = null;
                                    if (currentGroundMarker != null)
                                    {
                                        StartCoroutine(FadeCurrentGroundMarker());
                                    }
                                }
                            }
                        } else
                        {
                            if (Input.GetMouseButtonUp(0)) //lift mouse on human
                            {
                                autoDrop = true;
                            }
                        }
                        break;
                    }
                case Selected.building:
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            previousSelection.go.transform.root.GetComponent<BuildingController>().Select(false);
                            previousSelection.selected = null;
                        }
                        break;
                    }
                case Selected.tombstone:
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        int layerMask = ~LayerMask.NameToLayer("Tombstone");
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                        {
                            if (currentGroundMarker != null)
                                currentGroundMarker.transform.position = hit.point;
                            previousSelection.go.GetComponent<tombstoneController>().followPoint = hit.point;
                            previousSelection.go.GetComponent<tombstoneController>().beam.SetActive(true);
                        }if (currentSelection.selected == Selected.planet || (currentSelection.selected == Selected.planetVoid && currentSelection.go.name == "PlanetVoidCollider") || currentSelection.selected == Selected.building)
                        {
                            SwitchMarker(MarkerColour.Green);
                            if (Input.GetMouseButtonUp(0)) //lift mouse on planet
                            {
                                previousSelection.go.GetComponent<tombstoneController>().selected = false;
                                previousSelection.selected = null;
                                previousSelection.go.GetComponent<tombstoneController>().beam.SetActive(false);
                                if (currentGroundMarker != null)
                                {
                                    StartCoroutine(FadeCurrentGroundMarker());
                                }
                            }
                        }
                        else
                        {
                            SwitchMarker(MarkerColour.Red);
                            if (Input.GetMouseButtonUp(0))
                                autoDrop = true;
                        }
                        break;
                    }
                case Selected.resource:
                    {
                        if (previousSelection.go.GetComponent<TreeController>() != null)
                        {
                            if (Input.GetMouseButtonDown(0))
                                previousSelection.go.GetComponent<TreeController>().Select(true);
                            if (Input.GetMouseButtonUp(0))
                            {
                                previousSelection.go.GetComponent<TreeController>().Select(false);
                                previousSelection.selected = null;
                            }
                        }
                        break;
                    }
            }
        }
        if (markerColour == MarkerColour.Green && previousSelection.selected != null)
        {
            lastValidLocation = previousSelection.go.transform.position;
        }
        if (autoDrop)
        {
            if (previousSelection.go.GetComponent<HumanController>() != null)
                previousSelection.go.GetComponent<HumanController>().Select(false);
            else if (previousSelection.go.GetComponent<tombstoneController>() != null)
            {
                previousSelection.go.GetComponent<tombstoneController>().selected = false;
                previousSelection.go.GetComponent<tombstoneController>().beam.SetActive(false);
                previousSelection.selected = null;
                if (currentGroundMarker != null)
                {
                    StartCoroutine(FadeCurrentGroundMarker());
                }
            }
            previousSelection.selected = null;
            if (currentGroundMarker != null)
            {
                StartCoroutine(FadeCurrentGroundMarker());
            }
            previousSelection.go.transform.position = lastValidLocation;
            autoDrop = false;
        }
        //switch ()
    }

    enum MarkerColour { Green, Red, Blue }
    MarkerColour markerColour = MarkerColour.Green;
    bool autoDrop = false;
    Vector3 lastValidLocation;

    void SwitchMarker (MarkerColour m)
    {
        if (currentGroundMarker == null)
        {
            currentGroundMarker = Instantiate(GroundMarker);
        }
            if (m == MarkerColour.Green)
            {
            ParticleSystem.MainModule te = currentGroundMarker.GetComponentInChildren<RFX4_EffectSettingColor>().GetComponent<ParticleSystem>().main;
            te.startColor = GroundMarkerGreen;
            ParticleSystem.MainModule t = currentGroundMarker.GetComponentInChildren<RFX4_Turbulence>().GetComponent<ParticleSystem>().main;
                t.startColor = GroundMarkerGreen;
                if (previousSelection.go.GetComponent<HumanController>() != null)
                    previousSelection.go.GetComponent<HumanController>().whirlwind.GetComponent<RFX4_EffectSettingColor>().Color = GroundMarkerGreen;
                else if (previousSelection.go.GetComponent<tombstoneController>() != null)
                    previousSelection.go.GetComponent<tombstoneController>().beam.GetComponent<RFX4_EffectSettingColor>().Color = GroundMarkerGreen;
            }
            else if (m == MarkerColour.Red)
            {
                ParticleSystem.MainModule te = currentGroundMarker.GetComponentInChildren<RFX4_EffectSettingColor>().GetComponent<ParticleSystem>().main;
                te.startColor = GroundMarkerRed;
                ParticleSystem.MainModule t = currentGroundMarker.GetComponentInChildren<RFX4_Turbulence>().GetComponent<ParticleSystem>().main;
                t.startColor = GroundMarkerRed;
                if (previousSelection.go.GetComponent<HumanController>() != null)
                    previousSelection.go.GetComponent<HumanController>().whirlwind.GetComponent<RFX4_EffectSettingColor>().Color = GroundMarkerRed;
                else if (previousSelection.go.GetComponent<tombstoneController>() != null)
                    previousSelection.go.GetComponent<tombstoneController>().beam.GetComponent<RFX4_EffectSettingColor>().Color = GroundMarkerRed;
            }
            else if (m == MarkerColour.Blue)
            {
            ParticleSystem.MainModule te = currentGroundMarker.GetComponentInChildren<RFX4_EffectSettingColor>().GetComponent<ParticleSystem>().main;
            te.startColor = GroundMarkerBlue;
            ParticleSystem.MainModule t = currentGroundMarker.GetComponentInChildren<RFX4_Turbulence>().GetComponent<ParticleSystem>().main;
                t.startColor = GroundMarkerBlue;
                if (previousSelection.go.GetComponent<HumanController>() != null)
                    previousSelection.go.GetComponent<HumanController>().whirlwind.GetComponent<RFX4_EffectSettingColor>().Color = GroundMarkerBlue;
                else if (previousSelection.go.GetComponent<tombstoneController>() != null)
                    previousSelection.go.GetComponent<tombstoneController>().beam.GetComponent<RFX4_EffectSettingColor>().Color = GroundMarkerBlue;
            }
        markerColour = m;
    }
}


