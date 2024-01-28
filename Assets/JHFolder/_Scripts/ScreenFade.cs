using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeSquare;
    public float fadeInTime = 2;
    public float fadeOutTime = 2;

    public bool fadeOutOnStart = false;

    private void Start()
    {
        if (fadeOutOnStart)
        {
            fadeSquare.color = new Color(fadeSquare.color.r, fadeSquare.color.g, fadeSquare.color.b, 1);
            FadeOut();
        }
    }

    public void FadeIn()
    {
        StartCoroutine(ImageFade(fadeSquare, 1, 2));
    }

    public void FadeOut()
    {
        StartCoroutine(ImageFade(fadeSquare, 0, 2));
    }

    public IEnumerator ImageFade(Image image, float endValue, float duration)
    {
        float elapsedTime = 0;
        float startValue = image.color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
    }
}
