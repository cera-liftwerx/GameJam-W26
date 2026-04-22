using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        animator.Play("door_2_open");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        animator.Play("door_2_close");
    }
}