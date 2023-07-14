using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseLivesElement : VisualElement
{
    BaseUpgradeLives _upgrade;

    List<UpgradeLevelElement> _maxLivesTree = new();

    public BaseLivesElement(BaseUpgradeLives upgrade)
    {
        _upgrade = upgrade;

        style.flexDirection = FlexDirection.Row;

        Label lives = new($"Lives: {_upgrade.CurrentLives.Value}");
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
        UpgradeLevelElement restoreLives = new(_upgrade.RestoreLivesTree);
        Add(restoreLives);

        if (_upgrade.CurrentLives.Value == _upgrade.MaxLivesTree[_upgrade.CurrentMaxLivesLevel].Value)
            restoreLives.SetEnabled(false);

        restoreLives.OnPurchased += RestoreLives;
    }

    void RestoreLives()
    {
        _upgrade.RestoreLives(5);
    }

    void AddMaxLivesTree()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        Label title = new($"Max Lives {_upgrade.MaxLivesTree[_upgrade.CurrentMaxLivesLevel].Value}");
        container.Add(title);

        for (int i = 0; i < _upgrade.MaxLivesTree.Count; i++)
        {
            UpgradeLevelElement el = new(_upgrade.MaxLivesTree[i]);
            Add(el);
            _maxLivesTree.Add(el);

            if (i != _upgrade.CurrentMaxLivesLevel + 1)
                el.SetEnabled(false);

            el.OnPurchased += () =>
            {
                title.text = $"Max Lives {_upgrade.MaxLivesTree[i].Value}";
                _upgrade.CurrentMaxLivesLevel = i;
                if (i != _upgrade.MaxLivesTree.Count - 1)
                    _maxLivesTree[i + 1].SetEnabled(true);
            };
        }
    }


}
