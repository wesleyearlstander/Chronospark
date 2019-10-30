using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialOnTime : MonoBehaviour {

    public Material dayMaterial;
    public Material nightMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.IsDay()) GetComponent<MeshRenderer>().material = dayMaterial;
        else GetComponent<MeshRenderer>().material = nightMaterial;
    }
}
