using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public Animator playButtonAnim;
    public Animator quitButtonAnim;
    public Animator popOutTutorialQuestionAnim;

    public Animator cameraAnim;
    public Animator moveWorldSpaceUIAnim;

    public GameObject popOutTutorialPanel;
    bool show1;
    bool play;
    bool quit;
    bool withTut = false;
    

    


	// Use this for initialization
	void Start () {

        popOutTutorialPanel.SetActive(false);

        show1 = popOutTutorialQuestionAnim.GetBool("Show");

    }
	
	// Update is called once per frame
	void Update () {

        Vector3 camPos = new Vector3(0, 0, -150);
        if (Camera.main.transform.position == camPos && withTut == true)
            SceneManager.LoadScene("SampleScene");
        else if (Camera.main.transform.position == camPos && withTut == false)
            SceneManager.LoadScene("SampleScene2");

    }


    public void TutPanelAnim()
    {
        popOutTutorialPanel.SetActive(true);
        show1 = !show1;
        popOutTutorialQuestionAnim.SetBool("Show", show1);
    }

    public void PlayWithTutorial()
    {
        popOutTutorialPanel.SetActive(false);
        cameraAnim.SetBool("Play", true);
        moveWorldSpaceUIAnim.SetBool("move", true);
       withTut = true;
    }

    public void PlayWithOutTutorial()
    {
        popOutTutorialPanel.SetActive(false);
        cameraAnim.SetBool("Play", true);
        moveWorldSpaceUIAnim.SetBool("move", true);
        withTut = false;
    }

    public void TogglePlayButtonAnim()
    {
        play = playButtonAnim.GetBool("DO");
       playButtonAnim.SetBool("DO", !play);
    }

    public void ToggleQuitButtonAnim()
    {
        quit = quitButtonAnim.GetBool("DO");
        quitButtonAnim.SetBool("DO", !quit);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
