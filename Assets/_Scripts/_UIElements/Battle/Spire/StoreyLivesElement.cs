using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreyLivesElement : VisualElement
{
    StoreyLives _storey;

    StoreyUpgradeElement _restoreLivesElement;

    Label _maxLivesTreeTitle;
    List<StoreyUpgradeElement> _maxLivesTreeElements = new();

    public StoreyLivesElement(StoreyLives storey)
    {
        _storey = storey;

        style.flexDirection = FlexDirection.Row;

        Label lives = new($"Lives: {_storey.CurrentLives.Value}");
        Add(lives);

        VisualElement container = new();
        Add(container);
        container.Add(new Label("----->"));
        container.Add(new Label("----->"));

        AddMaxLivesTree();
        AddRestoreLivesTree();
    }

    void AddRestoreLivesTree()
    {
        _restoreLivesElement = new(_storey.RestoreLivesTree);
        Add(_restoreLivesElement);

        _restoreLivesElement.OnPurchased += RestoreLives;

        UpdateRestoreLivesElement();
    }

    void RestoreLives(StoreyUpgrade storeyUpgrade)
    {
        _storey.RestoreLives(5);
        UpdateRestoreLivesElement();
    }

    void UpdateRestoreLivesElement()
    {
        if (_storey.CurrentLives.Value >= _storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value)
            _restoreLivesElement.SetEnabled(false);
    }

    void AddMaxLivesTree()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        _maxLivesTreeTitle = new($"Max Lives {_storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value}");
        container.Add(_maxLivesTreeTitle);

        for (int i = 0; i < _storey.MaxLivesTree.Count; i++)
        {
            StoreyUpgradeElement el = new(_storey.MaxLivesTree[i]);
            Add(el);
            _maxLivesTreeElements.Add(el);

            if (i != _storey.CurrentMaxLivesLevel + 1)
                el.SetEnabled(false);

            el.OnPurchased += MaxLivesUpgradePurchased;
        }
    }

    void MaxLivesUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        _maxLivesTreeElements[_storey.CurrentMaxLivesLevel + 1].SetEnabled(false);
        // add difference between values of current and next level to current lives
        _storey.CurrentLives.ApplyChange(_storey.MaxLivesTree[_storey.CurrentMaxLivesLevel + 1].Value - _storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value);
        _storey.CurrentMaxLivesLevel++;

        if (_storey.CurrentMaxLivesLevel < _storey.MaxLivesTree.Count - 1)
            _maxLivesTreeElements[_storey.CurrentMaxLivesLevel + 1].SetEnabled(true);
    }
}
