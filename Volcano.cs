using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volcano : MonoBehaviour {

    public static Volcano instance;

    public GameObject whirlWind;
    public GameObject lava;
    public GameObject lavaBuildUp;

    private float timer = 0;
    private float volcanoTimer = 0;

	// Use this for initialization
	void Start () {
        instance = this;
        PlayerPrefs.SetInt("FinishedGame", 2);
    }

    public void ResetVolcano ()
    {
        volcanoTimer = 0;
        lavaBuildUp.SetActive(false);
        lavaBuildUp.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        volcanoTimer += Time.deltaTime;
        if (volcanoTimer > 1200)
        {
            Fader.instance.SetEnding(Fader.Endings.volcanoTimer);
            Fader.instance.finished = true;
        }
        if (Input.GetMouseButtonUp(0) && !Fader.instance.finished) whirlWind.SetActive(false);
        if (whirlWind.activeInHierarchy)
        {
            timer += Time.deltaTime;
            if (timer > 10f)
            {
                Fader.instance.SetEnding(Fader.Endings.Volcano);
                Fader.instance.finished = true;
            }
        }
        else timer = 0;
	}

    public void ShrineBuilt ()
    {
        StartCoroutine(ReduceLava());
    }

    IEnumerator ReduceLava ()
    {
        float timer = 0;
        while (GameController.instance.panning)
        {
            yield return new WaitForEndOfFrame();
        }
        while (timer < 5)
        {
            lava.transform.localPosition -= Vector3.up * Time.deltaTime;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    private void OnMouseDown()
    {
        whirlWind.SetActive(true);
    }
}
