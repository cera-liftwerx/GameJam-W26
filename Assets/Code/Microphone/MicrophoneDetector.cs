using TMPro;
using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneDetector : MonoBehaviour
{
    public float sensitivity = 100f;
    public float threshold = 0.8f;
    public bool isDetecting;

    private AudioSource audioSource;
    private string microphoneName;
    private const int sampleWindow = 128;
    private float[] waveData = new float[sampleWindow];
    private GameObject instructions;
    private TMP_Text instructionsText;
    [SerializeField] private Slider volumeBar;
    [SerializeField] private GameObject volumeBarText;
    [SerializeField] private RectTransform thresholdLine;
    bool firstTime;
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        audioSource = GetComponent<AudioSource>();
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0]; // Use first mic
            audioSource.clip = Microphone.Start(microphoneName, true, 1, 44100);
            audioSource.loop = true;
            // Wait for mic to start
            while (!(Microphone.GetPosition(microphoneName) > 0)) { }
            audioSource.Play();
        }

        instructions = GameObject.FindWithTag("Instructions");
        instructionsText = instructions.transform.Find("Spatial Panel Scroll/Background/Body").GetComponent<TextMeshProUGUI>();

        // position the threshold line as a percentage of the bar width
        if (thresholdLine != null)
        {
            // move the line to where the threshold sits on the bar
            float thresholdPercent = Mathf.Clamp01(threshold);
            thresholdLine.anchorMin = new Vector2(thresholdPercent, 0f);
            thresholdLine.anchorMax = new Vector2(thresholdPercent, 1f);
            thresholdLine.anchoredPosition = Vector2.zero;
        }

        firstTime = true;
    }

    void Update()
    {
        float loudness = GetLoudnessFromMicrophone() * sensitivity;
        isDetecting = loudness > threshold;

        // update the volume bar to show current loudness, clamped to 0-1
        if (volumeBar != null)
        {
            volumeBar.value = Mathf.Clamp01(loudness);
        }

        if (isDetecting && firstTime)
        {
            Debug.Log("Sound Detected: " + loudness);
            instructionsText.text = "Just Kidding, you didn't need to do that";
            firstTime = false;
            StartCoroutine(HideInstructions());
        }
    }

    private IEnumerator HideInstructions()
    {
        yield return new WaitForSeconds(3f);
        instructions.SetActive(false);
        volumeBar.gameObject.SetActive(true);
        volumeBarText.SetActive(true);
    }

    float GetLoudnessFromMicrophone()
    {
        int micPosition = Microphone.GetPosition(microphoneName) - (sampleWindow + 1);
        if (micPosition < 0) return 0;
        
        audioSource.clip.GetData(waveData, micPosition);
        
        float totalLoudness = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }
        
        return totalLoudness / sampleWindow;
    }
}