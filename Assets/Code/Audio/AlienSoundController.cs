using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AlienSoundController : MonoBehaviour
{
    [Header("clips for each state")]
    public AudioClip idleClip;
    public AudioClip walkClip;
    public AudioClip attackClip;
    public AudioClip deathClip;

    [Header("random delay range in seconds")]
    public float minDelay = 2f;
    public float maxDelay = 5f;

    private Animator animator;
    private Coroutine soundLoop;

    private static readonly int stateIdle = Animator.StringToHash("alien_idle");
    private static readonly int stateWalk = Animator.StringToHash("alien_walk");
    private static readonly int stateAttack = Animator.StringToHash("alien_attack");
    private static readonly int stateDeath = Animator.StringToHash("alien_death");

    [SerializeField] private AudioMixerGroup mixerGroup;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        soundLoop = StartCoroutine(AmbientSoundLoop());
    }

    private void OnDisable()
    {
        if (soundLoop != null) 
        {
            StopCoroutine(soundLoop);
        }
    }

    private IEnumerator AmbientSoundLoop()
    {
        while (true)
        {
            // wait a random amount before playing anything
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            AudioClip clipToPlay = GetClipForCurrentState();

            if (clipToPlay != null)
            {
                AudioManager.Instance.PlayOneShot(clipToPlay, 1f, mixerGroup);
            }
        }
    }

    private AudioClip GetClipForCurrentState()
    {
        // check what state the animator is currently in and return the matching clip
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.shortNameHash == stateIdle)
        {
            return idleClip;
        }
        if (info.shortNameHash == stateWalk)
        {
            return walkClip;
        }
        if (info.shortNameHash == stateAttack)
        {
            return attackClip;
        }
        if (info.shortNameHash == stateDeath)
        {
            return deathClip;
        }

        return null;
    }

    // TODO: stop the loop on death by calling StopCoroutine(soundLoop)
    // or just let OnDisable handle it if we disable the GameObject on death
}