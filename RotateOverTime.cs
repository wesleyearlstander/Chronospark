using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour {

    public static RotateOverTime instance;
    public GameObject sun, moon;

	public float rotateSpeed = 1;

    private void Awake()
    {
        
    }
    private void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update () {
        instance = this;
        if (Camera.main.transform.IsDay())
            transform.Rotate(transform.right, rotateSpeed / 2 * 360 / 60 * Time.deltaTime);
        else
            transform.Rotate(transform.right, rotateSpeed * 360 / 60 * Time.deltaTime);
    }
}
