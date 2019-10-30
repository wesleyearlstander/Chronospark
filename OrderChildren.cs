using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderChildren : MonoBehaviour {

    public static OrderChildren oc;
    private void Awake()
    {
        oc = this;
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        foreach (PopulationButtonList p in GetComponentsInChildren<PopulationButtonList>())
        {
            for (int i = 0; i < GameController.instance.gameManager.humanManager.sortedArray.Length; i++)
            {
                if (p.human == GameController.instance.gameManager.humanManager.sortedArray[i])
                {
                    p.transform.SetSiblingIndex(i);
                }
            }
        }
    }
}
