using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PaperCanvasController : MonoBehaviour
{
    public Transform playerCamera;   // Drag 'Main Camera' here
    public TextMeshProUGUI noteText; // Drag your Text component here
    public TextMeshProUGUI nextStepText; 
    public float distance = 0.6f;    // Distance from player's face
    
    [Header("Input Settings")]
    public InputActionReference bButtonAction; // Drag XRI RightHand/SecondaryButton here

    private bool hasChangedText = false;

    public void Show()
    {
        // Position it exactly in front of the player's eyes
        transform.position = playerCamera.position + (playerCamera.forward * distance);
        transform.LookAt(transform.position + playerCamera.forward);
        
        // Reset state and show
        hasChangedText = false;
        noteText.text = "Moo MOOO!! Moomooo moooo. Moomooo momoo. M.O.O.! MOOOOOO moomomomo mooomoo mooo. Moo mooo. Moo mooo moo mooooooo momomooooo mooo mmmmoooooo. mooo moo moo mooo MOOOOOOOOOO momooo.";
        nextStepText.text = "Bet you really understood that well. Press B on your controller to see the translation.";
        gameObject.SetActive(true);
    }

    void Update()
    {
        // Check if B button was pressed this frame
        if (bButtonAction.action.triggered)
        {
            if (!hasChangedText)
            {
                // First press: Change the text
                noteText.text = "Oh no! The neFURious Behemoth Claw pirates want to eat you. These PURRpetrators are blind but have hyPURRactive hearing, so keep quiet to avoid detection. Both the aliens and the chains have suPUR good senses of smell, so use that to your advantage. Gas urself up by drinking protein COWder. There's one in this room. To moo-ve, teleport with your joysticks. -COWnt Methany MOOscles";
                nextStepText.text = "Press B to exit";
                hasChangedText = true;
            }
            else
            {
                // Second press: Close the canvas
                gameObject.SetActive(false);
            }
        }
    }
}
