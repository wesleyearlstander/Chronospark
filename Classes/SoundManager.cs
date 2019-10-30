using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public Sounds[] count;

    private GameObject gObject;

    public static SoundManager sm;

    private void Awake()
    {
        sm = this;
    }

    private void Start()
    {
        for(int i=0; i<count.Length; i++)
        {
            gObject = new GameObject("Sound " + i + " " + count[i].soundName);
            count[i].SetAudio(gObject.AddComponent<AudioSource>());
        }

        
    }

    public void Play(string name)
    {
        for (int i = 0; i < count.Length; i++)
        {
            if (count[i].soundName == name)
            {
                count[i].PlayClip();
                return;
            }
        }
    }

}
