using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CastleElement : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "castle__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopContainer = _ussClassName + "top-container";
    const string _ussMiddleContainer = _ussClassName + "middle-container";
    const string _ussBottomContainer = _ussClassName + "bottom-container";

    GameManager _gameManager;
    Castle _castle;

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;
    public CastleElement(VisualElement root, Castle castle)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CastleElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);

        _castle = castle;

        AddToClassList(_ussMain);

        _topContainer = new();
        _topContainer.AddToClassList(_ussTopContainer);
        Add(_topContainer);

        _middleContainer = new();
        _middleContainer.AddToClassList(_ussMiddleContainer);
        Add(_middleContainer);

        _bottomContainer = new();
        _bottomContainer.AddToClassList(_ussBottomContainer);
        Add(_bottomContainer);

        AddBuildings();
        AddBackButton();
    }

    void AddBuildings()
    {
        foreach (Building b in _castle.Buildings)
        {
            if (b.GetType() == typeof(ProductionBuilding))
            {
                ProductionBuildingElement el = new((ProductionBuilding)b);
                _topContainer.Add(el);
                continue;
            }

            BuildingElement e = new(b);
            _topContainer.Add(e);
        }
    }

    public override void Hide()
    {
        base.Hide();
        _gameManager.ToggleTimer(true);
    }
}
