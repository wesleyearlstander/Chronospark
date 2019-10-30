using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DressingItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponentInParent<ResourceHolder> () == null)
        transform.SetParent(ResourceHolder.instance.transform);
    }
}
