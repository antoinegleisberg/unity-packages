using System.Collections;
using UnityEngine;

namespace antoinegleisberg.Animation
{
    public static class Fading
    {
        public static IEnumerator FadeOut(this SpriteRenderer spriteRenderer, float duration)
        {
            Color color = spriteRenderer.color;
            float startAlpha = color.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
                spriteRenderer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        public static IEnumerator FadeIn(this SpriteRenderer spriteRenderer, float duration)
        {
            Color color = spriteRenderer.color;
            float startAlpha = color.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
                spriteRenderer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
