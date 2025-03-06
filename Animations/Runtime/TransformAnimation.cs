using System;
using System.Collections;
using UnityEngine;

namespace antoinegleisberg.Animation
{
    public static class TransformAnimation
    {
        public static IEnumerator Slide(this RectTransform rt, Vector3 start, Vector3 end, float duration)
        {
            rt.anchoredPosition = start;
            
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                Vector3 position = Vector3.Lerp(start, end, elapsedTime / duration);
                rt.anchoredPosition = position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rt.anchoredPosition = end;
        }
    }
}
