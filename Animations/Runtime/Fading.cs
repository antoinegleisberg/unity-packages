using System.Collections;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Animation
{
    public static class Fading
    {
        public static IEnumerator FadeGameObjectAndChildren(this Transform transform, float duration, MonoBehaviour coroutineTarget, float startAlpha, float endAlpha)
        {
            List<Coroutine> coroutines = new List<Coroutine>();

            foreach (Transform child in transform)
            {
                coroutines.Add(coroutineTarget.StartCoroutine(FadeGameObjectAndChildren(child, duration, coroutineTarget, startAlpha, endAlpha)));
            }

            foreach (Component component in transform.GetComponents<Component>())
            {
                if (HasColorField(component))
                {
                    coroutines.Add(coroutineTarget.StartCoroutine(Fade(component, duration, startAlpha, endAlpha)));
                }
            }

            foreach (Coroutine coroutine in coroutines)
            {
                yield return coroutine;
            }
        }

        public static IEnumerator FadeOutGameObjectAndChildren(this Transform transform, float duration, MonoBehaviour coroutineTarget)
        {
            return transform.FadeGameObjectAndChildren(duration, coroutineTarget, 1f, 0f);
        }

        public static IEnumerator FadeInGameObjectAndChildren(this Transform transform, float duration, MonoBehaviour coroutineTarget)
        {
            return transform.FadeGameObjectAndChildren(duration, coroutineTarget, 0f, 1f);
        }

        public static IEnumerator Fade<T>(this T component, float duration, float startAlpha, float endAlpha) where T : Component
        {
            if (!HasColorField(component))
            {
                throw new ArgumentException("This function only accepts types that have a 'color' field");
            }

            Color color = GetColor(component);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                SetColor(component, color);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            color.a = endAlpha;
            SetColor(component, color);
        }

        public static IEnumerator FadeOut<T>(this T component, float duration) where T : Component
        {
            return Fade(component, duration, 1f, 0f);
        }

        public static IEnumerator FadeIn<T>(this T component, float duration) where T : Component
        {
            return Fade(component, duration, 0f, 1f);
        }

        private static Color GetColor<T>(T component) where T : Component
        {
            Type type = component.GetType();

            // Check if the type has a property named "color"
            PropertyInfo property = type.GetProperty("color", BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(Color) && property.CanWrite)
            {
                return (Color)property.GetValue(component);
            }

            // Check if the type has a field named "color"
            FieldInfo field = type.GetField("color", BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(Color))
            {
                return (Color)field.GetValue(component);
            }

            throw new ArgumentException("This function only accepts types that have a 'color' field");
        }

        private static void SetColor<T>(T component, Color color) where T : Component
        {
            Type type = component.GetType();

            // Check if the type has a property named "color"
            PropertyInfo property = type.GetProperty("color", BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(Color) && property.CanWrite)
            {
                property.SetValue(component, color);
            }

            // Check if the type has a field named "color"
            FieldInfo field = type.GetField("color", BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(Color))
            {
                field.SetValue(component, color);
            }
        }

        private static bool HasColorField<T>(T component) where T : Component
        {
            Type type = component.GetType();

            // Check if the type has a property named "color"
            PropertyInfo property = type.GetProperty("color", BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(Color))
            {
                return true;
            }

            // Check if the type has a field named "color"
            FieldInfo field = type.GetField("color", BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(Color))
            {
                return true;
            }

            return false;
        }
    }
}
