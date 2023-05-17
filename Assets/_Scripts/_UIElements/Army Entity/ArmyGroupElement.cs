using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class ArmyGroupElement : ElementWithTooltip
{
    const string _ussClassName = "army-group__";
    const string _ussMain = _ussClassName + "main";
    const string _ussCount = _ussClassName + "count";

    GameManager _gameManager;

    public EntityIcon EntityIcon;
    Label _armyCountLabel;

    public bool IsLocked { get; private set; }

    public ArmyGroup ArmyGroup;

    public event Action OnEvolutionFinished;
    public ArmyGroupElement(ArmyGroup armyGroup, bool isLocked = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyGroupElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        ArmyGroup = armyGroup;
        armyGroup.OnEvolved += OnEvolved;
        armyGroup.OnCountChanged += OnCountChanged;

        IsLocked = isLocked;

        AddToClassList(_ussMain);

        EntityIcon = new(armyGroup.ArmyEntity);
        Add(EntityIcon);

        _armyCountLabel = new($"{armyGroup.EntityCount}");
        _armyCountLabel.AddToClassList(_ussCount);
        Add(_armyCountLabel);
    }

    public void LargeIcon()
    {
        style.width = 200;
        style.height = 200;

        EntityIcon.LargeIcon();
    }

    void OnEvolved(ArmyEntity armyEntity)
    {
        DOTween.To(x => EntityIcon.style.opacity = x, 1, 0, 0.5f);

        EntityIcon.SwapEntity(armyEntity);


        DOTween.To(x => EntityIcon.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f)
                .OnComplete(() => OnEvolutionFinished?.Invoke());
    }

    void OnCountChanged(int listPos, int total) { _armyCountLabel.text = $"{total}"; }

    protected override void DisplayTooltip()
    {
        EntityElement tooltip = new(ArmyGroup.ArmyEntity);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
