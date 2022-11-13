using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;

public static class Helpers
{
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

        { ItemRaririty.Common.ToString(), new Color(1f,1f,1f,1f) },
        { ItemRaririty.Magic.ToString(), new Color(0.31f,1f,0.69f,1f) },
        { ItemRaririty.Rare.ToString(), new Color(0.38f,0.51f,0.84f,1f) },
        { ItemRaririty.Epic.ToString(), new Color(0.32f,0.22f,0.44f,1f) },
    };

    static Camera _camera;
    //https://www.youtube.com/watch?v=JOABOQMurZo
    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static int GetManhattanDistance(Vector2 start, Vector2 end)
    {
        return Mathf.RoundToInt(Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y));
    }

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

    public static Vector2 GetDirectionToClosestWithTag(GameObject self, string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        if (targets.Length == 0)
            return Vector2.zero;

        float distanceToClosestTarget = Vector3.Distance(self.transform.position, targets[0].transform.position);
        GameObject closestTarget = targets[0];

        foreach (GameObject target in targets)
        {
            float dist = Vector3.Distance(self.transform.position, target.transform.position);
            if (dist < distanceToClosestTarget)
                closestTarget = target;
        }

        return (closestTarget.transform.position - self.transform.position).normalized;
    }

    public static List<GameObject> FindGameObjectsWithInterface<T>()
    {
        List<GameObject> objectsWithInterfaces = new();
        GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); // TODO: I am worried this lags unity.

        foreach (GameObject g in gameObjects)
        {
            if (!g.activeInHierarchy)
                continue;

            T[] childrenInterfaces = g.GetComponentsInChildren<T>();
            if (childrenInterfaces.Length > 0)
                objectsWithInterfaces.Add(g);
        }
        return objectsWithInterfaces;
    }

    public static async void DisplayTextOnElement(VisualElement root, VisualElement element, string text, Color color)
    {
        Label l = new Label(text);
        l.AddToClassList("textSecondary");
        l.style.color = color;
        l.style.position = Position.Absolute;
        l.style.left = element.worldBound.xMin - element.worldBound.width / 2;
        l.style.top = element.worldBound.yMax;

        root.Add(l);
        float end = element.worldBound.yMin + 100;
        await DOTween.To(x => l.style.top = x, element.worldBound.yMax, end, 1).SetEase(Ease.OutSine).AsyncWaitForCompletion();
        await DOTween.To(x => l.style.opacity = x, 1, 0, 1).AsyncWaitForCompletion();
        root.Remove(l);
    }


}
