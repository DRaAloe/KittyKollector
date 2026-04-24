using UnityEngine;
using System.Collections;

public class PawZoomTransition : MonoBehaviour
{
    public RectTransform paw;

    public float holdTime = 0.4f;     // how long it stays still
    public float zoomDuration = 0.6f; // how fast it zooms
    public float targetScale = 30f;

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

        // show paw
        paw.gameObject.SetActive(true);
        paw.localScale = Vector3.one;

        // PHASE 1: HOLD (this is what you were missing)
        yield return new WaitForSeconds(holdTime);

        // PHASE 2: ZOOM
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

        paw.localScale = Vector3.one * targetScale;
    }
}