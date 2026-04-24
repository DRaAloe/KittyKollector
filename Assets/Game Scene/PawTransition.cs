using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PawTransition : MonoBehaviour
{
    public RectTransform[] paws;
    public float speed = 2500f;
    public float delay = 0.05f;
    public string sceneName = "GameScene";

    public void PlayTransition()
    {
        StartCoroutine(Swipe());
    }

    IEnumerator Swipe()
    {
        for (int i = 0; i < paws.Length; i++)
        {
            StartCoroutine(MovePaw(paws[i], i * delay));
        }

        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator MovePaw(RectTransform paw, float wait)
    {
        yield return new WaitForSeconds(wait);

        Vector2 target = new Vector2(2000f, paw.anchoredPosition.y);

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