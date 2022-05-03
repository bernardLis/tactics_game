using UnityEngine;
using UnityEngine.UIElements;

public class SaveFileVisual : VisualElement
{
    public SaveFileVisual(string fileName)
    {
        Add(new Label(fileName));
        // some info: player name, obols.
        // how to get info from that file?

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            Add(new Label(sd.PlayerName));
        }

        // button to start game from that file
        this.AddManipulator(new Clickable(evt => GameManager.Instance.StartGameFromSave(fileName)));
    }

    // button to destory the file

}
