using UnityEngine;
using UnityEngine.UIElements;
using System;

public class SaveFileVisual : VisualElement
{
    string _fileName;
    VisualElement _parent;

    public event Action OnClick;
    public SaveFileVisual(VisualElement parentVisualEl, string fileName)
    {
        _fileName = fileName;
        _parent = parentVisualEl;

        AddToClassList("uiContainer");
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;
        style.justifyContent = Justify.SpaceBetween;

        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            Label playerName = new Label(sd.PlayerName);
            playerName.AddToClassList("primaryText");
            Add(playerName);

            VisualElement portraitContainer = new VisualElement();
            portraitContainer.style.flexDirection = FlexDirection.Row;
            Add(portraitContainer);

            CharacterDatabase database = GameManager.Instance.CharacterDatabase;
            foreach (CharacterData cd in sd.Characters)
            {
                Label portrait = new Label();
                portrait.style.backgroundImage = database.GetPortraitByID(cd.Portrait).texture;
                portrait.AddToClassList("characterCardPortrait");
                portraitContainer.Add(portrait);
            }
        }

        this.AddManipulator(new Clickable(StartGame));

        // button to destory the file
        Button deleteButton = new Button();
        deleteButton.text = "Remove Save";
        deleteButton.AddToClassList("menuButton");
        deleteButton.clickable.clicked += ConfirmDelete;
        Add(deleteButton);
    }

    void StartGame()
    {
        GameManager.Instance.StartGameFromSave(_fileName);
        OnClick?.Invoke();
    }

    void ConfirmDelete()
    {
        ConfirmPopUp pop = new ConfirmPopUp();
        pop.Initialize(_parent, DeleteSave);
    }

    void DeleteSave()
    {
        FileManager.DeleteFile(_fileName);
        _parent.Remove(this);
    }


}
