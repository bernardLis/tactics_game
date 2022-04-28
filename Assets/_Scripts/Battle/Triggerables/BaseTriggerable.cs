using UnityEngine;

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

    protected void DisplayBattleLog(GameObject target, Ability ability)
    {
        string abilityName = Helpers.ParseScriptableObjectCloneName(ability.name);
        string targetName = Helpers.ParseScriptableObjectCloneName(target.name);
        _battleUI.DisplayBattleLog($"{gameObject.name} uses {abilityName} on {targetName}.");
    }


}
