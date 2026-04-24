using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
/*


FINDING THE RIGHT TIMING TOOK A LONG TIME, DO NOT CHANGE TIMING VALUES besides fadeDuration UNLESS YOU NEED TO

okay

*/
public class SimpleFadeIn : MonoBehaviour
{
    public Renderer fadeQuad;        // Assign the Quad's Renderer in the Inspector
    public float fadeDuration = 1f;  // Duration of the fade in seconds

    private Material mat;

    [SerializeField] protected GameObject cameraLocation;
    [SerializeField] protected GameObject box;
    [SerializeField] protected List<GameObject> stuffOnOff = new List<GameObject>();
    [SerializeField] private VideoPlayer startSceneVideoPlayer;
    [SerializeField] private Renderer startSceneVideoRenderer;
    [SerializeField] private VideoPlayer badEndVideoPlayer;
    [SerializeField] private Renderer badEndVideoRenderer;
    [SerializeField] private VideoPlayer goodEndVideoPlayer;
    [SerializeField] private Renderer goodEndVideoRenderer;
    public GameObject startScene;
    public GameObject badEndScene;
    public GameObject goodEndScene;


    public MapGenerator mapGenerator;
    public GameObject microphoneDetector;

    void Awake()
    {
        if (fadeQuad == null)
        {
            //Debug.LogError("Fade Quad not assigned!");
            return;
        }

        if (mapGenerator == null)
        {
            mapGenerator = GameObject.FindObjectOfType<MapGenerator>();
        }

        if (microphoneDetector == null)
        {
            microphoneDetector = GameObject.FindObjectOfType<MicrophoneDetector>(true).gameObject;
        }
    }

    void Start()
    {
        // Subscribe to end event
        startSceneVideoPlayer.loopPointReached += OnStartSceneVideoFinished;
        badEndVideoPlayer.loopPointReached += OnBadEndVideoFinished;
        goodEndVideoPlayer.loopPointReached += OnGoodEndVideoFinished;

        // set blind box
        box.transform.position = cameraLocation.transform.position;
        Vector3 rot = box.transform.eulerAngles;
        rot.y = cameraLocation.transform.eulerAngles.y; 
        box.transform.eulerAngles = rot;
    }

    public void PlayIntroVideo()
    {
        FadeFromTransparentToBlack();
        startScene.SetActive(true);
        StartCoroutine(WaitThenPlay(startSceneVideoPlayer));
    }

    public void PlayBadEndVideo()
    {
        FadeFromTransparentToBlack();
        badEndScene.SetActive(true);
        StartCoroutine(WaitThenPlay(badEndVideoPlayer));
    }

    public void PlayGoodEndVideo()
    {
        FadeFromTransparentToBlack();
        goodEndScene.SetActive(true);
        StartCoroutine(WaitThenPlay(goodEndVideoPlayer));
    }

    private IEnumerator WaitThenPlay(VideoPlayer player)
    {
        // wait for fade to black to finish before playing
        yield return new WaitForSeconds(fadeDuration + 0.5f);
        player.Play();
    }

    void OnStartSceneVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished!");

        startScene.SetActive(false);
        mapGenerator.GenerateMap();
        microphoneDetector.SetActive(true);
        FadeFromBlackToTransparent();
    }

    void OnBadEndVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished!");
        badEndScene.SetActive(false);
        FadeFromBlackToTransparent();
    }

    void OnGoodEndVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished!");
        goodEndScene.SetActive(false);
        FadeFromBlackToTransparent();
    }

    private void activationScene(bool status)
    {
        // get the top level parent of this object to exclude it
        Transform root = transform.root;

        // go through every root object in the scene and toggle them
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            // skip the parent of this object
            if (obj.transform == root) continue;

            // skip xr origin
            if (obj.tag == "Player") continue;

            // skip managers
            if (obj.name == "Game Manager") continue;
            if (obj.name == "Audio Manager") continue;

            if (obj.name == "Teleport Area Setup") continue;
            obj.SetActive(status);
        }

        // turn off stuff we dont need in xr origin
        foreach (GameObject thing in stuffOnOff)
        {
            thing.SetActive(status);
        }
    }
    public void FadeFromBlackToTransparent()
    {
        StartCoroutine(delayedFadeAway());
    }

    public void FadeFromTransparentToBlack()
    {
        box.transform.position = cameraLocation.transform.position;
        Vector3 rot = box.transform.eulerAngles;
        rot.y = cameraLocation.transform.eulerAngles.y; 
        box.transform.eulerAngles = rot;
        activationScene(false);
        StartCoroutine(Fade(0f, 1f));
        StartCoroutine(FadeRoutine(1.5f, fadeDuration+0.5f, startSceneVideoRenderer));
        StartCoroutine(FadeRoutine(1.5f, fadeDuration+0.5f, badEndVideoRenderer));
        StartCoroutine(FadeRoutine(1.5f, fadeDuration+0.5f, goodEndVideoRenderer));
    }

    protected IEnumerator delayedFadeAway()
    {
        yield return new WaitForSeconds(1f); 
        StartCoroutine(FadeRoutine(0.0f, fadeDuration-1.8f, startSceneVideoRenderer));
        StartCoroutine(FadeRoutine(0.0f, fadeDuration-1.8f, badEndVideoRenderer));
        StartCoroutine(FadeRoutine(0.0f, fadeDuration-1.8f, goodEndVideoRenderer));
        yield return new WaitForSeconds(0.25f); 
        StartCoroutine(Fade2(1f, 0f));
        yield return new WaitForSeconds(2f); 
        activationScene(true);
        yield return null;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Material mat = fadeQuad.material;
        Color color = mat.GetColor("_BaseColor");

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            mat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, alpha));
            yield return null;
        }
 
        mat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, endAlpha));
    }

    private IEnumerator Fade2(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Material mat = fadeQuad.material;
        Color color = mat.GetColor("_BaseColor");

        while (elapsed < fadeDuration+1f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (fadeDuration+1f);

            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            mat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, alpha));

            yield return null;
        }

        mat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, endAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration, Renderer videoRenderer)
    {
        yield return new WaitForSeconds(0.5f);
        Material videoMat = videoRenderer.material;
        Color color = videoMat.GetColor("_BaseColor");
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            videoMat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, newAlpha));
            time += Time.deltaTime;
            yield return null;
        }

        videoMat.SetColor("_BaseColor", new Color(color.r, color.g, color.b, targetAlpha));
    }
}
