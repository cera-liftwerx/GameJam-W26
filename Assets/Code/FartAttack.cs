using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FartAttack : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public Transform noseHand;    // Hand covering nose
    public Transform fanningHand; // Hand extended & fanning
    public ParticleSystem fartParticles;
    public Volume postProcessVolume;

    [Header("Settings")]
    public float noseThreshold = 0.3f;      // Max distance for "at nose"
    public float extensionThreshold = 0.4f; // Min distance for "extended"
    public float fanSpeedThreshold = 1f;  // Speed required to "fart"

    private Vignette vignette;
    private Vector3 lastFanPosition;
    private bool hasFiredThisStroke = false;

    public SpawnerManager spawnManager;

    void Start()
    {
        if (postProcessVolume && postProcessVolume.profile.TryGet(out vignette))
            vignette.intensity.value = 0;

        // Ensure particles don't spray constantly
        var emission = fartParticles.emission;
        emission.rateOverTime = 0;
    }

    void Update()
    {
        if (!head || !noseHand || !fanningHand) return;

        // 1. CALCULATE DISTANCES (World Space)
        float noseDist = Vector3.Distance(noseHand.position, head.position);
        float extensionDist = Vector3.Distance(fanningHand.position, head.position);

        // 2. CHECK POSE
        bool isAtNose = noseDist < noseThreshold;
        bool isExtended = extensionDist > extensionThreshold;

        // --- LOGCAT DIAGNOSTICS ---
        // Search "FART_LOG" in Logcat to see this
        Debug.Log($"FART_LOG: Nose:{noseDist:F2}/{noseThreshold} | Ext:{extensionDist:F2}/{extensionThreshold} | Pose:{isAtNose && isExtended}");

        if (isAtNose && isExtended)
        {
            Debug.Log($"FART_LOG: controllers in right position");
            // Visual feedback (Green Tint)
            if (vignette) vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.45f, Time.deltaTime * 5f);

            // 3. TRACK FANNING MOTION (Velocity-based)
            float currentSpeed = (fanningHand.position - lastFanPosition).magnitude / Time.deltaTime;
            
            // Log speed when in pose
            Debug.Log($"FART_LOG: Fanning Speed: {currentSpeed:F2}/{fanSpeedThreshold}, hasFiredThisStroke: {hasFiredThisStroke}");

            if (currentSpeed > fanSpeedThreshold && !hasFiredThisStroke)
            {
                Debug.Log("FART_LOG: ATTACK TRIGGERED!");
                TriggerFartBurst();
                hasFiredThisStroke = true;
            }

            // Reset stroke lock if hand slows down
            if (currentSpeed < 0.2f) hasFiredThisStroke = false;
        }
        else
        {
            // Reset visuals
            if (vignette) vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0, Time.deltaTime * 2f);
            hasFiredThisStroke = false;
        }

        lastFanPosition = fanningHand.position;
    }

    void TriggerFartBurst() {
        bool canFart = spawnManager.ReleaseFart();

        if (!canFart) return;
        // 1. Start the bubbles at the hand
        fartParticles.transform.position = fanningHand.position;

        // 2. IGNORE the hand's rotation. Use the HEAD's forward direction.
        // This ensures that even if you fan wildly, the "attack" goes where you are looking.
        fartParticles.transform.rotation = Quaternion.LookRotation(head.forward);
        
        // 3. Fire!
        fartParticles.Emit(10); 
        
        Debug.Log("FART_LOG: Bubbles fired exactly where you are looking!");
    }
}
