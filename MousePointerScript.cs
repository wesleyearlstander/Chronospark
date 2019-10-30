using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointerScript : MonoBehaviour {

    public Texture2D hand;
    public CursorMode cur = CursorMode.Auto;
    public Vector3 hotspot = Vector3.zero;


	// Use this for initialization
	void Start () {

       
	}
	
	// Update is called once per frame
	void Update () {

        Cursor.SetCursor(hand, hotspot, cur);
    }
}
