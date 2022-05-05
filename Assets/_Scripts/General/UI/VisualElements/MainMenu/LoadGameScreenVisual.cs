using UnityEngine;
using UnityEngine.UIElements;

public class LoadGameScreenVisual : FullScreenVisual
{

    // back button on click destory self
    // save files know what file they are and on click they load game from file
    public LoadGameScreenVisual(VisualElement root, string[] saveFiles)
    {
        Initialize(root);

        foreach (string f in saveFiles)
            Add(new SaveFileVisual(this, f));

        Button backButton = new Button();
        backButton.AddToClassList("menuButton");
        backButton.clickable.clicked += Hide;
    }

}
