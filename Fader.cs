using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour {

    public static Fader instance;
    public GUIStyle style;
    public Texture2D fadeTexture;
    public float fadeSpeed = 0.2f;
    public int drawDepth = -1000;
    private float alpha = 0f;
    private int fadeDir = 1;
    private float textAlpha = 0f;
    public bool finished = false;

    public string ending = "";

    public enum Endings { Volcano, Nonna, builtShrine, Dead, volcanoTimer }

    public void Awake()
    {
        instance = this;
    }

    public void SetEnding (Endings e)
    {
        switch (e)
        {
            case Endings.Volcano: ending = "And they caught the creator on a bad day."; break;
            case Endings.Nonna: ending = "If we take too long, those we love will fade away."; break;
            case Endings.builtShrine: ending = "Was it all in vain or part of a larger plan?"; break;
            case Endings.Dead: ending = "For it is up to the creator to give and take life as they please..."; break;
            case Endings.volcanoTimer: ending = "The volcano erupted... for those who stay deaf to the Creator's cries will force their hand."; break;
        }
    }

    float timer = 0;

    void OnGUI()
    {
        if (finished && ending != "")
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            GameController.instance.GetComponent<AudioSource>().volume -= fadeDir * fadeSpeed * Time.deltaTime / 2;
            Color temp = Color.black;
            temp.a = alpha;
            GUI.color = temp;
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
            if (alpha == 1)
            {
                GameController.instance.GetComponent<AudioSource>().Stop();
                textAlpha += fadeDir * fadeSpeed * Time.deltaTime;
                textAlpha = Mathf.Clamp01(textAlpha);
                Color temx = Color.white;
                temx.a = textAlpha;
                GUI.color = temx; 
                GUI.depth = drawDepth - 1;
                GUI.Label(new Rect(Screen.width / 2 - Screen.width / 3, Screen.height / 2 - Screen.height / 10, Screen.width / 3 * 2, Screen.height / 5), ending, style);
                if (textAlpha == 1)
                {
                    timer += Time.deltaTime;
                }
            }

        }
        if (timer > 5)
        {
            alpha = 2;
            fadeDir = -1;
            if (textAlpha == 0)
            {
                PlayerPrefs.SetInt("FinishedGame", 1);
                SceneManager.LoadScene(0);
            }
        }
    }
}
