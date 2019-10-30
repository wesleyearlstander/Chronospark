using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievmentPanel : MonoBehaviour {

    public static AchievmentPanel ap;

	public Text currentAmount;
	public Text totalAmount;

    private void Awake()
    {
        ap = this;
    }

    // Use this for initialization
    void Start () {

		totalAmount.text = GameController.instance.achievements.Length.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        currentAmount.text = EventHandler.instance.currentAchievements.Count.ToString();
	}
}
