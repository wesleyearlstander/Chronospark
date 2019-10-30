using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTutorialScript : MonoBehaviour {


    public NewCameraScript ncs;
    public float zoomSpeed;
    public float moveSpeed;
    public bool test;
    public GameObject camHolder3;
    public GameObject placeHolder;
    public float testSpeed;
    float speed;
    public bool clampRotation = false;
    public bool clampPosition = false;
    public bool clampZoom = false;
    Vector3 pos = new Vector3(0, 0, -150);



    private void Start()
    {
        if (!GameController.instance.GetComponent<MovieController>().isActiveAndEnabled)
        {
            StartCoroutine(PanCamera(17.96f, 147.16f, -22.78f, false));
            StartCoroutine(Zoom(92f));
        }
        //StartCoroutine(PanCamera(17.96f, 147.16f, -22.78f));
        //StartCoroutine(Zoom(92f));

    }

    private void Update()
    {
        // ClampCameraRotation(-1, 1, -1, 1, -1, 1);
        if (clampPosition)
            ClampMovement(pos, 5);
        //  ClampCameraMovement(-10, 10, -20, 20, -170, -110);       
        //  PanCamera();    
    }

    Vector3 rot = new Vector3(0, 0, 0);

    public void PanTest()
    {

        //   StartCoroutine(ClampPosition(pos, 30));

        StartCoroutine(ClampRotPoint(rot, 5));
    }





    public IEnumerator ClampRotPoint(Vector3 rotationPoint, float size)
    {
        Vector3 p = rotationPoint;
        float s = size;
        clampRotation = true;

        while (clampRotation)
        {
            ClampCamRotation(p, s);

            yield return new WaitForEndOfFrame();
        }



        yield return new WaitForEndOfFrame();

    }


   


    public IEnumerator ClampPosition(Vector3 pos, float size)
    {

        Vector3 p = pos;
        float s = size;
        clampPosition = true;

        while (clampPosition)
        {
            ClampPosition(p, s);

            yield return new WaitForEndOfFrame();
        }



        yield return new WaitForEndOfFrame();
    }






    #region // Working IEnumerators

    public IEnumerator ClampZoomWithinBounds(float min, float max)
    {
        float m = min;
        float x = max;
        clampZoom = true;
        while (clampZoom)
        {
            ClampCamZoom(m, x);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

    }

    public IEnumerator Zoom (float value)
    {
        GameController.instance.zooming = true;
        float start = ncs.holderDistance;
        float timer = 0;
        if (start > -value)//zoom in
        {
            while (ncs.holderDistance > (-value + 1))//(ncs.holderDistance > (-value - (value + start) / 20))
            {
                ncs.holderDistance = Mathf.Lerp(ncs.holderDistance, -value, timer * zoomSpeed / (value + start));
                ncs.ZoomCheck();
                timer += Time.deltaTime;
                yield return null;
            }
        } else //zoom out
        {
            while (ncs.holderDistance < (-value - 1))//(ncs.holderDistance < (-value + (start+value)/20))
            {
                ncs.holderDistance = Mathf.Lerp(ncs.holderDistance, -value, timer * zoomSpeed / -(start + value));
                ncs.ZoomCheck();
                timer += Time.deltaTime;
                yield return null;
            }
        }
        GameController.instance.zooming = false;
    }

    public IEnumerator ZoomOut(float _timeBetweenPans)
    {
        if (ncs.holderDistance <= -41f)
        {
            while (ncs.holderDistance < -39f)
            {
                ncs.holderDistance = Mathf.Lerp(ncs.holderDistance, -38f, Time.deltaTime * testSpeed);
                ncs.ZoomCheck();
                yield return null;
            }
        }

        yield return new WaitForSeconds(_timeBetweenPans);
    }

    public IEnumerator ZoomIn(float _timeBetweenPans, float zoomAmount)
    {
        yield return new WaitForSeconds(_timeBetweenPans);


        //Zoom in
        while (ncs.holderDistance > (-zoomAmount + zoomAmount/100))
        {
            ncs.holderDistance = Mathf.Lerp(ncs.holderDistance, -zoomAmount, Time.deltaTime * zoomSpeed);
            ncs.ZoomCheck();
            yield return null;
        }
    }


    public IEnumerator PanRotationOnZ()
    {

        
        Vector3 currentRotation = new Vector3(ncs.holderTransform.root.localEulerAngles.x, ncs.holderTransform.root.localEulerAngles.y, ncs.holderTransform.root.localEulerAngles.z);
        Vector3 moveTo = new Vector3(ncs.holderTransform.root.localEulerAngles.x, ncs.holderTransform.root.localEulerAngles.y, -38.601f);


        float timer = 0;

          while(timer < 1)
            {
            timer += Time.deltaTime/2   ;

           
            float f = currentRotation.z;
            if (f > 180.0f)
                f -= 360.0f;

            ncs.holderTransform.root.localEulerAngles = new Vector3(ncs.holderTransform.root.localEulerAngles.x, ncs.holderTransform.root.localEulerAngles.y, Mathf.Lerp(f, moveTo.z, timer));


            
            yield return new WaitForEndOfFrame();

            }

        yield return new WaitForEndOfFrame();

    }

    bool panning = false;

    public IEnumerator PanCamera(float xPos, float yPos, float zPos, bool rotatez)
    {
        if (!panning)
        {
            panning = true;
            //Pan to position
            Vector3 moveTo = new Vector3(xPos, yPos, zPos);
            Vector3 zeroPos = new Vector3(0, 0, -150);
            Vector3 zeroRot = new Vector3(0, 0, 0);

            GameController.instance.panning = true;

            float timer = 0;
            Vector3 startPos = ncs.transform.position;
            Vector3 direction = moveTo.normalized;
            float distance = startPos.magnitude;
            moveTo = direction * distance;
            while (Mathf.Pow(timer, 2) * testSpeed < 1)
            {
                ncs.holderTransform.position = Vector3.Slerp(startPos, moveTo, Mathf.Pow(timer, 2) * testSpeed);
                ncs.holderTransform.LookAt(ncs.planet.transform, ncs.holderTransform.transform.up);
                timer += Time.deltaTime / 2;
                yield return new WaitForEndOfFrame();
            }
            GameController.instance.panning = false;
            panning = false;
        }
        if (rotatez)
        StartCoroutine(PanRotationOnZ());
        //Can be used to change rotation
        // ncs.holderTransform.rotation = Quaternion.RotateTowards(ncs.holderTransform.rotation, placeHolder.transform.rotation, testSpeed * Time.deltaTime);

    }

    public IEnumerator PanCamera(Transform t)
    {
        if (!panning)
        {
            panning = true;
            //Pan to position
            Vector3 moveTo = new Vector3(t.position.x, t.position.y, t.position.z);
            Vector3 zeroPos = new Vector3(0, 0, -150);
            Vector3 zeroRot = new Vector3(0, 0, 0);
            Human h = null;
            if (t.GetComponent<HumanController>() != null)
            {
                h = t.GetComponent<HumanController>().human;
            }
            GameController.instance.panning = true;

            float timer = 0;
            Vector3 startPos = ncs.holderTransform.position;
            Vector3 direction = moveTo.normalized;
            float distance = startPos.magnitude;
            moveTo = direction * distance;
            while ((ncs.holderTransform.position.y < moveTo.y + Mathf.Epsilon || ncs.holderTransform.position.y > moveTo.y - Mathf.Epsilon) && (ncs.holderTransform.position.x < moveTo.x + Mathf.Epsilon || ncs.holderTransform.position.x > moveTo.x - Mathf.Epsilon) && (ncs.holderTransform.position.z < moveTo.z + Mathf.Epsilon || ncs.holderTransform.position.z > moveTo.z - Mathf.Epsilon) && Mathf.Pow(timer, 2) * testSpeed < 1 && t.gameObject != null)
            {
                if (h != null)
                    if (GameController.instance.gameManager.humanManager.humans.Contains(h))
                        moveTo = new Vector3(t.position.x, t.position.y, t.position.z);
                Transform c = GetComponent<NewCameraScript>().cameraHolder2;
                if (GetComponent<NewCameraScript>().holderDistance / GetComponent<NewCameraScript>().maxZoom > 0.5f)
                {
                    Vector3 temp = c.position.normalized;
                    Vector3 temp2 = c.transform.root.position.normalized;
                    Vector3 difference = temp2 - temp;
                    direction = moveTo.normalized + difference/1.4f;
                }
                else
                {
                    direction = moveTo.normalized;
                }
                direction = direction.normalized;
                moveTo = direction * distance;
                ncs.holderTransform.position = Vector3.Slerp(startPos, moveTo, Mathf.Pow(timer, 2) * testSpeed);
                ncs.holderTransform.LookAt(ncs.planet.transform, ncs.holderTransform.transform.up);
                timer += Time.deltaTime / 2;
                yield return new WaitForEndOfFrame();
            }

            GameController.instance.panning = false;
            panning = false;
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.cameraPanned);
        }


        //Can be used to change rotation
        // ncs.holderTransform.rotation = Quaternion.RotateTowards(ncs.holderTransform.rotation, placeHolder.transform.rotation, testSpeed * Time.deltaTime);

    }



    #endregion


    #region Functions


    public void ClampCamZoom(float minZoomClamp, float maxZoomClamp)
    {
        ncs.holderDistance = Mathf.Clamp(ncs.holderDistance, -maxZoomClamp, minZoomClamp);     
    }
   

    public void ClampMovement (Vector3 pos, float _size)
    {
        ClampCameraMovement(pos.x - _size, pos.x + _size, pos.y - _size, pos.y + _size, pos.z - _size, pos.z + _size);
    }


    public void ClampCameraMovement(float xMinValue, float xMaxValue, float yMinvalue, float yMaxValue, float zMinValue, float zMaxValue)
    {
        
        Vector3 holder1Pos = ncs.holderTransform.position;

        holder1Pos.x = Mathf.Clamp(holder1Pos.x, xMinValue, xMaxValue);
        holder1Pos.y = Mathf.Clamp(holder1Pos.y, yMinvalue, yMaxValue);
        holder1Pos.z = Mathf.Clamp(holder1Pos.z, zMinValue, zMaxValue);

        ncs.holderTransform.position = holder1Pos;

        ncs.holderTransform.LookAt(ncs.planet.transform, ncs.holderTransform.transform.up);
    }


    public void ClampCamRotation(Vector3 clampPoint, float size)
    {
        ClampCameraRotation(clampPoint.x - size, clampPoint.x + size, clampPoint.y - size, clampPoint.y + size, clampPoint.z - size, clampPoint.z + size);
    }

    public void ClampCameraRotation(float xMinValue, float xMaxValue, float yMinvalue, float yMaxValue, float zMinValue, float zMaxValue)
    {
        ncs.holderTransform.eulerAngles = new Vector3(ClampAngle(ncs.holderTransform.eulerAngles.x, xMinValue, xMaxValue),
                                                            ClampAngle(ncs.holderTransform.eulerAngles.y, yMinvalue, yMaxValue),
                                                            ClampAngle(ncs.holderTransform.eulerAngles.z, zMinValue, zMaxValue));
    }


    private float ClampAngle(float angle, float min, float max)
    {

        angle = NormalizeAngle(angle);
        if (angle > 180)
        {
            angle -= 360;
        }
        else if (angle < -180)
        {
            angle += 360;
        }

        min = NormalizeAngle(min);
        if (min > 180)
        {
            min -= 360;
        }
        else if (min < -180)
        {
            min += 360;
        }

        max = NormalizeAngle(max);
        if (max > 180)
        {
            max -= 360;
        }
        else if (max < -180)
        {
            max += 360;
        }

        return Mathf.Clamp(angle, min, max);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;
        while (angle < 0)
            angle += 360;
        return angle;
    }


    #endregion


}
