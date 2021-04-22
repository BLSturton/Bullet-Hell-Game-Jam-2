using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAudioManager : MonoBehaviour
{
    public static TempAudioManager instance;

    public List<AudioClip> clips;
    private List<AudioSource> sources;

    private void Awake()
    {
        instance = this;

        sources = new List<AudioSource>();

        for (int x = 0; x < 15; x++)
        {
            sources.Add(gameObject.AddComponent(typeof(AudioSource)) as AudioSource);
            sources[x].playOnAwake = false;
        }
    }

    public void PlayClip(int index)
    {
        foreach(AudioSource source in sources)
        { 
            if (!source.isPlaying)
            {
                source.clip = clips[index];
                source.Play();
                return;
            }

        }
    }
}
