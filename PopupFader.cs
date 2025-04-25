using System.Collections;
using UnityEngine;

public class PopupFader : MonoBehaviour
{
    public CanvasGroup targetGroup;

    public void FadeIn()
    {
        targetGroup.alpha = 0f;
        targetGroup.interactable = true;
        targetGroup.blockRaycasts = true;
        gameObject.SetActive(true);
        StopAllCoroutines();
    }

    public void FadeOut()
    {
        StopAllCoroutines();
    }

    pribvate IEnumerator FadeCanvas(bool show)
    {
        float startAlpha = targetGroup.alpha;
        float endAlpha = show ? 1f : 0f;
        float elapsed = 0f;

        targetGroup.interactable = show;
        targetGroup.blockRaycasts = show;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            targetGroup.alpha = newAlpha;
            yield return null;
        }

        targetGroup.alpha = endAlpha;

    }
}