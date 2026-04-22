using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    private Animator animator;

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
        if (!other.CompareTag("Player")) return;
        animator.SetBool("character_nearby", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        animator.SetBool("character_nearby", false);
    }
}