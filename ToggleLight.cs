using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource), typeof(Collider))]
public class ToggleLight : MonoBehaviour {

    public Material glow;
    public GameObject light;
    private bool on = true;

    private Color startColor;

	// Use this for initialization
	void Start () {
        if (glow != null)
        {
            startColor = glow.GetColor("_EmissionColor");
            foreach (Material m in GetComponent<Renderer>().materials)
                if (glow.GetColor("_EmissionColor") == m.GetColor("_EmissionColor"))
                {
                    glow = m;
                }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        if (!GameController.instance.GetComponent<MovieController>().isActiveAndEnabled)
        {
            on = !on;
            if (on == false) EventHandler.instance.AddAchievement(Achievement.enviro);
            light.SetActive(on);
            GetComponent<AudioSource>().PlayOneShot(GameController.instance.lightSwitch);
            Color temp = glow.GetColor("_EmissionColor");
            float emission = 1;
            if (on)
                temp = startColor;
            else emission = 0;
            temp *= Mathf.LinearToGammaSpace(emission);
            glow.SetColor("_EmissionColor", temp);
        }
    }
}
