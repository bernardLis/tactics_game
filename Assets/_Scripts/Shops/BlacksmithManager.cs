using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlacksmithManager : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;
    VisualElement _blacksmithItemContainer;

    bool _wasVisited;
    void Awake()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _blacksmithItemContainer = _root.Q<VisualElement>("blacksmithItemContainer");

        Button blacksmithRerollButton = _root.Q<Button>("blacksmithRerollButton");
        blacksmithRerollButton.clickable.clicked += PopulateItems;

        Button blacksmithBackButton = _root.Q<Button>("blacksmithBackButton");
        blacksmithBackButton.clickable.clicked += Back;

    }

    public void Initialize()
    {
        if (!_wasVisited)
            PopulateItems();

        _wasVisited = true;
    }

    void PopulateItems()
    {
        _blacksmithItemContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            VisualElement container = new();
            Item item = _gameManager.CharacterDatabase.GetRandomItem();
            ItemVisual iv = new(item);
            container.Add(iv);
            container.Add(new Label("Price: " + item.Price));

            _blacksmithItemContainer.Add(container);
        }
    }

    void Back()
    {
        ShopManager.Instance.ShowShops();
    }
}
