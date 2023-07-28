using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;
using DG.Tweening;

public static class Helpers
{
    const string _ussCommonTextPrimary = "common__text-primary";

    public static Gradient GetGradient(Color color)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.5f;
        colorKey[1].color = Color.white;
        colorKey[1].time = 1f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.5f;
        alphaKey[1].time = 1f;

        gradient.SetKeys(colorKey, alphaKey);

        return gradient;
    }


    public static string ParseScriptableObjectName(string text)
    {
        text = text.Replace("(Clone)", "");
        // https://stackoverflow.com/questions/3216085/split-a-pascalcase-string-into-separate-words
        Regex r = new Regex(
            @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
          );

        text = r.Replace(text, " "); // pascal case
        text = Regex.Replace(text, @"\s+", " "); // whitespace clean-up
        return text;
    }

    /* UI toolkit */
    public static void DisplayTextOnElement(VisualElement root, VisualElement element, string text, Color color)
    {
        if (root == null)
            root = GameManager.Instance.Root; 

        Label l = new Label(text);
        l.AddToClassList(_ussCommonTextPrimary);
        l.style.color = color;

        Vector3 start = new Vector3(element.worldBound.xMin, element.worldBound.yMin, 0);
        Vector3 end = new Vector3(element.worldBound.xMin + Random.Range(-100, 100),
                element.worldBound.yMin - 100, 0);
        ArcMovementElement arcMovementElement = new(l, start, end);
        root.Add(arcMovementElement);
        arcMovementElement.OnArcMovementFinished += () =>
        {
            DOTween.To(x => l.style.opacity = x, 1, 0, 1)
                    .SetUpdate(true)
                    .OnComplete(() => root.Remove(arcMovementElement));
        };
    }
    /*
        public static VisualElement GetRoot(VisualElement el)
        {
            if (el.parent == null)
                return el;
            return GetRoot(el.parent);
        }
        */

    public static int GetRandomNumber(int digits)
    {
        int min = (int)Mathf.Pow(10, digits - 1);
        int max = (int)Mathf.Pow(10, digits) - 1;
        return Random.Range(min, max);
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}