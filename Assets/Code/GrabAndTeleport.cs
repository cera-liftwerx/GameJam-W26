using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabAndTeleport : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    [HideInInspector] public SpawnerManager manager;
    [HideInInspector] public SpawnPoint currentPoint;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        // Using selectEntered (the very start of the grab)
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (manager != null)
        {
            // 1. Tell the VR hand to drop the item immediately
            // This prevents the user from actually holding/throwing it
            args.manager.SelectExit(args.interactorObject, args.interactableObject);
            
            // 2. Teleport it to a new room
            manager.MoveToRandomPoint(this.gameObject, currentPoint);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrab);
    }
}

