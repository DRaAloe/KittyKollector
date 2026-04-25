using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PawZoomTransition : MonoBehaviour
{
    public RectTransform paw;

    [Header("Timing")]
    public float holdTime = 0.2f;
    public float zoomDuration = 0.5f;

    [Header("Zoom")]
    public float targetScale = 35f;

    [Header("Scene")]
    public string sceneToLoad;

    bool isRunning = false;

    void Start()
    {
        paw.gameObject.SetActive(false);
    }

    public void StartTransition()
    {
        if (isRunning) return;
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        isRunning = true;

        paw.gameObject.SetActive(true);
        paw.localScale = Vector3.one;

        // small hold so player sees paw
        yield return new WaitForSeconds(holdTime);

        // zoom phase
        float t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;

            float progress = t / zoomDuration;
            float curve = progress * progress * progress;

            float scale = Mathf.Lerp(1f, targetScale, curve);

            paw.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        // MAKE SURE FULL ANIMATION FINISHES BEFORE SCENE LOAD
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(sceneToLoad);
    }
}