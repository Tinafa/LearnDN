using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource efxSource;
        public AudioSource musicSource;
        public static SoundManager instance = null;
        public float lowPitchRange = .95f;
        public float highPitchRange = 1.0f;

        void Awake()
        {
            if (null == instance)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            efxSource = GameObject.Find("EfxSound").GetComponent<AudioSource>();
            musicSource = GameObject.Find("MusicSound").GetComponent<AudioSource>();
        }

        public void PlaySingle(AudioClip clip)
        {
            efxSource.clip = clip;
            efxSource.Play();
        }
    }
}
