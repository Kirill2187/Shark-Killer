using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixerGroup mixer;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var sound in sounds)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixer;
            sound.source = source;

            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
        }
    }

    public void Play(string name)
    {
        var s = Array.Find(sounds, sound => sound.name.Equals(name));
        s.source.Play();
    }

    [Serializable]
    public class Sound
    {
        public AudioClip clip;
        public string name;

        [Range(0.1f, 3f)] public float pitch;

        [HideInInspector] public AudioSource source;

        [Range(0f, 1f)] public float volume;
    }
}