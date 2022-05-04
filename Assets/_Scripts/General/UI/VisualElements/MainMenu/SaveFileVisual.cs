using UnityEngine;
using UnityEngine.UIElements;

public class SaveFileVisual : VisualElement
{
    string _fileName;

    public SaveFileVisual(string fileName)
    {
        _fileName = fileName;

        AddToClassList("uiContainer");
        //Add(new Label(fileName));
        // some info: player name, obols.
        // how to get info from that file?

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            VisualElement container = new VisualElement();
            Label playerNameLabel = new Label("Player Name: ");
            Label playerName = new Label(sd.PlayerName);

            playerNameLabel.AddToClassList("primaryText");
            playerName.AddToClassList("primaryText");

            container.Add(playerNameLabel);
            container.Add(playerName);

            Add(container);
        }

        this.AddManipulator(new Clickable(evt => GameManager.Instance.StartGameFromSave(fileName)));

        // button to destory the file
        Button deleteButton = new Button();
        deleteButton.text = "Remove Save";
        deleteButton.AddToClassList("primaryText");
        deleteButton.clickable.clicked += DeleteSave;
        Add(deleteButton);
    }

    void DeleteSave()
    {
        FileManager.DeleteFile(_fileName);
        // gray out the save or something.
        style.backgroundColor = Color.gray;
    }


}
