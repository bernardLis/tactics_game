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

    List<ArmySlotElement> _castleArmySlots = new();

    public event Action OnSetUpFinished;
    public CastleElement(VisualElement root, Castle castle, MapHero hero)
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
        AddVisitingHero();

        AddBackButton();
        Debug.Log($"finished should call");
        OnSetUpFinished?.Invoke();
    }

    void AddBuildings()
    {
        foreach (Building b in _castle.Buildings)
        {
            if (b.GetType() == typeof(ProductionBuilding))
            {
                ProductionBuilding pb = (ProductionBuilding)b;
                ProductionBuildingElement el = new(pb);
                pb.OnArmyBought += AddArmy;
                _topContainer.Add(el);
                continue;
            }

            BuildingElement e = new(b);
            _topContainer.Add(e);
        }
    }

    void AddCastleArmySlots()
    {
        _castleArmySlots = new();
        for (int i = 0; i < Castle.MaxCastleArmySlots; i++)
            AddArmySlot();
    }

    ArmySlotElement AddArmySlot()
    {
        ArmySlotElement el = new();


        el.OnArmyAdded += (ArmyElement el) => _castle.AddArmy(el.ArmyGroup);
        el.OnArmyAdded += (ArmyElement el) => _castle.RemoveArmy(el.ArmyGroup);

        _castleArmySlots.Add(el);
        _middleContainer.Add(el);

        return el;
    }

    void AddCastleArmy()
    {
        foreach (ArmyGroupData agd in _castle.AvailableArmy)
        {
            ArmyGroup a = ScriptableObject.CreateInstance<ArmyGroup>();
            a.LoadFromData(agd);

            foreach (ArmySlotElement el in _castleArmySlots)
            {
                if (el.ArmyElement == null)
                {
                    el.AddArmyNoDelegates(new(a));
                    return;
                }
            }

            ArmySlotElement armySlotElement = AddArmySlot();
            armySlotElement.AddArmyNoDelegates(new(a));
        }
    }


    void AddArmy(ArmyGroup armyGroup)
    {
        // stack
        foreach (ArmySlotElement el in _castleArmySlots)
        {
            if (el.ArmyElement == null) continue;

            if (el.ArmyElement.ArmyGroup.ArmyEntity == armyGroup.ArmyEntity)
            {
                el.ArmyElement.ArmyGroup.ChangeCount(armyGroup.EntityCount);
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

        ArmySlotElement armySlotElement = AddArmySlot();
        armySlotElement.AddArmy(new(armyGroup));
    }


    void AddVisitingHero()
    {
        if (_visitingHero == null) return;
        _bottomContainer.Add(new HeroCastleElement(_visitingHero.Character));
    }

    public override void Hide()
    {
        base.Hide();
        _gameManager.ToggleTimer(true);
    }
}
