using UnityEngine;
using System.Collections;

public class cameraRotationonZ : MonoBehaviour
{

    public float rotateSpeed = 0.5f;
    public float maxAngle = 360.0f;
    public Transform target;
    public float originAngle = 0.0f;
    public float currentAngle;
    private float rotSpeed;



    void Update()
    {

        rotSpeed = rotateSpeed * Time.deltaTime;

        currentAngle = target.localEulerAngles.z;
       
        currentAngle = (currentAngle > 180) ? currentAngle - 360 : currentAngle;


        if (currentAngle > maxAngle)
            currentAngle = maxAngle;
        

        else if (currentAngle < -maxAngle)        
            currentAngle = -maxAngle;
        

        
        if (Input.GetKey(KeyCode.A))
        {
            if (currentAngle < maxAngle)
                target.Rotate(0, 0, rotSpeed);            
        }


        if (Input.GetKey(KeyCode.D))
        {
            if (currentAngle > -maxAngle)
                target.Rotate(0, 0, -rotSpeed);
            
        }


     //   GetAngleBetween()


        
    }




    public float GetAngleBetween(Vector2 A, Vector2 B)
    {
        float DotProd = Vector2.Dot(A, B);
        float Length = A.magnitude * B.magnitude;
        return Mathf.Acos(DotProd / Length);
    }
}
