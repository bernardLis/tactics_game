using System;
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
    MapHero _visitingHero;

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;

    List<BuildingElement> _buildingElements = new();
    List<ArmySlotElement> _castleArmySlots = new();

    public CastleElement(VisualElement root, Castle castle, MapHero hero = null)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CastleElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);
        _gameManager.ToggleTimer(false);

        _castle = castle;
        _visitingHero = hero;

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
        AddCastleArmySlots();
        AddCastleArmy();
        if (_visitingHero != null)
            AddVisitingHero();

        AddBackButton();

        RegisterCallback<DetachFromPanelEvent>(OnDetach);
    }

    void AddBuildings()
    {
        foreach (Building b in _castle.Buildings)
        {
            b.OnBuilt += UpdateBuildings;

            if (b.GetType() == typeof(ProductionBuilding))
            {
                ProductionBuilding pb = (ProductionBuilding)b;
                ProductionBuildingElement el = new(_castle, pb);
                pb.OnArmyBought += AddArmy;
                _topContainer.Add(el);
                _buildingElements.Add(el);
                continue;
            }

            BuildingElement e = new(_castle, b);
            _buildingElements.Add(e);
            _topContainer.Add(e);
        }
    }

    void OnDetach(DetachFromPanelEvent e)
    {
        foreach (Building b in _castle.Buildings)
        {
            b.OnBuilt -= UpdateBuildings;

            if (b.GetType() == typeof(ProductionBuilding))
            {
                ProductionBuilding pb = (ProductionBuilding)b;
                pb.OnArmyBought -= AddArmy;
                continue;
            }
        }
    }

    void UpdateBuildings()
    {
        foreach (BuildingElement el in _buildingElements)
            el.UpdateBuildButton();
    }

    void AddCastleArmySlots()
    {
        _castleArmySlots = new();
        for (int i = 0; i < Castle.MaxCastleArmySlots; i++)
            AddArmySlot(i);
    }

    ArmySlotElement AddArmySlot(int i)
    {
        ArmySlotElement el = new(null, i);

        el.OnArmyAdded += (ArmyGroupElement el) => _castle.AddArmy(el.ArmyGroup);
        el.OnArmyRemoved += (ArmyGroupElement el) => _castle.RemoveArmy(el.ArmyGroup);

        _castleArmySlots.Add(el);
        _middleContainer.Add(el);

        return el;
    }

    void AddCastleArmy()
    {
        foreach (ArmyGroup ag in _castle.Army)
            _castleArmySlots[ag.ListPosition].AddArmyNoDelegates(new(ag));
    }

    void AddArmy(ArmyGroup armyGroup)
    {
        // stack
        foreach (ArmySlotElement el in _castleArmySlots)
        {
            if (el.ArmyElement == null) continue;

            if (el.ArmyElement.ArmyGroup.ArmyEntity == armyGroup.ArmyEntity)
            {
                el.AddArmy(new(armyGroup));
                return;
            }
        }

        // there is a free slot
        foreach (ArmySlotElement el in _castleArmySlots)
        {
            if (el.ArmyElement == null)
            {
                el.AddArmy(new(armyGroup));
                return;
            }
        }

        Debug.LogError($"No free slots for army in castle {_castle.name}");
    }


    void AddVisitingHero()
    {
        if (_visitingHero == null) return;
        _bottomContainer.Add(new HeroArmyElement(_visitingHero.Hero));
    }

    public override void Hide()
    {
        base.Hide();
        _gameManager.ToggleTimer(true);
    }
}
