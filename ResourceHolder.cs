using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResourceHolder : MonoBehaviour {

    public static ResourceHolder instance;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }
}
