using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraScript : MonoBehaviour {

    private Vector3? mouseStartPosition;
    private Vector3? currentMousePosition;
    [HideInInspector]
    public float holderDistance;

    [Header("Settings")]
    public float planetRadius = 100f;
    public float distance;
    public float zoomSensitivity;
    public float maxZoom;
    public float minZoom;
    public float slowZoomValue;
    public float scrollDampening;
    public float zoomInAngleValue;
    public float zoomInAngleDampening;

   [SerializeField]
    public GameObject planet;
    [HideInInspector]
    public HumanController hc;
    [HideInInspector]
    public Transform camTransform;
    [HideInInspector]
    public Transform holderTransform;

    [SerializeField]
    public Transform cameraHolder2;
    [SerializeField]
    public Transform cameraHolder3;

    public CameraTutorialScript cts;
    public bool clamping;
    private ClampPoint clampPoint;

    public float _size;


    void Awake()
    {
        // Cameras initial position
        holderTransform = transform;
        holderTransform.position = new Vector3(planet.transform.position.x, planet.transform.position.y, planet.transform.position.z - distance);
        holderTransform.LookAt(planet.transform.position);       
        holderDistance = maxZoom + 70f;     

    }

    public void PressMe()
    {
        Vector3 rot = new Vector3(0, 0, 0);
        SetClamp(rot, 40);

    }


    private void LateUpdate()
    {
        if (planet == null)
        {
            planet = GameController.instance.gameObject;
            holderTransform = transform;
            holderTransform.position = new Vector3(planet.transform.position.x, planet.transform.position.y, planet.transform.position.z - distance);
            holderTransform.LookAt(planet.transform.position);
            holderDistance = maxZoom + 70f;
        }

        if (Input.GetMouseButtonDown(1))
            mouseStartPosition = GetMouseHit();
        

        if (GameController.instance.canDragEarth && !GameController.instance.panning)
            DragPlanet();


        if (Input.GetMouseButtonUp(1))
            StaticPlanet();
           
        

        if (Input.GetAxis("Mouse ScrollWheel") != 0f && GameController.instance.canZoom && !GameController.instance.zooming && !GameController.instance.canZoomPop)
            Zoom();

        if (cameraHolder2.localPosition.z != holderDistance)
            cameraHolder2.localPosition = new Vector3(0f, cameraHolder2.localPosition.y, Mathf.Lerp(cameraHolder2.localPosition.z, holderDistance * -1f, Time.deltaTime * scrollDampening));

    }



    struct ClampPoint
    {
        public Vector3 basePoint;
        public float size;

        public ClampPoint (Vector3 b, float s)
        {
            basePoint = b;
            size = s;
        }


        public Vector3 GetMinBounds()
        {
            return new Vector3(basePoint.x - size, basePoint.y - size, basePoint.z - size);
        }

        public Vector3 GetMaxbounds()
        {
            return new Vector3(basePoint.x + size, basePoint.y + size, basePoint.z + size);
        }
    }

    public void SetClamp(Vector3 rot, float size)
    {
        clamping = true;
        clampPoint = new ClampPoint(rot, size);
    }

    private Vector2 centreOfPlanet;
    Vector2 mousePos;
    float angle;
    float startingZ;

    bool BeingUsed = false;
    bool BeingUsed2 = false;

    private void DragPlanet()
    {

        currentMousePosition = GetMouseHit();
        

        if (clamping)
        {
           // Debug.Log("We got here");
            if (clampPoint.GetMinBounds().x < currentMousePosition.Value.x && clampPoint.GetMinBounds().y < currentMousePosition.Value.y &&
                clampPoint.GetMaxbounds().x > currentMousePosition.Value.x && clampPoint.GetMaxbounds().y > currentMousePosition.Value.y &&
                clampPoint.GetMinBounds().z < currentMousePosition.Value.z && clampPoint.GetMaxbounds().z > currentMousePosition.Value.z)
             
            {
               // Debug.Log("Aint here douh");
                if (mouseStartPosition != null && currentMousePosition != null)
                    RotateCamera((Vector3)mouseStartPosition, (Vector3)currentMousePosition);
            }
        } else
        {
            if (!GetMouseHit().HasValue && BeingUsed)
            {
                BeingUsed = false;
                BeingUsed2 = true;
                centreOfPlanet = (Camera.main.WorldToScreenPoint(Vector3.zero));
                mousePos = (Vector2)(Input.mousePosition) - centreOfPlanet;

                angle = Mathf.Rad2Deg * Mathf.Atan2(mousePos.y, mousePos.x);
                startingZ = transform.localEulerAngles.z;
            }

            if (GetMouseHit().HasValue && !BeingUsed2)
            {
                if (mouseStartPosition != null && currentMousePosition != null)
                    RotateCamera((Vector3)mouseStartPosition, (Vector3)currentMousePosition);
            } else {
                if (!BeingUsed)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        BeingUsed2 = true;
                        centreOfPlanet = (Camera.main.WorldToScreenPoint(Vector3.zero));
                        mousePos = (Vector2)(Input.mousePosition) - centreOfPlanet;

                        angle = Mathf.Rad2Deg * Mathf.Atan2(mousePos.y, mousePos.x);
                        startingZ = transform.localEulerAngles.z;
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        Vector2 pos = (Vector2)Input.mousePosition - centreOfPlanet;
                        float currentAngle = Mathf.Rad2Deg * (Mathf.Atan2(pos.y, pos.x));

                        timer += Time.deltaTime;
                        if (timer > 2f)
                        {
                            timer = 0;
                            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.panCamera);
                        }
                        float angleDifference = (currentAngle - angle);


                        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, startingZ - angleDifference);
                        if (GetMouseHit().HasValue)
                        {
                            mouseStartPosition = GetMouseHit();
                            BeingUsed2 = false;
                            BeingUsed = true;
                        }
                    }
                }
            }

        }
    }

    Vector2 FlipY (Vector2 v)
    {
        return new Vector2(v.x, Screen.height - v.y);
    }


    private void StaticPlanet()
    {
        mouseStartPosition = null;
        currentMousePosition = null;
        BeingUsed = false;
        BeingUsed2 = false;
    }



    private void Zoom()
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;

        if (zoomAmount < 0)
        {
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.ZoomCameraOut);
        }

        //makes camera scroll slower the closer you get to the planet
        zoomAmount *= (holderDistance * slowZoomValue);

        holderDistance += zoomAmount * 1f;

        //clamp scroll amount
        holderDistance = Mathf.Clamp(holderDistance, maxZoom, minZoom);

        ZoomCheck();
    }


    public void ZoomCheck() {

        if (holderDistance / maxZoom > 0.99f)
        {
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.ZoomCameraInfull);
        }

        if (holderDistance / maxZoom > 0.5f)
        {
            float angle = Mathf.LerpAngle(cameraHolder3.localEulerAngles.x, holderDistance / maxZoom * -120 + 60, Time.deltaTime * zoomInAngleDampening);
            cameraHolder3.localEulerAngles = new Vector3(angle, 0, 0);
            // cameraHolder3.localEulerAngles = new Vector3(holderDistance / maxZoom * -120 + 60, 0, 0);
            cameraHolder2.localPosition = new Vector3(0, holderDistance / maxZoom * -40 + 20, cameraHolder2.localPosition.z);
        }
        else
        {
            float zero = Mathf.LerpAngle(cameraHolder3.localEulerAngles.x, 0, Time.deltaTime * zoomInAngleDampening);
            cameraHolder3.localEulerAngles = new Vector3(zero, 0, 0);
            //  cameraHolder3.localEulerAngles = Vector3.zero;
        }



    }


    float timer = 0;

    public void RotateCamera(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {

        BeingUsed = true;
        //normalised for odd edges
        dragEndPosition = dragEndPosition.normalized *planetRadius;
        dragStartPosition = dragStartPosition.normalized * planetRadius;

        // Cross Product
        Vector3 cross = Vector3.Cross(dragEndPosition, dragStartPosition);


        // Angle for rotation
        float angle = Vector3.SignedAngle(dragEndPosition, dragStartPosition, cross);

        //Causes Rotation of angle around the vector from Cross product
        holderTransform.RotateAround(planet.transform.position, cross, angle);
        holderTransform.LookAt(planet.transform, holderTransform.up);

        timer += Time.deltaTime;
        if (timer > 2f)
        {
            timer = 0;
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.panCamera);
        }
    }


    private static Vector3? GetMouseHit()
    {
        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("Planet"); //raycasting on the planet
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layer_mask))
        {
            return hit.point;
        }
        return null;
    }



}
