using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoTabScript : MonoBehaviour {

    public GameObject infoTab;
    public GameObject cheatSheet;
    public GameObject ageKey;
    public GameObject controls;

    public GameObject quitpanel;


    void Start()
    {
        infoTab.SetActive(false);
        cheatSheet.SetActive(false);
        ageKey.SetActive(false);
        controls.SetActive(false);
        quitpanel.SetActive(false);
    }

    public void ShowInfo()
    {
        infoTab.SetActive(true);
        cheatSheet.SetActive(false);
        ageKey.SetActive(false);
        controls.SetActive(true);

    }

    public void HideInfo()
    {
        infoTab.SetActive(false);
    }

    public void ShowCheat()
    {
        cheatSheet.SetActive(true);
        ageKey.SetActive(false);
        controls.SetActive(false);
    }

    public void ShowControls()
    {
        cheatSheet.SetActive(false);
        ageKey.SetActive(false);
        controls.SetActive(true);
    }

    public void ShowAgeKey()
    {
        cheatSheet.SetActive(false);
        ageKey.SetActive(true);
        controls.SetActive(false);
    }

    public void OpenQuitPanel()
    {
        quitpanel.SetActive(true);
    }

   public void CloseGame()
    {
        SceneManager.LoadScene("NewDankMenu");
    }

    public void CloseQuitPanel()
    {
        quitpanel.SetActive(false);
    }


}
