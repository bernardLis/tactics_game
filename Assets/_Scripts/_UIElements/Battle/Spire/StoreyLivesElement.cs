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
    StoreyUpgradeTreeElement _maxLivesTreeElement;
    StoreyUpgradeElement _restoreLivesElement;

    //  List<StoreyUpgradeElement> _maxLivesTreeElements = new();

    public StoreyLivesElement(StoreyLives storey)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyLivesStyles);
        styleSheets.Add(ss);

        AddToClassList(_ussMain);
        _storey = storey;

        _lives = new($"Lives {_storey.CurrentLives.Value}/{_storey.MaxLivesTree.CurrentValue.Value}");
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
        
        _maxLivesTreeElement = new(_storey.MaxLivesTree);
        _topContainer.Add(_maxLivesTreeElement);
        _storey.MaxLivesTree.CurrentValue.OnValueChanged += UpdateLivesTitle;
        _storey.MaxLivesTree.CurrentValue.OnValueChanged += (v) => UpdateRestoreLivesElement();

        UpdateLivesTitle(0);
        AddRestoreLivesUpgrade();
    }

    void AddRestoreLivesUpgrade()
    {
        _restoreLivesElement = new(_storey.RestoreLivesUpgrade);
        _bottomContainer.Add(_restoreLivesElement);

        _restoreLivesElement.OnPurchased += RestoreLives;

        UpdateRestoreLivesElement();
    }

    void RestoreLives(StoreyUpgrade storeyUpgrade)
    {
        _storey.RestoreLives(5);
        UpdateLivesTitle(0);
        UpdateRestoreLivesElement();
    }

    void UpdateLivesTitle(int value)
    {
        _lives.text = $"Lives: {_storey.CurrentLives.Value}/{_storey.MaxLivesTree.CurrentValue.Value}";
    }

    void UpdateRestoreLivesElement()
    {
        _restoreLivesElement.SetEnabled(true);

        if (_storey.CurrentLives.Value >= _storey.MaxLivesTree.CurrentValue.Value)
            _restoreLivesElement.SetEnabled(false);
    }
}
