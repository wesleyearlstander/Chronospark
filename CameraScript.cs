using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    private Transform camTransform;
    private Transform pivotTransform;
    private Vector3 localRotation;
    private float camDistance;

    [HideInInspector]
    public HumanController hc;

    [Header("Settings")]
    public float MouseSensitivity = 3f;
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 5f;
    public float ScrollDampening = 3f;
    public float maxZoom = 55f;
    public float minZoom = 100f;
    public float yBoundary = 50f;
    public float xBoundary = 50f;
    public float slowZoomValue = 0.3f;
    

 
   



    void Start()
    {
        this.camTransform = this.transform;
        this.pivotTransform = this.transform.parent;
        camDistance = maxZoom + 50f;
    }


    void LateUpdate()
    {


        //Rotation of the Camera based on Mouse Coordinates
        if (Input.GetAxis("Mouse X") != 0 && Input.GetMouseButton(1) || Input.GetAxis("Mouse Y") != 0 && Input.GetMouseButton(1))
        {
                     
                localRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                localRotation.y += Input.GetAxis("Mouse Y") * -MouseSensitivity;
            
           

            //Clamp the y Rotation to not flip the planet at the top/bottom              
            //     localRotation.y = Mathf.Clamp(localRotation.y, -90f, 90f);

        } else if (hc != null) { 

            if (Input.mousePosition.x > Screen.width - xBoundary && hc.selected)
            {
                localRotation.x +=  -(MouseSensitivity*0.2f);

            }

            else if (Input.mousePosition.x < 0 + xBoundary && hc.selected)
            {
                localRotation.x += (MouseSensitivity * 0.2f);

            }

            else if (Input.mousePosition.y > Screen.height - yBoundary && hc.selected )
            {
                localRotation.y += (MouseSensitivity * 0.2f);

            }

            else if (Input.mousePosition.y < 0 + yBoundary && hc.selected)
            {
                localRotation.y += -(MouseSensitivity * 0.2f);
            }

        }

        //Zooming Input from Mouse Scroll Wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
             float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

            //makes camera scroll slower the closer you get to the planet
            zoomAmount *= (camDistance * slowZoomValue);

            camDistance += zoomAmount * -1f;

            //clamp scroll amount
            camDistance = Mathf.Clamp(camDistance, maxZoom, minZoom);

            
            
        }
        

        //Camera Transformations
        Quaternion Quat = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        
        //creates dampening effect for rotation
       pivotTransform.rotation = Quaternion.Lerp(pivotTransform.rotation, Quat, Time.deltaTime * OrbitDampening);

        if (camTransform.localPosition.z != this.camDistance * -1f)
        {
            //creates dampening effect for scroll
            camTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(camTransform.localPosition.z, camDistance * -1f, Time.deltaTime * ScrollDampening));
        }
    }
}
