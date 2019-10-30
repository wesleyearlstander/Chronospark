using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
    public class Sounds {

        public string soundName;
        public AudioClip audioClip;
        private AudioSource audioSource;
        [Range(0, 1)]
        public float volume;
        [Range(0, 1)]
        public float pitch;

        public void SetAudio(AudioSource aSource)
        {
            audioSource = aSource;
            audioSource.clip = audioClip;
        }

        public void PlayClip()
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
        }

    }




