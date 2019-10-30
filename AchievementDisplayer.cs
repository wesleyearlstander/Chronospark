using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplayer : MonoBehaviour {

    public static AchievementDisplayer instance;
    public GameObject dialogueBox;
    public Text text;
    public float waitTime = 4;
    float timer = 0;
    public Queue<Achievement> q;

	// Use this for initialization
	void Start () {
        instance = this;
        q = new Queue<Achievement>();
	}

    AchievementCard temp;

	// Update is called once per frame
	void Update () {
        if (q.Count > 0)
        {
            if (timer == 0)
            {
                dialogueBox.GetComponent<Animator>().SetBool("Show", true);
                dialogueBox.GetComponent<AudioSource>().Play();
                foreach (AchievementCard c in GameController.instance.achievements)
                {
                    if (c.achievement == q.Peek())
                    {
                        temp = c;
                    }
                }
                text.text = temp.Name;
            }
            timer += Time.deltaTime;
            if (timer > waitTime * 2 + 1.5f)
            {
                text.text = "";
                timer = 0;
                q.Dequeue();
            } else if (timer > waitTime * 2)
            {
                dialogueBox.GetComponent<Animator>().SetBool("Show", false);
            } else if (timer > waitTime)
            {
                text.text = temp.description;
            }
        }
    }

    public void displayAchievement(Achievement a)
    {
        q.Enqueue(a);
    }
}
