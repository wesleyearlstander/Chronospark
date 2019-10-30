using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DialogueBox : MonoBehaviour {


    public Text text;
    string s = "fill the text in here yo";

    public bool showAnim;
    public Animator anim;
    bool Show;


    // Use this for initialization
    void Start () {

        Show = anim.GetBool("Show");

    }
	
	// Update is called once per frame
	void Update () {

        text.text = s;


        
    }

    public void Toggle()
    {
        Show = !Show;
     anim.SetBool("Show", Show);

    }
}
