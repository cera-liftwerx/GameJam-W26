using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;

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

    public void PlayOneShot(AudioClip clip, float volume = 1f, AudioMixerGroup mixerGroup = null)
    {
        if (clip == null) 
        {
            return;
        }
        AudioSource src = GetFreeSource();
        src.volume = volume;
        Debug.Log($"[audio] playing: {clip.name}");
        src.outputAudioMixerGroup = mixerGroup;
        src.PlayOneShot(clip);
    }

    public void PlayLooping(AudioClip clip)
    {
        // create a dedicated audiosource for looping bgm
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    public void PlaySequenceWithOverlap(AudioClip first, AudioClip second, float overlapSeconds = 0.2f, float volume = 1f)
    {
        // start the first clip immediately then hand off to a coroutine
        Debug.Log($"[audio] sequence first playing: {first.name}");
        PlayOneShot(first, volume);
        StartCoroutine(PlayAfterDelay(second, first.length - overlapSeconds, 5f));
    }

    private IEnumerator PlayAfterDelay(AudioClip clip, float delay, float volume)
    {
        // wait until just before the first clip ends then fire the second
        Debug.Log($"[audio] sequence wait for {delay}");
        yield return new WaitForSeconds(Mathf.Max(0f, delay));
        Debug.Log($"[audio] sequence second playing: {clip.name}");
        PlayOneShot(clip, volume);
    }
}