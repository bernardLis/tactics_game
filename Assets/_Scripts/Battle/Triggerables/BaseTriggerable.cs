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
        string abilityName = Helpers.ParseScriptableObjectCloneName(ability.name);
        _battleUI.DisplayBattleLog(new Label($"{gameObject.name.Trim()} uses {abilityName}."));
    }
}
