using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private int initialPoolSize = 20; // TODO: amount of clips playing simultaneously

    private List<AudioSource> sourcePool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // build the initial pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddSourceToPool();
        }
    }

    private AudioSource AddSourceToPool()
    {
        // create a new audiosource and add it to our pool
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        sourcePool.Add(src);
        return src;
    }

    private AudioSource GetFreeSource()
    {
        // find a source thats not currently playing
        foreach (AudioSource src in sourcePool)
        {
            if (!src.isPlaying) 
            {
                return src;
            }
        }

        // if all busy, grow the pool
        return AddSourceToPool();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) 
        {
            return;
        }
        AudioSource src = GetFreeSource();
        src.volume = volume;
        src.PlayOneShot(clip);
    }

    public void PlaySequenceWithOverlap(AudioClip first, AudioClip second, float overlapSeconds = 0.2f, float volume = 1f)
    {
        // start the first clip immediately then hand off to a coroutine
        PlayOneShot(first, volume);
        StartCoroutine(PlayAfterDelay(second, first.length - overlapSeconds, volume));
    }

    private IEnumerator PlayAfterDelay(AudioClip clip, float delay, float volume)
    {
        // wait until just before the first clip ends then fire the second
        yield return new WaitForSeconds(Mathf.Max(0f, delay));
        PlayOneShot(clip, volume);
    }
}