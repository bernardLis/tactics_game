using UnityEngine;
using UnityEngine.UIElements;

public class LoadGameScreenVisual : FullScreenVisual
{
    public LoadGameScreenVisual(VisualElement root, string[] saveFiles)
    {
        Initialize(root);
        AddToClassList("menuScreen");

        foreach (string f in saveFiles)
            Add(new SaveFileVisual(this, f));

        AddBackButton();
    }
}
