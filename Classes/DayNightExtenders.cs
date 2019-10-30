using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DayNightExtenders
{
    public static bool IsDay(this Transform g)
    {
        if (AtmosphereZoomer.az.sun != null)
        {
            Vector3 sunDirection = AtmosphereZoomer.az.sun.position.normalized;
            Vector3 moonDirection = AtmosphereZoomer.az.moon.position.normalized;
            Vector3 cameraDirection = g.position.normalized;
            if ((sunDirection - cameraDirection).magnitude <= (moonDirection - cameraDirection).magnitude) //closer to sun
            {
                return true;
            }
            else //closer to moon
            {
                return false;
            }
        }
        return false;
    }
}
