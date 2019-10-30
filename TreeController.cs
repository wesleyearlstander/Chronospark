using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    public bool selected;
    public GameObject whirlwind;
    ResourceController rc;
    public AnimationCurve AgingCurve;
    private Color[] startColors;
    private Renderer[] renderers;

    private void Awake()
    {
        rc = GetComponent<ResourceController>();
        int count = 0;
        foreach (Renderer r in whirlwind.transform.parent.GetComponentsInChildren<Renderer>())
        {
            if (r.material.color.g > r.material.color.r)
            {
                count++;
            }
        }
        startColors = new Color[count];
        renderers = new Renderer[count];
        count = 0;
        foreach (Renderer r in whirlwind.transform.parent.GetComponentsInChildren<Renderer>())
        {
            if (r.material.color.g > r.material.color.r)
            {
                renderers[count] = r;
                startColors[count] = r.material.color;
                count++;
            }
        }
    }

    // Use this for initialization
    void Start () {
     
    }
	
	// Update is called once per frame
	void Update () {
        if (rc.resource._age._time.year < 1)
            rc.resource._age.AddMonth(Random.Range(1, 30) * 12);
        if (rc.resource._age._time.year < 40)
        {
            float size = Mathf.Lerp(0.5f, 1, AgingCurve.Evaluate(rc.resource._age._time / new TimeManager.YearMonth(40, 0)));
            whirlwind.transform.parent.transform.localScale = new Vector3(size, size, size);
            GetComponent<ResourceController>().GroundMarker.transform.GetChild(0).transform.localScale = new Vector3(size * 2.8f, size * 2.8f, size * 2.8f);
            GetComponent<ResourceController>().gatherTransform.GetChild(0).transform.localScale = new Vector3(size, size, size);
            size *= 2;
            whirlwind.transform.localScale = new Vector3(size, size, size);
            if (rc.resource.resourceMax == rc.resource.currentResources)
                rc.resource.currentResources = 100 + rc.resource._age._time.year * 3;
            rc.resource.resourceMax = 100 + rc.resource._age._time.year * 3;
        } else if (rc.resource._age._time.year < 500)
        {
            int i = 0;
            foreach (Renderer r in renderers)
            {
                r.material.color = Color.Lerp(startColors[i], new Color (0.407f,0.301f,0.067f,1), new TimeManager.YearMonth (rc.resource._age._time.year - 40, rc.resource._age._time.month) / new TimeManager.YearMonth(430, 0));
                i++;
            }
        } else if (rc.resource._age._time.year > 500)
        {
            Die();
        }
        if (selected && Input.GetMouseButtonUp(0))
        {
            Select(false);
        }
	}

    public void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Planet")
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void Select (bool s)
    {
        selected = s;
        foreach (Human h in rc.resource.humans)
        {
            h._age.SetBeingAged(selected);
        }
        rc.resource._age.SetBeingAged(selected);
        if (whirlwind != null)
            whirlwind.SetActive(selected);
        if (PlayerController.instance.previousSelection.selected == PlayerController.Selected.human && selected)
        {
            Select(false);
        }
    }

    public void Die ()
    {
        Instantiate(GameController.instance.stump, transform.position, Quaternion.identity);
        foreach (Collider m in GetComponentsInChildren<Collider>())
            m.isTrigger = true;
        GetComponent<Gravity>().Gravitate = true;
        Select(false);
        foreach (Human h in rc.resource.humans)
            h._age._go.GetComponent<HumanController>().LeaveResource();
        bool biggest = true;
        GetComponent<ResourceController>().DestroyAfterSeconds(4);
        foreach (Resource r in GameController.instance.gameManager.resourceManager.resources)
        {
            if (r.resourceMax > rc.resource.resourceMax)
            {
                biggest = false;
            }
        }
        if (biggest) EventHandler.instance.AddAchievement(Achievement.biggestTree);
        rc.resource.dead = true;
    }
}
