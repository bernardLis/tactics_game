using UnityEngine;
using UnityEngine.UIElements;

public class LoadGameScreenVisual : FullScreenVisual
{
    public LoadGameScreenVisual(VisualElement root, string[] saveFiles)
    {
        Initialize(root);
        AddToClassList("menuScreen");
        ScrollView scrollView = new ScrollView();
        scrollView.style.width = Length.Percent(100);
        Add(scrollView);

        foreach (string f in saveFiles)
        {
            SaveFileVisual sfv = new SaveFileVisual(scrollView, f);
            sfv.OnClick += Hide; // TODO: do I need to unsubscribe from it?
            scrollView.Add(sfv);
        }

        AddBackButton();
    }
}
