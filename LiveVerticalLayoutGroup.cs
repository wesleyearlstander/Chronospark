using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(VerticalLayoutGroup))]
public class LiveVerticalLayoutGroup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<VerticalLayoutGroup>().enabled = false;
        GetComponent<VerticalLayoutGroup>().enabled = true;
    }
}
