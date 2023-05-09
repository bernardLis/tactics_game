using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;
using DG.Tweening;

public static class Helpers
{
    const string _ussCommonTextPrimary = "common__text-primary";

    static Dictionary<string, Color> _colors = new()
    {
        { "healthBarRed", new Color(0.529f, 0.16f, 0.16f) },
        { "manaBarBlue", new Color(0.168f, 0.149f, 0.85f) },
        { "movementBlue", new Color(0.53f, 0.52f, 1f, 1f) },
        { "healthGainGreen", new Color(0.42f, 1f, 0.42f, 1f) },
        { "damageRed", new Color(1f, 0.42f, 0.42f, 1f) },
        { "gray", new Color(0.22f, 0.22f, 0.22f, 1f) },

        {QuestState.Pending.ToString(), new Color(0.27f, 0.4f, 0.56f)},
        {QuestState.Delegated.ToString(), new Color(0.55f, 0.7f, 0.21f)},
        {QuestState.Finished.ToString(), new Color(0.18f, 0.2f, 0.21f)},
        {QuestState.Expired.ToString(), new Color(0.55f, 0.2f, 0.21f)},
        {QuestState.RewardCollected.ToString(), new Color(0.55f, 0.2f, 0.7f)},

        { ItemRarity.Common.ToString(), new Color(1f,1f,1f,1f) },
        { ItemRarity.Uncommon.ToString(), new Color(0.31f,1f,0.69f,1f) },
        { ItemRarity.Rare.ToString(), new Color(0.38f,0.51f,0.84f,1f) },
        { ItemRarity.Epic.ToString(), new Color(0.32f,0.22f,0.44f,1f) },
    };

    public static Color GetColor(string name)
    {
        Color col;
        if (!_colors.TryGetValue(name, out col))
        {
            Debug.LogError($"Color: {name} is not in the color dictionary");
            return Color.black;
        }
        return col;
    }

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


    public static string ParseScriptableObjectCloneName(string text)
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
            root = GetRoot(element);
        Label l = new Label(text);
        l.AddToClassList(_ussCommonTextPrimary);
        l.style.color = color;
        l.style.position = Position.Absolute;
        l.style.left = element.worldBound.xMin;
        l.style.top = element.worldBound.yMin;

        root.Add(l);
        float end = element.worldBound.yMin - 100;
        DOTween.To(x => l.style.top = x, element.worldBound.yMin, end, 1).SetEase(Ease.OutSine)
                .OnComplete(() => DOTween.To(x => l.style.opacity = x, 1, 0, 1)
                .OnComplete(() => root.Remove(l)));
    }

    public static VisualElement GetRoot(VisualElement el)
    {
        if (el.parent == null)
            return el;
        return GetRoot(el.parent);
    }
}
