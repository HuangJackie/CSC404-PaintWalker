using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class SoundManager
    {
        public AudioSource[] AudioSources = Array.Empty<AudioSource>();
        public AudioSource AudioSource;

        public void SetAudioSources(AudioSource[] audioSource)
        {
            AudioSources = audioSource;
        }

        public void SetAudioSource(AudioSource audioSource)
        {
            AudioSource = audioSource;
        }

        public void PlayAudio()
        {
            AudioSource.Play();
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