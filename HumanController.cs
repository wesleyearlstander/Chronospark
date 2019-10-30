using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanController : MonoBehaviour {

    public enum HumanState { idle, walking, beingHeld, working, inBuilding, dead, movie, descending }



    //hidden members
    public Human human = null;
    [HideInInspector]
    public HumanState humanState;
    private GameManager gm;
    private Rigidbody rb;
    [HideInInspector]
    public bool selected = false;
    [HideInInspector]
    public Vector3 followPoint;
    [HideInInspector]
    public Resource resourceBeingGathered = null;
    [HideInInspector]
    public Building buildingInside = null;

    public GameObject actionsCanvas;
    //public member and settings
    [Header("Settings")]
    [SerializeField]
    private Animator anim;
    [SerializeField]
    [Range(10f, 20f)]
    private float percentageOffPlanet = 10f;
    [SerializeField]
    private GameObject eyes;
    [SerializeField]
    private GameObject model;
    private GameObject hair;
    public Template template = 0;
    public enum Template { Ava, Nonna, Frase, Dena, heno, random }
    [HideInInspector]
    public ActionType movieAction;
    public Canvas SpeechBubbleCanvas;
    public GameObject whirlwind;
    public GameObject feetTrailPrefab;
    private GameObject feetTrail;

    public bool showUI = false;
    public bool anotherBool = false;
    public ShowUIOnHoverPlayer show;
    public GameObject circularCanvas;
    private int timerrr = 0;
    public bool dummy;
    public Camera cam;

    public GameObject[] maleHairs;
    public GameObject[] femaleHairs;
    public GameObject[] shoes;
    public GameObject[] Shirts;
    public GameObject[] shorts;
    public GameObject axe;
    public GameObject farmingTool;
    public GameObject diaper;

    [SerializeField]
    public GameObject button;
    private PopulationButtonListControl pbl;
    private PopulationButtonList popBut;


    public Vector3 startingPoint;


    void Awake() //initialization of local variables
    {
        rb = GetComponent<Rigidbody>();
    }

    private Vector3 lastPositionForTombstone;
    private bool setLastPositionForTombstone = true;

    public void SetLastPositionForTombstone(bool b)
    {
        setLastPositionForTombstone = b;
    }

    private float timer = 0;
    bool born = false;
    // Use this for initialization
    void Start() {
        if (human == null)
        {
            CreateHuman((int)template);
            humanState = HumanState.idle;
            SpeechBubbleCanvas.gameObject.SetActive(true);
            movieAction = ActionType.idle;
            human._speechDone = false;


        }
        if (human._age._time < new TimeManager.YearMonth(0, 1))
        {
            born = true;
        }
        startingPoint = transform.position;
        EventHandler.instance.AddHiddenEvent(EventHandler.EventType.childBorn);
    }

    public void OnClickButton()
    {
        if (human != null)
        {
            StartCoroutine(Camera.main.transform.root.GetComponent<CameraTutorialScript>().PanCamera(transform));
            StartCoroutine(ActivateCircle());
            PopulationPanelAnim.ppa.Toggle();
        }

    }

    IEnumerator ActivateCircle()
    {
        circularCanvas.SetActive(true);
        while (GameController.instance.panning)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2f);
        circularCanvas.SetActive(false);
    }

    private float idleTimer = 0, walkTimer = 0, turnTimer = 0;

    public bool drowning = false;

    IEnumerator Drown()
    {
        Vector3 drownPoint = transform.position;
        Vector3 direction = transform.position.normalized * 0.2f;
        drowning = true;
        float timer = 0f;
        while (timer < 4)
        {
            transform.position = drownPoint + Vector3.Lerp(-1 * direction, direction, Mathf.Sin(Mathf.PI * timer * 2));
            timer += Time.deltaTime;
            rb.velocity = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
        if (human._isVirgin)
        {
            EventHandler.instance.AddAchievement(Achievement.cliche);
            Volcano.instance.ResetVolcano();
        }
        Die(true);
        yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "LavaCollider" && !drowning)
        {
            Instantiate(GameController.instance.lavaExplosion, transform.position, Quaternion.identity);
            StartCoroutine(Drown());
        }
        else
        if (other.transform.root.GetComponent<GameController>() != null)
        {
            GetComponent<CapsuleCollider>().isTrigger = false;
        }
        else if (other.transform.GetComponent<ResourceController>() != null)
        {
            if (movieAction == ActionType.WalkTo && humanState == HumanState.movie)
            {
                StartCoroutine(WalkToAssignPoint(other.transform.GetComponent<ResourceController>()));
            }
        }
        else if (other.transform.GetComponent<ResourceController>() != null && humanState != HumanState.movie)
        {
            if (humanState == HumanState.working)
            {
                //rb.velocity = Vector3.zero;
            }
            else if (humanState == HumanState.walking)
            {
                if (other.transform.GetComponent<ResourceController>().resource.CanAddHuman(human))
                {
                    collisions++;
                    if (Random.Range(0, collisions) < collisions - 1)
                    {
                        other.transform.GetComponent<ResourceController>().AddHuman(human);
                        resourceBeingGathered = other.transform.GetComponent<ResourceController>().resource;
                        humanState = HumanState.working;
                        collisions = 0;
                    }
                    else
                    {
                        StartCoroutine(Turn(Random.Range(90, 270)));
                    }
                }
                else
                {
                    StartCoroutine(Turn(Random.Range(90, 270)));
                }
            }
        }
    }

    public float GetAngleBetween(Vector2 A, Vector2 B)
    {
        float DotProd = Vector2.Dot(A, B);
        float Length = A.magnitude * B.magnitude;
        return Mathf.Acos(DotProd / Length);
    }

    IEnumerator WalkToAssignPoint(ResourceController rc)
    {
        while (GetAngleBetween(transform.position, rc.gatherTransform.position) > 0.008f)
        {
            transform.LookAt(rc.gatherTransform);
            rb.velocity = transform.forward * 2;
            yield return new WaitForEndOfFrame();
        }
        if (rc.resource.CanAddHuman(human))
        {
            rc.AddHuman(human);
            resourceBeingGathered = rc.resource;
            rc.ActivateGroundMarker(false);
            movieAction = ActionType.idle;
        }

        if (rc.GetComponent<TreeController>() != null)
        {
            axe.SetActive(true);
        }
        else
            farmingTool.SetActive(true);

        yield return new WaitForEndOfFrame();
    }

    public void Explode()
    {
        Instantiate(GameController.instance.bloodSplatter, transform.position, Quaternion.identity);
        //EZCameraShake.CameraShaker.Instance.ShakeOnce(6, 6, 0.1f, 1);
        EventHandler.instance.AddAchievement(Achievement.toto);
        Die(false);
    }

    public void Die(bool sound)
    {
        if (sound) GetComponent<AudioSource>().PlayOneShot(GameController.instance.deathSound);
        if (buildingInside != null)
        {
            LeaveHouse();
        } else if (resourceBeingGathered != null)
        {
            LeaveResource();
        }
        GameController.instance.gameManager.humanManager.RemoveHuman(human);
        if (PlayerController.instance.previousSelection.go = gameObject)
        {
            PlayerController.instance.previousSelection.selected = null;
            Destroy(PlayerController.instance.currentGroundMarker);
        }
        Destroy(button);
        if (human._ageZone == Human.AgeZone.Elder && human._age.agingTimer > 1) EventHandler.instance.AddAchievement(Achievement.hospice);
        humanState = HumanState.dead;
    }

    int AnimationIndex()
    {
        if (drowning) return 7;
        switch (humanState)
        {
            case HumanState.idle: return Random.Range(0, 3);
            case HumanState.walking: return 3;
            case HumanState.working: return 4;
            case HumanState.inBuilding: return Random.Range(0, 3);
            case HumanState.dead: return Random.Range(0, 3);
            case HumanState.beingHeld: return Random.Range(0, 3);
            case HumanState.descending: return 5;
            case HumanState.movie:
                {
                    if (buildingInside != null)
                    {
                        return Random.Range(0, 3);
                    } else if (resourceBeingGathered != null)
                    {
                        return 4;
                    }
                    switch (movieAction)
                    {
                        case ActionType.idle: return Random.Range(0, 3);
                        case ActionType.WalkTo: return 3;
                        case ActionType.walkToLastCrop: return 3;
                        case ActionType.Wander: return Random.Range(0, 3);
                        case ActionType.leavehouse: return Random.Range(0, 3);
                        case ActionType.leaveResource: return Random.Range(0, 3);
                        case ActionType.releaseHuman: return Random.Range(0, 3);
                        case ActionType.Talk: return Random.Range(0, 3);
                    }
                    break;
                }

        }
        return 0;
    }

    float blinkTimer = 0;
    float blinkTime = 0;
    float riseTimer = 0;
    bool wasHeld = false;

    const float agingAnimationCorrectionFactor = 1.5f;
    private Color StartColour;
    // Update is called once per frame
    void Update()
    {
        if ((human._name == "Frase" && human._mother == null) || (human._name == "Ava" && human._father == null))
        {
            if (human._name == "Frase")
            {
                human._mother = GameController.instance.gameManager.humanManager.FindHumanByName("Nonna");
            } else
            {
                human._father = GameController.instance.gameManager.humanManager.FindHumanByName("Frase");
            }
        }
        if (setLastPositionForTombstone)
        {
            lastPositionForTombstone = transform.position;
        }

        if (transform.position.magnitude > 150 || transform.position.magnitude < 40)
        {
            transform.position = startingPoint;
        }
        if (born && human._ageZone > Human.AgeZone.Teen)
        {
            born = false;
            GameController.instance.raisedChildren++;
            if (GameController.instance.raisedChildren >= 3)
            {
                EventHandler.instance.AddAchievement(Achievement.kidsThree);
            }
        }

        blinkTimer += Time.deltaTime * Mathf.Clamp(human._age.agingMultiplier / Mathf.Pow(agingAnimationCorrectionFactor, human._age.agingTimer), 1, Mathf.Infinity);
        model.transform.parent.GetComponent<Animator>().speed = 1*Mathf.Clamp(human._age.agingMultiplier / Mathf.Pow(agingAnimationCorrectionFactor, human._age.agingTimer), 1, Mathf.Infinity);
        if (blinkTimer > Mathf.Clamp(blinkTime / human._age.agingMultiplier * Mathf.Pow(agingAnimationCorrectionFactor,human._age.agingTimer), 0.01f, 5))
        {
            blinkTime = Random.Range(0, 2) + 3;
            blinkTimer = 0;
            eyes.GetComponent<Animator>().SetTrigger("blink");
            eyes.GetComponent<Animator>().speed = Mathf.Clamp(human._age.agingTimer / 4, 1, Mathf.Infinity);
        }

        if (human != null)
        {
            if (human._ageZone < Human.AgeZone.Adult && human._ageZone > Human.AgeZone.Baby)
            {
                float size = Mathf.Lerp(0.8f, 1f, new TimeManager.YearMonth(human._age._time.year - Human.childYear, human._age._time.month) / human.childDuration);
                float sizeXZ = Mathf.Lerp(0.6f, 1f, new TimeManager.YearMonth(human._age._time.year - Human.childYear, human._age._time.month) / human.childDuration);
                model.transform.parent.transform.localScale = new Vector3(sizeXZ, size, sizeXZ);
                if (!human._humanGenetics._clothes.top.activeInHierarchy)
                {
                    human._humanGenetics._clothes.top.SetActive(true);
                    human._humanGenetics._clothes.bottom.SetActive(true);
                    human._humanGenetics._clothes.shoes.SetActive(true);
                    diaper.SetActive(false);
                }
            } else if (human._ageZone == Human.AgeZone.Baby)
            {
                float size = Mathf.Lerp(0.5f, 0.8f, human._age._time / human._age.timeEvents[(int)Human.AgeZone.Child].time);
                float sizeXZ = Mathf.Lerp(0.5f, 0.6f, human._age._time / human._age.timeEvents[(int)Human.AgeZone.Child].time);
                model.transform.parent.transform.localScale = new Vector3 (sizeXZ, size, sizeXZ);
                if (human._humanGenetics._clothes.top.activeInHierarchy)
                {
                    human._humanGenetics._clothes.top.SetActive(false);
                    human._humanGenetics._clothes.bottom.SetActive(false);
                    human._humanGenetics._clothes.shoes.SetActive(false);
                    diaper.SetActive(true);
                }
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (human._age._time.year >= 40 && hair.GetComponent<MeshRenderer>().material.color != GameController.instance.greyHair.color)
            {
                if (human._age._time.year == 40)
                    StartColour = hair.GetComponent<MeshRenderer>().material.color;
                hair.GetComponent<MeshRenderer>().material.color = Color.Lerp (StartColour, GameController.instance.greyHair.color, new TimeManager.YearMonth(human._age._time.year - Human.elderYear + 20, human._age._time.month) / new TimeManager.YearMonth(20,0));
            }
            if (selected && humanState == HumanState.beingHeld)
            {
                if (feetTrail != null)
                {
                    if (feetTrail.GetComponent<TrailRenderer>().emitting)
                        StartCoroutine(DestroyCurrentFootTrail());
                }
                wasHeld = true;
                riseTimer += Time.deltaTime;
                riseTimer = Mathf.Clamp01(riseTimer);
                anotherBool = true;
                float offsetPerc = Mathf.Lerp(1, (1 + percentageOffPlanet / 100), riseTimer);
                Vector3 offset = (followPoint - GameController.instance.transform.position) * offsetPerc;
                //transform.position = Vector3.SmoothDamp(transform.position, GameController.instance.transform.position + offset, ref vel, 0.5f); also look at slerp, but change time to 2nd order 
                transform.position = GameController.instance.transform.position + offset;
                GetComponent<CapsuleCollider>().isTrigger = true;
                whirlwind.SetActive(true);
                rb.velocity = Vector3.zero;
            }
            else
            {
                if (humanState == HumanState.working)
                {
                    if (human._ageZone == Human.AgeZone.Elder)
                    {
                        LeaveResource();
                        Select(false);
                    }
                }
                else if (humanState == HumanState.walking)
                {
                    float modifier = 1;
                    if (human._ageZone == Human.AgeZone.Adult)
                        modifier = 2;
                    else if (human._ageZone == Human.AgeZone.Elder)
                        modifier = 1.5f;
                    else if (human._ageZone < Human.AgeZone.Adult)
                        modifier = 2 * model.transform.parent.localScale.x;
                    rb.velocity = modifier * transform.forward;
                    model.transform.parent.GetComponent<Animator>().speed = modifier / 2 ;
                    if (feetTrail != null)
                    {
                        feetTrail.transform.localScale = new Vector3(modifier / 2 * 0.5f, modifier / 2 * 0.5f, modifier / 2 * 0.5f);
                        feetTrail.GetComponent<TrailRenderer>().minVertexDistance = modifier / 2;
                    } else
                    {
                        feetTrail = Instantiate(feetTrailPrefab, transform, false);
                        feetTrail.GetComponent<TrailRenderer>().emitting = true;
                    }
                    walkTimer += Time.deltaTime;
                    turnTimer += Time.deltaTime * 5;
                    if (walkTimer > 5)
                    {
                        walkTimer = 0;
                        StartCoroutine(Turn(Random.Range(0, 360)));
                        //transform.Rotate(Vector3.up * Random.Range(0, 360));
                    }
                    if (rb.velocity.magnitude < 0.9f && turnTimer > 0.1f)
                    {
                        turnTimer = 0;
                        StartCoroutine(Turn(180));
                    }
                }
                else if (humanState == HumanState.inBuilding)
                {
                    if (feetTrail != null)
                        if (feetTrail.GetComponent<TrailRenderer>().emitting)
                            StartCoroutine(DestroyCurrentFootTrail());
                    int count = buildingInside.HumanIndex(human);
                    transform.position = buildingInside._age._go.transform.up.normalized * 1000 + count * buildingInside._age._go.transform.up * 10;
                    transform.eulerAngles = Vector3.zero;
                    rb.velocity = Vector3.zero;
                    SetShadowsActive(true);
                    whirlwind.SetActive(false);
                }
                else if (humanState == HumanState.idle)
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer > 2)
                    {
                        idleTimer = 0;
                        humanState = HumanState.walking;
                    }
                }
                else if (humanState == HumanState.movie)
                {
                    if (buildingInside != null)
                    {
                        if(feetTrail != null)
                            if (feetTrail.GetComponent<TrailRenderer>().emitting)
                                StartCoroutine(DestroyCurrentFootTrail());
                        int count = buildingInside.HumanIndex(human);
                        transform.position = buildingInside._age._go.transform.up.normalized * 1000 + count * buildingInside._age._go.transform.up * 20;
                        transform.eulerAngles = Vector3.zero;
                        rb.velocity = Vector3.zero;
                        SetShadowsActive(false);
                    } else
                    {
                        SetShadowsActive(true);
                        if (resourceBeingGathered != null)
                        {
                            if (resourceBeingGathered._age._go.GetComponent<ResourceController>())
                            {
                                if (!axe.activeInHierarchy)
                                    axe.SetActive(false);
                            } else
                            {
                                if (!farmingTool.activeInHierarchy)
                                    farmingTool.SetActive(false);
                            }
                        }
                    }
                }
                else if (humanState == HumanState.dead)
                {
                    if (!drowning)
                    {
                        GameObject temp = Instantiate(GameController.instance.tombStone[Random.Range(0, GameController.instance.tombStone.Length)], lastPositionForTombstone, Quaternion.identity);
                        temp.GetComponent<tombstoneController>().human = human;
                    }
                    Destroy(this.gameObject);
                } 
            }

            if (!selected)

                anotherBool = false;
            if (!descending)
                anim.SetInteger("State", AnimationIndex());
            else anim.SetInteger("State", Random.Range(0, 3));
            ShowUI();
        }

    }

    void SetShadowsActive (bool b)
    { 
        UnityEngine.Rendering.ShadowCastingMode set;
        if (b)
        {
            set = UnityEngine.Rendering.ShadowCastingMode.On;
        } else {
            set = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        human._humanGenetics._clothes.top.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = set;
        human._humanGenetics._clothes.bottom.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = set;
        human._humanGenetics._clothes.shoes.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = set;
        human._humanGenetics._hair.GetComponent<MeshRenderer>().shadowCastingMode = set;
        model.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = set;
        eyes.GetComponent<MeshRenderer>().shadowCastingMode = set;
        diaper.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = set;
        human._humanGenetics._clothes.top.GetComponent<SkinnedMeshRenderer>().receiveShadows = b;
        human._humanGenetics._clothes.bottom.GetComponent<SkinnedMeshRenderer>().receiveShadows = b;
        human._humanGenetics._clothes.shoes.GetComponent<SkinnedMeshRenderer>().receiveShadows = b;
        human._humanGenetics._hair.GetComponent<MeshRenderer>().receiveShadows = b;
        model.GetComponent<SkinnedMeshRenderer>().receiveShadows = b;
        eyes.GetComponent<MeshRenderer>().receiveShadows = b;
        diaper.GetComponent<SkinnedMeshRenderer>().receiveShadows = b;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Planet")
        {
            if (AnimationIndex() == 1)
            {
                if (feetTrail != null)
                    feetTrail.GetComponent<TrailRenderer>().emitting = true;
            }
            else if (AnimationIndex() == 5)
            {
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Land" && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "fall_001")
                {
                    humanState = HumanState.idle;
                }
            }
            if (humanState == HumanState.working)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    IEnumerator DestroyCurrentFootTrail ()
    {
        GameObject temp = Instantiate (feetTrailPrefab, transform, false);
        GameObject fade = feetTrail;
        fade.transform.parent = null;
        feetTrail = temp;
        temp.GetComponent<TrailRenderer>().emitting = false;
        float timer = 0;
        while (timer < 10)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(fade);
        yield return new WaitForEndOfFrame();
    }
 
    IEnumerator Turn (int rotation)
    {
        float startRotation = transform.eulerAngles.y;
        float finishRotation = 0;
        if (rotation > 180)
        {
            if (startRotation > rotation)
            {
                finishRotation = startRotation - rotation;
                while (transform.eulerAngles.y > finishRotation && humanState == HumanState.walking)
                {
                    transform.Rotate(transform.up * -10);
                    yield return new WaitForEndOfFrame();
                }
            } else
            {
                finishRotation = startRotation - rotation;
                while (transform.eulerAngles.y < 360 + finishRotation && humanState == HumanState.walking)
                {
                    transform.Rotate(transform.up * 10);
                    yield return new WaitForEndOfFrame();
                }
            }
        } else
        {
            
            if (startRotation > 180)
            {
                finishRotation = startRotation - rotation;
                while (transform.eulerAngles.y > finishRotation && humanState == HumanState.walking)
                {
                    transform.Rotate(transform.up * -5);
                    yield return new WaitForEndOfFrame();
                }
            } else
            {
                finishRotation = startRotation + rotation;
                while (transform.eulerAngles.y < finishRotation && humanState == HumanState.walking)
                {
                    transform.Rotate(transform.up * 5);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }

    private float turnAroundTimer = 0;

    bool descending = false;
    Resource descendResource = null;
    public IEnumerator DescendToResource (Resource resource)
    {
        float descendTimer = 0;
        descending = true;
        GetComponent<CapsuleCollider>().isTrigger = true;
        descendResource = resource;
        humanState = HumanState.working;
        GameObject t = Instantiate(resource._age._go.GetComponent<ResourceController>().gatherTransform.gameObject, followPoint, Quaternion.identity);
        t.transform.parent = resource._age._go.transform;
        float distance = resource._age._go.GetComponent<ResourceController>().gatherTransform.position.magnitude;
        float otherDistance = t.transform.position.magnitude;
        float ratio = otherDistance / distance;
        t.transform.Translate(transform.position.normalized * ratio * -1);
        Destroy(resource._age._go.GetComponent<ResourceController>().gatherTransform.gameObject);
        resource._age._go.GetComponent<ResourceController>().gatherTransform = t.transform;
        resource._age._go.GetComponent<ResourceController>().gatherTransform.localEulerAngles = Vector3.zero;
        resource._age._go.GetComponent<ResourceController>().whirlwind = t.transform.GetChild(0).gameObject;
        //if (resource._age._go.GetComponent<TreeController>())
           // resource._age._go.GetComponent<TreeController>().whirlwind = resource._age._go.GetComponent<ResourceController>().whirlwind;
        Select(false);
        Vector3 offset = followPoint - GameController.instance.transform.position;
        Vector3 offsetHuman = transform.position - GameController.instance.transform.position;
        float startPerc = offsetHuman.magnitude / offset.magnitude;
        while (descendTimer < 1 && descending)
        {
            GetComponent<Gravity>().Gravitate = false;
            descendTimer += Time.deltaTime;
            descendTimer = Mathf.Clamp01(descendTimer);
            float offsetPerc = Mathf.Lerp(startPerc, 1, descendTimer);
            //Vector3 offset = (dropPoint - GameController.instance.transform.position);
            //transform.position = Vector3.SmoothDamp(transform.position, GameController.instance.transform.position + offset, ref vel, 0.5f); also look at slerp, but change time to 2nd order 
            transform.position = GameController.instance.transform.position + offset * offsetPerc;
            yield return new WaitForEndOfFrame();
        }
        anim.speed = 1;
        descending = false;
        GetComponent<Gravity>().Gravitate = true;
        humanState = HumanState.working;
        resourceBeingGathered = resource;
        //resource._age._go.GetComponent<ResourceController>().gatherTransform.RotateAroundLocal(resource._age._go.transform.up, )
        resource._age._go.GetComponent<ResourceController>().AddHuman(human);
        descendResource = null;
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator DescendToBuilding (Building building)
    {
        yield return new WaitForEndOfFrame();
    }

    IEnumerator Wander (MovieActionObject m)
    {
        Transform wander = m.walkTo;
        Vector3 position = wander.position;
        movieAction = ActionType.Wander;
        while (movieAction == ActionType.Wander && humanState == HumanState.movie)
        {
            //rb.velocity = transform.forward * 2;
            //turnAroundTimer += Time.deltaTime;
            //if ((transform.position - position).magnitude > 5 && turnAroundTimer > 1)
            //{
                //turnAroundTimer = 0;
                ///.LookAt(wander);
            //}
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator WalkTo (MovieActionObject m)
    {
        Transform walkTo;
        if (m.action == ActionType.walkToLastHouse)
        {
            walkTo = GameController.instance.lastHouse.transform;
        } else if (m.action == ActionType.walkToLastCrop)
        {
            walkTo = GameController.instance.lastCrops.transform;
        }
        else
        {
            walkTo = m.walkTo;
        }
        Vector3 position = walkTo.position;
        movieAction = ActionType.WalkTo;
        float follow = 3f;
        if (walkTo.transform.root.GetComponent<ResourceController>() != null || walkTo.transform.root.GetComponent<BuildingController>() != null) follow = 0f;
        while ((transform.position - position).magnitude > follow && movieAction == ActionType.WalkTo)
        {
            transform.LookAt(walkTo);
            rb.velocity = transform.forward * 2;
            yield return new WaitForEndOfFrame();
        }
        if (buildingInside == null && resourceBeingGathered == null)
            movieAction = ActionType.idle;
        yield return new WaitForEndOfFrame();
    }

    IEnumerator TalkTo (MovieActionObject m)
    {
        ConversationObject c = m.conversation;
        Human leftHuman = GameController.instance.gameManager.humanManager.FindHumanByName(c.CharacterLeftName);
        Human rightHuman = GameController.instance.gameManager.humanManager.FindHumanByName(c.CharacterRightName);
        
        movieAction = ActionType.Talk;
        int length = 0;
        bool left = false;
        if (leftHuman == human)
        {
            left = true;
        }
        length += c.Conversation_1_Left.Length;
        length += c.Conversation_2_Right.Length;
        int phrase = 0;
        int sentence = 0;
        if (left)
        {
            CreateSpeechBubble(c.Conversation_1_Left[sentence]);
            leftHuman._speechDone = false;
            sentence++;
        }
        while (phrase < length)
        {
            if ((phrase == 0 || phrase%2 == 0))
            {
                if (leftHuman._speechDone)
                {
                    phrase++;
                    if (rightHuman == human && phrase < length)
                    {
                        human._speechDone = false;
                        movieAction = ActionType.WaitForEvent;
                    }
                }
            } else if ((phrase % 2 != 0))
            {
                if (rightHuman._speechDone)
                {
                    phrase++;
                    if (leftHuman == human && phrase < length)
                    {
                        human._speechDone = false;
                        movieAction = ActionType.WaitForEvent;
                    }
                }
            }
            if (movieAction == ActionType.WaitForEvent)
            {
                if (left)
                {
                    CreateSpeechBubble(c.Conversation_1_Left[sentence]);
                }
                else
                {
                    CreateSpeechBubble(c.Conversation_2_Right[sentence]);
                }
                movieAction = ActionType.Talk;
                sentence++;
            }
            yield return new WaitForEndOfFrame();
        }
        //SpeechBubbleCanvas.gameObject.SetActive(false);
        movieAction = ActionType.idle;
        human._speechDone = false;
        yield return new WaitForEndOfFrame();
    }

    void CreateSpeechBubble (string text)
    {
        GameObject temp;
        if (buildingInside == null)
            temp = Instantiate(GameController.instance.speechBubble, SpeechBubbleCanvas.transform);
        else
        {
            temp = Instantiate(GameController.instance.speechBubble, buildingInside._age._go.GetComponent<BuildingController>().speechBubbleParents[buildingInside.HumanIndex(human)].transform);
            temp.gameObject.transform.localScale = 0.08f * temp.gameObject.transform.localScale;
            if (buildingInside._age._go.GetComponent<BuildingsUIScript>().humans[0] == human)
            {
                temp.gameObject.transform.localScale = new Vector3(temp.gameObject.transform.localScale.x * -1, temp.gameObject.transform.localScale.y, temp.gameObject.transform.localScale.z);
                temp.GetComponentInChildren<Text>().transform.localScale = new Vector3(temp.GetComponentInChildren<Text>().transform.localScale.x * -1, temp.GetComponentInChildren<Text>().transform.localScale.y, temp.GetComponentInChildren<Text>().transform.localScale.z);
            }
            temp.gameObject.transform.root.GetComponentInChildren<HoverHouse>().ShowPersistent = true;
        }
        
        temp.GetComponentInChildren<SpeechBubble>().human = human;
        temp.GetComponentInChildren<SpeechBubble>().test = text;

    }

    private int collisions = 0;

    void OnCollisionEnter(Collision collision)
    {
        whirlwind.SetActive(false);
        if (collision.collider.transform.name == "PlanetVoidCollider")
        {
            if (humanState == HumanState.walking)
            {
                StartCoroutine(Turn(Random.Range(90, 270)));
            }
        } else if (collision.collider.transform.root.tag == "Planet")
        {
            if (humanState == HumanState.descending)
            {
                rb.velocity = Vector3.zero;
                EZCameraShake.CameraShaker.Instance.ShakeOnce(model.transform.parent.localScale.x * 2 * riseTimer, 1, 0.1f, 0.3f);
                if (human._ageZone == Human.AgeZone.Baby) EventHandler.instance.AddAchievement(Achievement.babyCano);
                GetComponent<AudioSource>().PlayOneShot(GameController.instance.personLand);
            }
        }
        else //collides with object
        {
            if (collision.collider.transform.parent != null)
                if (collision.collider.transform.parent.name == "PlanetVoidCollider")
                {
                    if (humanState == HumanState.walking)
                    {
                        StartCoroutine(Turn(Random.Range(90, 270)));
                    }
                }
            }
            if (humanState == HumanState.walking)
            {
                if (collision.collider.transform.root.GetComponent<BuildingController>() != null)
                {
                    if (collision.collider.transform.root.GetComponent<BuildingController>().building.CanAddHuman())
                    {
                        collisions++;
                        if (Random.Range(0, collisions) < collisions - 1)
                        {
                            collision.collider.transform.root.GetComponent<BuildingController>().AddHuman(human);
                            buildingInside = collision.collider.transform.root.GetComponent<BuildingController>().building;
                            humanState = HumanState.inBuilding;
                            collisions = 0;
                        }
                        else
                        {
                        StartCoroutine(Turn(Random.Range(90, 270)));
                        }
                    }
                    else
                    {
                    StartCoroutine(Turn(Random.Range(90, 270)));
                    }
                }
                else if (turnAroundTimer > 2)
                {
                StartCoroutine(Turn(Random.Range(90, 270)));
                turnAroundTimer = 0;
                }
            }
            else if (movieAction == ActionType.Wander && humanState == HumanState.movie && turnAroundTimer > 1)
            {
            StartCoroutine(Turn(Random.Range(90, 270)));
            turnAroundTimer = 0;
            }
            else if (movieAction == ActionType.WalkTo && humanState == HumanState.movie)
            {
                if (collision.collider.transform.root.GetComponent<BuildingController>() != null)
                {
                    if (collision.collider.transform.root.GetComponent<BuildingController>().building.CanAddHuman())
                    {
                        collision.collider.transform.root.GetComponent<BuildingController>().AddHuman(human);
                        buildingInside = collision.collider.transform.root.GetComponent<BuildingController>().building;
                        movieAction = ActionType.idle;
                    }
                }
            }
        if (collision.collider.transform.parent != null)
        {
            if (collision.collider.transform.parent.name != "PlanetVoidCollider")
                riseTimer = 0;
        }
        else
        {
            riseTimer = 0;
        }
        wasHeld = false;
    }

    void CreateHuman (int t)
    {
        gm = GameController.instance.gameManager;
        if (t == 5)
            human = gm.humanManager.AddHuman(gameObject);
        else
            human = gm.humanManager.AddHuman(gameObject, t);
        gm.humanManager.RepopulateArray();
        if (eyes != null)
            eyes.GetComponent<MeshRenderer>().material = human._humanGenetics.GetEyeColour();
        if (model != null)
            model.GetComponent<SkinnedMeshRenderer>().material = human._humanGenetics.GetSkinColour();
        hair = human._humanGenetics._hair;
        if (hair != null)
            hair.GetComponent<MeshRenderer>().material = human._humanGenetics.GetHairColour();
        button = Instantiate(GameController.instance.button) as GameObject;
        popBut = button.GetComponent<PopulationButtonList>();
        button.SetActive(true);
        button.transform.SetParent(OrderChildren.oc.transform, false);
        popBut.human = human;
        feetTrail = Instantiate(feetTrailPrefab, transform, false);
    }

    public void CreateHuman (Human father, Human mother)
    {
        gm = GameController.instance.gameManager;
        human = gm.humanManager.AddHuman(this.gameObject, father, mother);
        gm.humanManager.RepopulateArray();
        if (eyes != null)
            eyes.GetComponent<MeshRenderer>().material = Instantiate(human._humanGenetics.GetEyeColour());
        if (model != null)
            model.GetComponent<SkinnedMeshRenderer>().material = Instantiate(human._humanGenetics.GetSkinColour());
        hair = human._humanGenetics._hair;
        if (hair != null)
            hair.GetComponent<MeshRenderer>().material = Instantiate(human._humanGenetics.GetHairColour());
        humanState = HumanState.idle;
        SpeechBubbleCanvas.gameObject.SetActive(true);
        button = Instantiate(GameController.instance.button) as GameObject;
        popBut = button.GetComponent<PopulationButtonList>();
        button.SetActive(true);
        button.transform.SetParent(OrderChildren.oc.transform, false);
        popBut.human = human;
        feetTrail = Instantiate(feetTrailPrefab, transform, false);
        EventHandler.instance.AddHiddenEvent(EventHandler.EventType.childBorn);
    }

    public void Select(bool b)
    {
        selected = b;
        if (selected)
        {
            if (humanState != HumanState.movie && humanState != HumanState.working)
                humanState = HumanState.beingHeld;
        }
        else
        {
            if (humanState == HumanState.beingHeld) {
                humanState = HumanState.descending;
            }
        }
        human._age.SetBeingAged(selected);//
    }

    public IEnumerator WaitAndLeave ()
    {
        float timer = 3;
        while (timer > 0)
        {
            timer--;
            yield return new WaitForEndOfFrame();
        }
        LeaveHouse();
        yield return new WaitForEndOfFrame();

    }

    public void LeaveHouse ()
    {
        buildingInside._age._go.GetComponent<BuildingController>().RemoveHuman(human);
    }

    public void LeaveResource ()
    {
        resourceBeingGathered.RemoveHuman(human);
        if (resourceBeingGathered._age._go.GetComponent<ResourceController>().whirlwind.activeInHierarchy)
        {
            resourceBeingGathered._age._go.GetComponent<ResourceController>().whirlwind.SetActive(false);
        }
        resourceBeingGathered = null;
        human._performingTask = false;
        if (axe.activeInHierarchy)
            axe.SetActive(false);
        if (farmingTool.activeInHierarchy)
            farmingTool.SetActive(false);
        human._age._taskTime = null;
        if (humanState != HumanState.movie && PlayerController.instance.previousSelection.go == gameObject)
            humanState = HumanState.beingHeld;
        else if (humanState != HumanState.movie)
        {
            humanState = HumanState.idle;
            Select(false);
        }
    }


    public void OnMouseOver()
    {
        showUI = true;
    }


    public void OnMouseExit()
    {
        showUI = false;
    }


    void ShowUI()
    {
        if ((showUI == true || show.active == true || anotherBool == true) && GameController.instance.canPlayerInfo)
        {
            circularCanvas.SetActive(true);
            if (resourceBeingGathered != null)
            {
                resourceBeingGathered._age._go.GetComponent<ResourceController>().ActivateGroundMarker(true);
            }
            dummy = true;
        }

        else if ((showUI == false && show.active == false && anotherBool == false && timerrr > 40) || !GameController.instance.canPlayerInfo)
        {
            circularCanvas.SetActive(false);
            timerrr = 0;
            dummy = false;
            showUI = false;
            show.active = false;
            anotherBool = false;
            if (resourceBeingGathered != null)
            {
                resourceBeingGathered._age._go.GetComponent<ResourceController>().ActivateGroundMarker(false);
            }
        }

        if (dummy)
            timerrr++;
    }


    public void SetPrevSel()
    {
        PlayerController.instance.SetPreviousSelection(gameObject);
    }




}

