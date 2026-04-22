using TMPro;
using UnityEngine;
using UnityEngine.Android;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneDetector : MonoBehaviour
{
    public float sensitivity = 100f;
    public float threshold = 0.1f;
    public bool isDetecting;

    private AudioSource audioSource;
    private string microphoneName;
    private const int sampleWindow = 128;
    private float[] waveData = new float[sampleWindow];
    [SerializeField] private TMP_Text instructions;
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
        firstTime = true;
    }

    void Update()
    {
        float loudness = GetLoudnessFromMicrophone() * sensitivity;
        isDetecting = loudness > threshold;

        if (isDetecting)
        {
            Debug.Log("Sound Detected: " + loudness);
            // Add your logic here (e.g., mouth movement, interaction)
        }

        if (isDetecting && firstTime)
        {
            instructions.text = "Just Kidding, you didn't need to do that";
            firstTime = false;
        }
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