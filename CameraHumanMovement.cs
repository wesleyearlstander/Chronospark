using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHumanMovement : MonoBehaviour {


    [Header("Settings")]
    public float planetRadius = 100f;
    public float distance;

    
    public GameObject planet;
  
    public HumanController hc;
   
    public Transform camTransform;
    
    public Transform holderTransform;



    private Vector3? mouseStartPosition;
    private Vector3? currentMousePosition;

    public bool dog;

    private void LateUpdate()
    {
        if (hc != null)
        {
            if (hc.selected == true)
            {
                mouseStartPosition = GetMouseHit();
                  dog = true;
            }

            if (mouseStartPosition != null)
            {
                DragPlanet();
             //   dog = true;
            }

        }
        else
        {
            dog = false;
            StaticPlanet();
        }
    }


    private void DragPlanet()
    {
        currentMousePosition = GetMouseHit();
        RotateCamera((Vector3)mouseStartPosition, (Vector3)currentMousePosition);
    }


    private void StaticPlanet()
    {
        mouseStartPosition = null;
        currentMousePosition = null;
    }

    private void RotateCamera(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        //normalised for odd edges
        dragEndPosition = dragEndPosition.normalized * planetRadius;
        dragStartPosition = dragStartPosition.normalized * planetRadius;

        // Cross Product
        Vector3 cross = Vector3.Cross(dragEndPosition, dragStartPosition);

        // Angle for rotation
        float angle = Vector3.SignedAngle(dragEndPosition, dragStartPosition, cross);

        //Causes Rotation of angle around the vector from Cross product
        holderTransform.RotateAround(planet.transform.position, cross, angle);
    }


    private static Vector3? GetMouseHit()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return hit.point;
        }
        return null;
    }




}
