using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossCardFull : EntityMovementCardFull
{

    Boss _boss;
    public BossCardFull(Boss boss) : base(boss)
    {
        _boss = boss;
    }

    public override void Initialize()
    {
        base.Initialize();
        AddBossAttacks();
    }

    void AddBossAttacks()
    {
        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonHorizontalSpacer);
        _mainCardContainer.Add(spacer);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        container.style.justifyContent = Justify.Center;
        container.style.alignItems = Align.Center;

        _mainCardContainer.Add(container);

        foreach (var attack in _boss.Attacks)
        {
            BossAttackElement attackElement = new(attack);
            container.Add(attackElement);
        }
    }

}
