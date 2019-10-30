using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingResourceText : MonoBehaviour {

    private Animator anim;
    public Text text;

	// Use this for initialization
	void Start () {

        anim = GetComponent<Animator>();
        AnimatorClipInfo[] clipinfo = anim.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipinfo[0].clip.length);
       
	}

    public void GetText (int amount)
    {
        text.text = amount.ToString();
    }
	
	// Update is called once per frame
	void Update () {
	
        
       
	}


}
