using UnityEngine;
using System.Collections;

public class PawZoomTransition : MonoBehaviour
{
    public RectTransform paw;

    public float duration = 0.5f;
    public float targetScale = 30f;

    bool isRunning = false;

    void Start()
    {
        paw.gameObject.SetActive(false);
    }

    public void StartTransition()
    {
        if (isRunning) return;
        StartCoroutine(Zoom());
    }

    IEnumerator Zoom()
    {
        isRunning = true;

        paw.gameObject.SetActive(true);
        paw.localScale = Vector3.one;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = t / duration;
            float curve = progress * progress * progress;

            float scale = Mathf.Lerp(1f, targetScale, curve);

            paw.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        paw.localScale = Vector3.one * targetScale;
    }
}