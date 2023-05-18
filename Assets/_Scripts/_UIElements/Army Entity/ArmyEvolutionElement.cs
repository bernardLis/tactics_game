using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class ArmyEvolutionElement : VisualElement
{
    const string _ussClassName = "army-evolution__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    ArmyGroupElement _armyGroupElement;
    ChangingValueElement _killCounter;

    public ArmyGroup ArmyGroup;

    public event Action OnFinished;
    public ArmyEvolutionElement(ArmyGroup armyGroup)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmyEvolutionElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        ArmyGroup = armyGroup;

        AddToClassList(_ussMain);

        Label groupName = new(armyGroup.Name);
        groupName.AddToClassList("common__text-primary");
        groupName.style.fontSize = 32;
        Add(groupName);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        _armyGroupElement = new(armyGroup);
        _armyGroupElement.LargeIcon();
        _armyGroupElement.EntityIcon.PlayAnimationAlways();

        container.Add(_armyGroupElement);

        VisualElement killCounterContainer = new();
        killCounterContainer.style.alignItems = Align.Center;
        _killCounter = new();
        _killCounter.Initialize(armyGroup.OldKillCount, 24);
        killCounterContainer.Add(new Label($"Kill count:"));
        killCounterContainer.Add(_killCounter);
        container.Add(killCounterContainer);

        style.opacity = 0;
        DOTween.To(x => style.opacity = x, 0, 1, 0.5f);

        AddKillCount(1000);
    }

    public void AddKillCount(int delay)
    {
        schedule.Execute(() =>
        {
            _killCounter.ChangeAmount(ArmyGroup.KillCount);
            _killCounter.OnAnimationFinished += ResolveEvolution;
            if (ArmyGroup.KillCount == ArmyGroup.OldKillCount) ResolveEvolution();

        }).StartingIn(delay);
    }

    void ResolveEvolution()
    {
        schedule.Execute(() =>
        {
            _killCounter.ChangeAmount(ArmyGroup.KillCount);

            schedule.Execute(() =>
            {
                if (!ArmyGroup.ShouldEvolve())
                {
                    OnFinished?.Invoke();
                    return;
                }
            }).StartingIn(1000);

            ArmyGroup.Evolve(); // HERE: need to wait for it to finish
            _armyGroupElement.OnEvolutionFinished += () => OnFinished?.Invoke();

        }).StartingIn(1000);
    }
}
