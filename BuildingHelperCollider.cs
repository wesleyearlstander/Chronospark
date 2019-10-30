using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHelperCollider : MonoBehaviour {

    BuildingHelper bh;

	// Use this for initialization
	void Start () {
        bh = transform.root.GetComponent<BuildingHelper>();
        if (GetComponent<MeshRenderer>() != null)
        {
            bh.mrs.Add(GetComponent<MeshRenderer>());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GameController>() == null)
        {
            bh.gos.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GameController>() == null)
        {
            bh.gos.Remove(other.gameObject);
        }
    }
}
