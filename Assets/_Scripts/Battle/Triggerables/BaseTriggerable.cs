using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseTriggerable : MonoBehaviour
{
    protected CharacterStats _myStats;
    protected CharacterRendererManager _characterRendererManager;
    BattleUI _battleUI;

    void Start()
    {
        _myStats = GetComponent<CharacterStats>();
        _characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        _battleUI = BattleUI.Instance;
    }

    protected void DisplayBattleLog(Ability ability)
    {
        VisualElement container = new();
        container.AddToClassList("textPrimary");
        container.style.flexDirection = FlexDirection.Row;
        Label txt = new($"{gameObject.name.Trim()} uses ");
        AbilityNameWithTooltip abilityNameWithTooltip = new(ability);

        container.Add(txt);
        container.Add(abilityNameWithTooltip);

        _battleUI.DisplayBattleLog(new BattleLogLine(container, BattleLogLineType.Ability));
    }
}
