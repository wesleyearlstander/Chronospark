using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour
{

    public Texture2D fadeTexture;
    public float fadeSpeed = 0.2f;
    public int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;

    bool done = false;
    void OnGUI()
    {
        if (!done)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            Color temp = GUI.color;
            if (PlayerPrefs.GetInt("FinishedGame") == 1) temp = Color.black; 
            if (PlayerPrefs.GetInt("FinishedGame") == 2) alpha = 0;
            temp.a = alpha;
            GUI.color = temp;
            GUI.depth = drawDepth;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
            if (alpha == 0)
            {
                done = true;
                PlayerPrefs.SetInt("FinishedGame", 0);
            }
        }
    }
}