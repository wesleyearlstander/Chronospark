using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AtmosphereZoomer : MonoBehaviour {

    public static AtmosphereZoomer az;

    public Transform moon;
    public Transform sun;

    public Color dayColour;
    public Color intermediaryColour;
    public Color nightColour;

    public bool tutorial = false;

    private SpriteRenderer sr;

    public float change;
    public float otherchange;



    void Awake ()
    {
        az = this;
    }

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        tutorial = true;

    }
	
	// Update is called once per frame
	void Update () {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (moon == null)
        {
            moon = RotateOverTime.instance.moon.transform;
        } else if (sun == null)
        {
            sun = RotateOverTime.instance.sun.transform;
        }
        if (sun != null && moon != null)
        {
            if (sceneName == "NewDankMenu")
            {

                float zoom = Camera.main.transform.position.z;
                Vector3 val = Camera.main.transform.position.normalized * -1 * (change);
                //    Vector3 val = new Vector3(otherchange, 0, change);
                this.transform.position = val;
                transform.LookAt(Vector3.zero);
                Vector3 sunDirection = sun.position.normalized;
                Vector3 moonDirection = moon.position.normalized;
                Vector3 cameraDirection = Vector3.back;
                if ((sunDirection - cameraDirection).magnitude <= (moonDirection - cameraDirection).magnitude) //closer to sun
                {
                    float distance = (sunDirection - cameraDirection).magnitude / 1.4f; //value from 0-1
                    sr.color = Color.Lerp(dayColour, intermediaryColour, distance);
                }
                else //closer to moon
                {
                    float distance = (moonDirection - cameraDirection).magnitude / 1.4f; //value from 0-1
                    sr.color = Color.Lerp(nightColour, intermediaryColour, distance);
                }
            }



            else
            {
                this.gameObject.transform.SetParent(Camera.main.transform.root);
                NewCameraScript temp = Camera.main.transform.root.GetComponent<NewCameraScript>();
                float zoom = -temp.holderDistance;
                this.transform.localPosition = new Vector3(0, 0, 300 - zoom);

                transform.localRotation = Camera.main.transform.parent.localRotation;
                Vector3 sunDirection = sun.position.normalized;
                Vector3 moonDirection = moon.position.normalized;
                Vector3 cameraDirection = temp.transform.position.normalized;
                if ((sunDirection - cameraDirection).magnitude <= (moonDirection - cameraDirection).magnitude) //closer to sun
                {
                    float distance = (sunDirection - cameraDirection).magnitude / 1.4f; //value from 0-1
                    sr.color = Color.Lerp(dayColour, intermediaryColour, distance);
                }
                else //closer to moon
                {
                    float distance = (moonDirection - cameraDirection).magnitude / 1.4f; //value from 0-1
                    sr.color = Color.Lerp(nightColour, intermediaryColour, distance);
                }
            }
        }

    }
}