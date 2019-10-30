using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtEarth : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Vector3.zero);
	}
}
