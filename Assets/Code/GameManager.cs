using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip ambienceClip;
    public AudioClip playerDeathClip;
    public AudioClip confettiClip;

    [Header("UI Screens")]
    public GameObject startScreen;
    public GameObject badEndScreen;
    public GameObject goodEndScreen;

    [Header("Start Screen")]
    public Slider difficultySlider;
    public TMP_Dropdown cowDropdown;
    public Button startButton;

    [Header("Bad End Screens")]
    public TextMeshProUGUI loserText;
    public Button restartButton;

    [Header("Good End Screens")]
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI victoryNameText;
    public TextMeshProUGUI victoryTimeText;
    public TextMeshProUGUI leaderboardText;
    public Button leaderboardButton;
    public Button restartButton2;

    [Header("Scene References")]
    public SimpleFadeIn simpleFadeIn;

    private float runtimeStart;
    private float runtimeSeconds;
    private bool timerRunning = false;

    private int difficulty;
    private string selectedCowName;

    private readonly string[] cowNames = new string[]
    {
        "The Stinker",
        "Moomoo Moo Moo",
        "Winner winner steak dinner",
        "Dairy Queen",
        "Monke",
        "Moogan Freeman",
        "Glen",
        "Sir Loin",
        "Tailwind",
        "Whiffzard"
    };

    private string leaderboardPath;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // bgm
        AudioManager.Instance.PlayLooping(ambienceClip);

        // set leaderboard file path in locallow
        leaderboardPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low",
            Application.companyName,
            Application.productName,
            "leaderboard.txt"
        );

        // populate cow dropdown
        cowDropdown.ClearOptions();
        cowDropdown.AddOptions(new System.Collections.Generic.List<string>(cowNames));

        // wire up buttons
        startButton.onClick.AddListener(OnStartPressed);
        restartButton.onClick.AddListener(OnRestartPressed);
        restartButton2.onClick.AddListener(OnRestartPressed);
        leaderboardButton.onClick.AddListener(OnLeaderboardPressed);

        // show only the start screen at launch
        ShowScreen(startScreen);
    }

    void OnStartPressed()
    {
        difficulty = Mathf.RoundToInt(difficultySlider.value);
        selectedCowName = cowNames[cowDropdown.value];

        // hide all ui so the video plays clean
        ShowScreen(null);

        // start intro vid
        simpleFadeIn.PlayIntroVideo();
    }

    public void BeginTimer()
    {
        runtimeStart = Time.time;
        timerRunning = true;
    }

    public void OnPlayerDied()
    {
        timerRunning = false;
        // TODO: start bad ending
        AudioManager.Instance.PlayOneShot(playerDeathClip);
        ShowScreen(badEndScreen);
    }

    public void OnBossDefeated()
    {
        if (!timerRunning) return;
        timerRunning = false;

        runtimeSeconds = Time.time - runtimeStart;
        WriteToLeaderboard(selectedCowName, runtimeSeconds);

        // update victory screen text
        victoryNameText.text = selectedCowName;
        victoryTimeText.text = FormatTime(runtimeSeconds);

        // TODO: start good ending
        AudioManager.Instance.PlayOneShot(confettiClip);
        ShowScreen(goodEndScreen);
        DisplayLeaderboard();
    }

    void OnRestartPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    void OnLeaderboardPressed()
    {
        DisplayLeaderboard();
    }

    void WriteToLeaderboard(string cowName, float seconds)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(leaderboardPath));
            string entry = $"{cowName},{seconds:F2}\n";
            File.AppendAllText(leaderboardPath, entry);
        }
        catch (Exception e)
        {
            Debug.LogError("// leaderboard write failed: " + e.Message);
        }
    }

    void DisplayLeaderboard()
    {
        if (!File.Exists(leaderboardPath))
        {
            leaderboardText.text = "no scores yet";
            return;
        }

        string[] lines = File.ReadAllLines(leaderboardPath);
        string display = "";

        // sort by time ascending
        System.Array.Sort(lines, (a, b) =>
        {
            float ta = ParseTime(a);
            float tb = ParseTime(b);
            return ta.CompareTo(tb);
        });

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length == 2)
            {
                string name = parts[0];
                float t = ParseTime(line);
                display += $"{name}   {FormatTime(t)}\n";
            }
        }

        leaderboardText.text = display.TrimEnd();
    }

    float ParseTime(string line)
    {
        string[] parts = line.Split(',');
        if (parts.Length == 2 && float.TryParse(parts[1], out float t))
            return t;
        return float.MaxValue;
    }

    string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        int ms = Mathf.FloorToInt((seconds % 1) * 100);
        return $"{m:00}:{s:00}.{ms:00}";
    }

    void ShowScreen(GameObject screen)
    {
        if (startScreen) startScreen.SetActive(screen == startScreen);
        if (badEndScreen) badEndScreen.SetActive(screen == badEndScreen);
        if (goodEndScreen) goodEndScreen.SetActive(screen == goodEndScreen);
    }
}