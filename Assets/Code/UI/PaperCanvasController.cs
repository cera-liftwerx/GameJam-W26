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

    // private bool hasChangedText = false;
    private int currIndex = 0;
    private string[] dialogueLines;
    private string[] nextStepLines;

    private void Awake()
    {
        dialogueLines = new string[]
        {
            "Moo MOOO!! Moomooo moooo. Moomooo momoo. M.O.O.! MOOOOOO moomomomo mooomoo mooo. Moo mooo. Moo mooo moo mooooooo momomooooo mooo mmmmoooooo. mooo moo moo mooo MOOOOOOOOOO momooo.",
            "Oh no! Hello fellow cow who has been captured, I was once in your position. Here are some tips to get home: The aliens are from the neFURious Behemoth Claw clan and they want to eat you. These PURRpetrators are blind but have hyPURRactive hearing, so KEEP QUIET to avoid detection. To moo-ve, teleport with your joysticks.",
            "The aliens suPUR good senses of smell, and since you are a cow, when you hit them hard you can't help but fart and the fart smell injures them.",
            "Gas urself up and therefore heal by drinking protein COWder. There's one in this room. You can grab it by squeezing your grip trigger.",
            "Better get Mooving now. -COWnt Methany MOOscles"
        };

        nextStepLines = new string[]
        {
            "Bet you really understood that well. Press B on your controller to see the translation.",
            "Press B on your right controller to see next",
            "Press B to exit"
        };
    }

    private void Start()
    {
        // Display the first line immediately
        if (dialogueLines.Length > 0)
        {
            noteText.text = dialogueLines[0];
            nextStepText.text = nextStepLines[0];
        }

        transform.position = playerCamera.position + (playerCamera.forward * distance);
        transform.LookAt(transform.position + playerCamera.forward);
    }

    void OnEnable() => bButtonAction.action.performed += OnBButtonPressed;
    void OnDisable() => bButtonAction.action.performed -= OnBButtonPressed;

    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        currIndex++;

        if (currIndex < dialogueLines.Length)
        {
            noteText.text = dialogueLines[currIndex];
            if (currIndex < dialogueLines.Length - 1)
            {
                nextStepText.text = nextStepLines[1];
            } else
            {
                nextStepText.text = nextStepLines[2];
            }
        }
        else
        {
            // Close the canvas after the last line
            gameObject.SetActive(false); 
        }
    }

    // public void Show()
    // {
    //     // Position it exactly in front of the player's eyes
    //     transform.position = playerCamera.position + (playerCamera.forward * distance);
    //     transform.LookAt(transform.position + playerCamera.forward);
        
    //     // Reset state and show
    //     hasChangedText = false;
    //     noteText.text = "Moo MOOO!! Moomooo moooo. Moomooo momoo. M.O.O.! MOOOOOO moomomomo mooomoo mooo. Moo mooo. Moo mooo moo mooooooo momomooooo mooo mmmmoooooo. mooo moo moo mooo MOOOOOOOOOO momooo.";
    //     nextStepText.text = "Bet you really understood that well. Press B on your controller to see the translation.";
    //     gameObject.SetActive(true);
    // }

    // void Update()
    // {
    //     // Check if B button was pressed this frame
    //     if (bButtonAction.action.triggered)
    //     {
    //         if (!hasChangedText)
    //         {
    //             // First press: Change the text
    //             noteText.text = "Oh no! The neFURious Behemoth Claw pirates want to eat you. These PURRpetrators are blind but have hyPURRactive hearing, so keep quiet to avoid detection. Both the aliens and the chains have suPUR good senses of smell, so use that to your advantage. Gas urself up by drinking protein COWder. There's one in this room. To moo-ve, teleport with your joysticks. -COWnt Methany MOOscles";
    //             nextStepText.text = "Press B to exit";
    //             hasChangedText = true;
    //         }
    //         else
    //         {
    //             // Second press: Close the canvas
    //             gameObject.SetActive(false);
    //         }
    //     }
    // }
}