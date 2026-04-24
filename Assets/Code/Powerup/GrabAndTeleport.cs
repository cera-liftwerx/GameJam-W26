using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabAndTeleport : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    [HideInInspector] public SpawnerManager manager;
    [HideInInspector] public SpawnPoint currentPoint;
    private PlayerCharacter playerCharacter;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        // Using selectEntered (the very start of the grab)
        grabInteractable.selectEntered.AddListener(OnGrab);
        manager = GameObject.FindObjectOfType<SpawnerManager>();
        playerCharacter = GameObject.FindObjectOfType<PlayerCharacter>();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Debug.Log($"[fart] in OnGrab");
        if (manager != null)
        {
            // 1. Tell the VR hand to drop the item immediately
            // This prevents the user from actually holding/throwing it
            args.manager.SelectExit(args.interactorObject, args.interactableObject);

            manager.replenishFart();
            playerCharacter.Heal(30);
            // 2. Teleport it to a new room
            manager.MoveToRandomPoint(this.gameObject, currentPoint);
        } else
        {
            // Debug.Log($"[fart] manager is null");
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrab);
    }
}
