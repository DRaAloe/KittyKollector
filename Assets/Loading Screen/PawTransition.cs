using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPawTransition : MonoBehaviour
{
    public RectTransform[] paws;
    public float speed = 2000f;
    public float delayBetweenPaws = 0.1f;
    public string sceneName = "GameScene";

    public void PlayTransition()
    {
        StartCoroutine(PlayWave());
    }

    IEnumerator PlayWave()
    {
        for (int i = 0; i < paws.Length; i++)
        {
            StartCoroutine(MovePaw(paws[i], i * delayBetweenPaws));
        }

        // Wait for all paws to finish animating (last paw delay + estimated animation time)
        float lastPawDelay = (paws.Length - 1) * delayBetweenPaws;
        float estimatedAnimationTime = 2f; // Adjust based on your animation speed
        yield return new WaitForSeconds(lastPawDelay + estimatedAnimationTime);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator MovePaw(RectTransform paw, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector2 target = new Vector2(1500f, paw.anchoredPosition.y);

        while (Vector2.Distance(paw.anchoredPosition, target) > 10f)
        {
            paw.anchoredPosition = Vector2.MoveTowards(
                paw.anchoredPosition,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }
    }
}