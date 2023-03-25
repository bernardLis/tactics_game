using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BuildingElement : ElementWithTooltip
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "building__";
    const string _ussMain = _ussClassName + "main";
    const string _ussSprite = _ussClassName + "sprite";
    const string _ussBuildButton = _ussClassName + "build-button";
    const string _ussHeader = _ussClassName + "header";

    protected GameManager _gameManager;
    protected Building _building;

    VisualElement _sprite;
    VisualElement _buildButtonContainer;
    MyButton _buildButton;
    GoldElement _costGoldElement;

    VisualElement _tooltipElement;

    public BuildingElement(Building building)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BuildingElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _gameManager.OnGoldChanged += OnGoldChanged;

        _building = building;

        AddToClassList(_ussMain);

        _sprite = new();
        _sprite.AddToClassList(_ussSprite);
        Add(_sprite);
        UpdateBuildingSprite();

        _buildButtonContainer = new();
        Add(_buildButtonContainer);
        HandleBuildButton();

        CreateTooltip();

        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);
    }

    void OnGoldChanged(int change)
    {
        if (_buildButton == null) return;
        _buildButton.SetEnabled(false);
        if (_gameManager.Gold >= _building.CostToBuild)
            _buildButton.SetEnabled(true);
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        _gameManager.OnGoldChanged -= OnGoldChanged;
    }

    void UpdateBuildingSprite()
    {
        _sprite.style.backgroundImage = _building.IsBuilt ? new StyleBackground(_building.BuiltSprite)
                : new StyleBackground(_building.OutlineSprite);
    }

    void HandleBuildButton()
    {
        if (_building.IsBuilt) return;

        _buildButton = new(null, _ussBuildButton, Build);
        _costGoldElement = new GoldElement(_building.CostToBuild);
        _buildButton.Add(_costGoldElement);
        _buildButton.SetEnabled(false);

        _buildButtonContainer.Add(_buildButton);

        if (_gameManager.Gold >= _building.CostToBuild)
            _buildButton.SetEnabled(true);
    }

    void Build()
    {
        _gameManager.ChangeGoldValue(-_building.CostToBuild);
        _building.Build();

        _buildButton.SetEnabled(false);
        DOTween.To(x => _buildButtonContainer.transform.scale = x * Vector3.one, 1, 0, 1f)
                .SetEase(Ease.InCirc)
                .OnComplete(() => _buildButtonContainer.style.display = DisplayStyle.None);

        VisualElement spriteMask = new();
        spriteMask.style.width = Length.Percent(100);
        spriteMask.style.height = Length.Percent(100);
        spriteMask.style.backgroundImage = new StyleBackground(_building.BuiltSprite);
        spriteMask.style.opacity = 0;

        _sprite.Add(spriteMask);
        DOTween.To(x => spriteMask.style.opacity = x, 0, 1, 1f)
            .SetEase(Ease.InCirc);
    }

    void CreateTooltip()
    {
        _tooltipElement = new();

        Label header = new($"{_building.DisplayName}");
        header.AddToClassList(_ussHeader);
        _tooltipElement.Add(header);

        Label description = new(_building.GetDescription());
        _tooltipElement.Add(description);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipElement);
        base.DisplayTooltip();
    }
}
