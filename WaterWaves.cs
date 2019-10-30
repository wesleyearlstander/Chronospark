using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaterWaves : MonoBehaviour {

    public int Speed = 1;
	public int Scale = 1;

	// Use this for initialization
	void Start () {
		
	}

    int x = 0;
    int y = 0;
    float timer = 0;
	// Update is called once per frame
	void Update () {
        x++;
        if (x%2==0)
            y++;
        if (x>1000)
        {
            x = 0;
            y = 0;
        }
        timer += Time.deltaTime;
        Vector2 direction = new Vector2((Mathf.PerlinNoise(x/1000,y / 1000) -0.5f)*2,(Mathf.PerlinNoise(x / 1000, y / 1000) - 0.5f) * 2);
        GetComponent<Renderer>().material.mainTextureOffset += direction.normalized * Time.deltaTime * 0.001f * Speed;
		Vector2 scale = new Vector2(Mathf.Sin(timer/2) * 0.01f * Scale + 0.995f * Scale, Mathf.Sin(timer/2) * -0.01f * Scale + 1.005f * Scale);
        GetComponent<Renderer>().material.mainTextureScale = scale;
    }
}
