using UnityEngine;
using UnityEngine.UIElements;

public class SaveFileVisual : VisualElement
{
    string _fileName;
    LoadGameScreenVisual _parent;

    public SaveFileVisual(LoadGameScreenVisual parentVisualEl, string fileName)
    {
        _fileName = fileName;
        _parent = parentVisualEl;

        AddToClassList("uiContainer");

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

        this.AddManipulator(new Clickable(StartGame));

        // button to destory the file
        Button deleteButton = new Button();
        deleteButton.text = "Remove Save";
        deleteButton.AddToClassList("primaryText");
        deleteButton.clickable.clicked += ConfirmDelete;
        Add(deleteButton);
    }

    void StartGame()
    {
        GameManager.Instance.StartGameFromSave(_fileName);
        _parent.Hide();
    }

    void ConfirmDelete()
    {
        ConfirmPopUp pop = new ConfirmPopUp();
        pop.Initialize(_parent, DeleteSave);
    }

    void DeleteSave()
    {
        FileManager.DeleteFile(_fileName);
        // gray out the save or something.
        //style.backgroundColor = Color.gray;
        _parent.Remove(this);
    }


}
