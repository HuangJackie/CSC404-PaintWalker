using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class SoundManager
    {
        public AudioSource[] AudioSources = Array.Empty<AudioSource>();

        public void SetAudioSources(AudioSource[] audioSource)
        {
            AudioSources = audioSource;
        }

        public void PlayRandom()
        {
            if (AudioSources.Length != 0)
            {
                AudioSource audioSource = AudioSources[Random.Range(0, AudioSources.Length)];
                audioSource.Play();
            }
        }
    }
}