using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tombstoneController : MonoBehaviour {

    public Vector3 followPoint;
    public bool selected;
    public Human human;
    private bool onPlanet = false;
    public GameObject beam;

	// Use this for initialization
	void Start () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.transform.root.GetComponent<GameController>() != null || collision.collider.transform.root.GetComponent<BuildingController>() != null) && (collision.relativeVelocity.magnitude > 10))
        {
            onPlanet = true;
            EZCameraShake.CameraShaker.Instance.ShakeOnce(3, 3, 0.1f, 0.5f);
        }
    }

    // Update is called once per frame
    void Update () {
        if (selected)
        {
            Vector3 offset = (followPoint - GameController.instance.transform.position) * (1.1f);
            //transform.position = Vector3.SmoothDamp(transform.position, GameController.instance.transform.position + offset, ref vel, 0.5f); also look at slerp, but change time to 2nd order 
            transform.position = GameController.instance.transform.position + offset;
            foreach (BoxCollider b in GetComponents<BoxCollider>())
                b.isTrigger = true;
            onPlanet = false;
        }
        else
        {
            foreach (BoxCollider b in GetComponents<BoxCollider>())
                b.isTrigger = false;
            if (onPlanet)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
}
