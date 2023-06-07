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

    public CreatureIcon EntityIcon;
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
        armyGroup.OnCountChanged += OnCountChanged;

        IsLocked = isLocked;

        AddToClassList(_ussMain);

        EntityIcon = new(armyGroup.Creature);
        Add(EntityIcon);

        _armyCountLabel = new($"{armyGroup.NumberOfUnits}");
        _armyCountLabel.AddToClassList(_ussCount);
        Add(_armyCountLabel);
    }

    public void LargeIcon()
    {
        style.width = 200;
        style.height = 200;

        EntityIcon.LargeIcon();
    }

    public void Evolve(Creature armyEntity)
    {
        DOTween.Shake(() => EntityIcon.transform.position, x => EntityIcon.transform.position = x,
                2f, 10f);

        Helpers.DisplayTextOnElement(BattleManager.Instance.Root, EntityIcon, "Evolving!!!", Color.red);

        Color _initialColor = EntityIcon.Frame.style.backgroundColor.value;
        Color _targetColor = Color.white;
        DOTween.To(() => EntityIcon.Frame.style.backgroundColor.value,
                x => EntityIcon.Frame.style.backgroundColor = x, _targetColor, 1f)
            .SetTarget(EntityIcon)
            .OnComplete(() => EntityIcon.SwapCreature(armyEntity));

        DOTween.To(() => EntityIcon.Frame.style.backgroundColor.value,
                x => EntityIcon.Frame.style.backgroundColor = x, _initialColor, 2f)
            .SetTarget(EntityIcon)
            .SetDelay(1f)
            .OnComplete(() => OnEvolutionFinished?.Invoke());
    }

    void OnCountChanged(int listPos, int total) { _armyCountLabel.text = $"{total}"; }

    protected override void DisplayTooltip()
    {
        ArmyGroupTooltip tooltip = new(ArmyGroup);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
