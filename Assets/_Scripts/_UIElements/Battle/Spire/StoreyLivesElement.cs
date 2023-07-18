using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreyLivesElement : VisualElement
{
    const string _ussClassName = "storey-lives__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTitle = _ussClassName + "title";

    const string _ussArrowTop = _ussClassName + "arrow-top";
    const string _ussArrowBottom = _ussClassName + "arrow-bottom";

    const string _ussTreeContainer = _ussClassName + "tree-container";
    const string _ussArrowLabel = _ussClassName + "arrow-label";

    GameManager _gameManager;

    StoreyLives _storey;

    VisualElement _topContainer;
    VisualElement _bottomContainer;

    Label _lives;
    StoreyUpgradeElement _restoreLivesElement;

    List<StoreyUpgradeElement> _maxLivesTreeElements = new();

    public StoreyLivesElement(StoreyLives storey)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyLivesStyles);
        styleSheets.Add(ss);

        AddToClassList(_ussMain);
        _storey = storey;

        _lives = new($"Lives: {_storey.CurrentLives.Value}/{_storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value}");
        _lives.AddToClassList(_ussTitle);
        Add(_lives);

        VisualElement container = new();
        Add(container);

        _topContainer = new();
        _bottomContainer = new();
        _topContainer.AddToClassList(_ussTreeContainer);
        _bottomContainer.AddToClassList(_ussTreeContainer);
        container.Add(_topContainer);
        container.Add(_bottomContainer);

        Label topArrow = new("-------->");
        topArrow.AddToClassList(_ussArrowTop);
        _topContainer.Add(topArrow);
        Label bottomArrow = new("-------->");
        bottomArrow.AddToClassList(_ussArrowBottom);
        _bottomContainer.Add(bottomArrow);

        AddMaxLivesTree();
        AddRestoreLivesTree();
    }

    void AddRestoreLivesTree()
    {
        _restoreLivesElement = new(_storey.RestoreLivesTree);
        _bottomContainer.Add(_restoreLivesElement);

        _restoreLivesElement.OnPurchased += RestoreLives;

        UpdateRestoreLivesElement();
    }

    void RestoreLives(StoreyUpgrade storeyUpgrade)
    {
        _storey.RestoreLives(5);
        _lives.text = $"Lives: {_storey.CurrentLives.Value}/{_storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value}";
        UpdateRestoreLivesElement();
    }

    void UpdateRestoreLivesElement()
    {
        if (_storey.CurrentLives.Value >= _storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value)
            _restoreLivesElement.SetEnabled(false);
    }

    void AddMaxLivesTree()
    {
        for (int i = 0; i < _storey.MaxLivesTree.Count; i++)
        {
            if (i > 0)
            {
                Label arrow = new("--->");
                arrow.AddToClassList(_ussArrowLabel);
                _topContainer.Add(arrow);
            }

            StoreyUpgradeElement el = new(_storey.MaxLivesTree[i]);
            _topContainer.Add(el);
            _maxLivesTreeElements.Add(el);

            if (i != _storey.CurrentMaxLivesLevel + 1)
                el.SetEnabled(false);

            el.OnPurchased += MaxLivesUpgradePurchased;
        }
    }

    void MaxLivesUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        _maxLivesTreeElements[_storey.CurrentMaxLivesLevel + 1].SetEnabled(false);
        _storey.CurrentLives.ApplyChange(_storey.MaxLivesTree[_storey.CurrentMaxLivesLevel + 1].Value - _storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value);
        _storey.CurrentMaxLivesLevel++;

        _lives.text = $"Lives: {_storey.CurrentLives.Value}/{_storey.MaxLivesTree[_storey.CurrentMaxLivesLevel].Value}";

        if (_storey.CurrentMaxLivesLevel < _storey.MaxLivesTree.Count - 1)
            _maxLivesTreeElements[_storey.CurrentMaxLivesLevel + 1].SetEnabled(true);
    }
}
