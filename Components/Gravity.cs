using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Gravity : MonoBehaviour {
    public float GravityScale = 1;
    public bool Gravitate = true;

    private float deltaTime;

    private void Start()
    {
        deltaTime = Time.deltaTime;
    }

    private void Update()
    {
        Vector3 gravityUp = (Vector3.zero - transform.position).normalized; //up vector from centre of earth
        Vector3 bodyUp = transform.up;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp * -1) * transform.rotation; //corrects up vector for location on planet
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * deltaTime);
        if (Gravitate && this.GetComponent<Rigidbody> () != null)
        {
            this.GetComponent<Rigidbody>().AddForce(gravityUp * GravityScale * 10f * GetComponent<Rigidbody>().mass); //applying gravity force
            this.GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
