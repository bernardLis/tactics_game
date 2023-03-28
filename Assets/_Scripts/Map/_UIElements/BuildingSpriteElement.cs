using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSpriteElement : ElementWithTooltip
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "building-sprite__";
    const string _ussMain = _ussClassName + "main";
    const string _ussHeader = _ussClassName + "header";

    GameManager _gameManager;

    Building _building;

    VisualElement _tooltipElement;

    public BuildingSpriteElement(Building building)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BuildingSpriteElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _building = building;

        AddToClassList(_ussMain);

        UpdateBuildingSprite();
        CreateTooltip();
    }

    void UpdateBuildingSprite()
    {
        style.backgroundImage = _building.IsBuilt ? new StyleBackground(_building.BuiltSprite)
                : new StyleBackground(_building.OutlineSprite);
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
