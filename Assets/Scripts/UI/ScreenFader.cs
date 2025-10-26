using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;
        while (elapsed < fadeDuration)
        {
            c.a = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            fadePanel.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        c.a = 1;
        fadePanel.color = c;
    }

    public IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;
        while (elapsed < fadeDuration)
        {
            c.a = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            fadePanel.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        c.a = 0;
        fadePanel.color = c;
    }
}