using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{

    public static DontDestroyOnLoad instanceRef;

    private void Awake()
    {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instanceRef != this)
            Destroy(gameObject);
    }
}
