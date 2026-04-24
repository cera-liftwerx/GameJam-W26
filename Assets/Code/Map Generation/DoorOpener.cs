using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public bool isDisabled = false;
    private Animator animator;
    public AudioClip doorOpenClip;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"no animator found in parent of {gameObject.name}");
        }
        else
        {
            Debug.Log($"dooropener ready on {gameObject.name}, animator found: {animator.gameObject.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDisabled) return;
        if (!other.CompareTag("Player")) return;
        animator.SetBool("character_nearby", true);
        AudioManager.Instance.PlayOneShot(doorOpenClip);
    }

    void OnTriggerExit(Collider other)
    {
        if (isDisabled) return;
        if (!other.CompareTag("Player")) return;
        animator.SetBool("character_nearby", false);
        AudioManager.Instance.PlayOneShot(doorOpenClip);
    }
}