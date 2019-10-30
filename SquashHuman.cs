using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashHuman : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    bool canSquash = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<HumanController>() != null && canSquash)
        {
            collision.collider.GetComponent<HumanController>().Explode();
            GetComponent<AudioSource>().PlayOneShot(GameController.instance.buildingSquash);
        }
        else if (collision.collider.transform.root.GetComponent<GameController>() != null && GameController.instance.gameManager.currentTime._time > new TimeManager.YearMonth(0, 1))
        {
            canSquash = false;
            transform.parent.GetComponent<BuildingController>().Respawn();
            GetComponentInParent<AudioSource>().PlayOneShot(GameController.instance.buildingDrop);
            foreach (Human h in GameController.instance.gameManager.humanManager.humans)
            {
                h._age._go.GetComponent<HumanController>().SetLastPositionForTombstone(true);
            }
        }
        else if (collision.collider.transform.root.GetComponent<GameController>() != null)
        {
            canSquash = false;
        }
    }
}
