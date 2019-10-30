using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCanvasScript : MonoBehaviour {
    // Use this for initialization

   // public GameObject cameraHokder2;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Camera.main.transform.rotation;
/*
        Vector3 pos = new Vector3(0, 0, 10f);
        Vector3 UIposs = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 campos = new Vector3(cameraHokder2.transform.position.x, cameraHokder2.transform.position.y, cameraHokder2.transform.position.z);

        transform.position = new Vector3(UIposs.x, UIposs.y, UIposs.z - campos.z - 10f); */
	}
}
