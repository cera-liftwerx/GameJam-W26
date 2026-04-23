using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
/*


FINDING THE RIGHT TIMING TOOK A LONG TIME, DO NOT CHANGE TIMING VALUES besides fadeDuration UNLESS YOU NEED TO



*/
public class SimpleFadeIn : MonoBehaviour
{
    public Renderer fadeQuad;        // Assign the Quad's Renderer in the Inspector
    public float fadeDuration = 1f;  // Duration of the fade in seconds

    private Material mat;

    [SerializeField] protected GameObject cameraLocation;
    [SerializeField] protected GameObject box;
    [SerializeField] protected List<GameObject> stuffOnOff = new List<GameObject>();
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Renderer videoRenderer;
    void Awake()
    {
        if (fadeQuad == null)
        {
            //Debug.LogError("Fade Quad not assigned!");
            return;
        }
    }

    void Start()
    {
        // Subscribe to end event
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start video
        videoPlayer.Play();
        FadeFromTransparentToBlack();
    }
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished!");

        // Call your function here
        FadeFromBlackToTransparent();
    }

    private void activationScene(bool status)
    {
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
    }

    protected IEnumerator delayedFadeAway()
    {
        yield return new WaitForSeconds(1f); 
        StartCoroutine(FadeRoutine(0.0f, fadeDuration-1.8f));
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

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
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
