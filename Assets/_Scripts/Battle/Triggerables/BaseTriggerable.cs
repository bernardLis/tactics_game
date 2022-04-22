using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

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
        _battleUI.DisplayBattleLog($"{gameObject.name} uses {abilityName} on {target.name} .");
    }


}
