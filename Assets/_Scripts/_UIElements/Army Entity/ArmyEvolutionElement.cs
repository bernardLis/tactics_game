using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyEvolutionElement : VisualElement
{
    const string _ussClassName = "army-evolution__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;


    ChangingValueElement _killCounter;
    MyButton _evolveButton;

    public ArmyGroup ArmyGroup;

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

        container.Add(new ArmyGroupElement(armyGroup));

        VisualElement killCounterContainer = new();
        killCounterContainer.style.alignItems = Align.Center;
        _killCounter = new();
        _killCounter.Initialize(armyGroup.OldKillCount, 24);
        killCounterContainer.Add(new Label($"Kill count:"));
        killCounterContainer.Add(_killCounter);
        container.Add(killCounterContainer);

        _evolveButton = new MyButton($"Evolve for: {armyGroup.NumberOfKillsToEvolve()} kills",
                 null, Evolve);
        _evolveButton.SetEnabled(false);
        container.Add(_evolveButton);
    }

    public void AddKillCount(int delay)
    {
        schedule.Execute(() =>
        {
            _killCounter.ChangeAmount(ArmyGroup.KillCount);
            _killCounter.OnAnimationFinished += ResolveEvolveButton;
        }).StartingIn(delay);
    }

    void ResolveEvolveButton()
    {
        if (!ArmyGroup.ShouldEvolve()) return;
        _evolveButton.SetEnabled(true);
    }

    void Evolve()
    {

        ArmyGroup.Evolve();
        _killCounter.ChangeAmount(ArmyGroup.KillCount);

        _evolveButton.SetEnabled(false);
    }
}
