using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Threading.Tasks;

public class Status : BaseScriptableObject
{
    public string ReferenceID;
    public int NumberOfTurns;
    public int Value;
    public Sprite Icon;

    public string DisplayText;
    public Color DisplayColor;
    public string AnimationEffectOnBody;

    protected GameObject _characterGameObject;
    protected CharacterSelection _characterSelection;
    protected BaseStats _baseStats;
    protected CharacterStats _characterStats;

    protected BattleCharacterController _battleCharacterController;
    protected ObjectUI _damageUI;

    [HideInInspector] public GameObject Attacker;

    public virtual void Initialize(GameObject self, GameObject attacker)
    {
        _characterGameObject = self;
        _characterSelection = self.GetComponent<CharacterSelection>();
        // TODO: ugh... 
        _baseStats = self.GetComponent<BaseStats>();
        _characterStats = self.GetComponent<CharacterStats>();

        _damageUI = self.GetComponent<ObjectUI>();
        _battleCharacterController = BattleCharacterController.Instance;
        if (_characterStats != null)
            AddFlag();
        if (attacker != null)
            Attacker = attacker;
    }

    public async virtual Task FirstTrigger()
    {
        // "normal" status application and triggering is when you apply status on character from opposite team, then it works fine! 
        // TODO: but when you apply it on person from your team, it is a bit weird. 
        await TriggerStatus();
        await Task.Yield();
    }

    public async virtual Task TriggerStatus()
    {
        _damageUI.DisplayOnCharacter(DisplayText, 24, DisplayColor);
        await Task.Yield();
    }

    public virtual void ResolveTurnEnd()
    {
        NumberOfTurns--;
    }

    public virtual bool ShouldTrigger()
    {
        if (NumberOfTurns > 0)
            return true;

        return false;
    }

    protected virtual void HandleFirstTurn()
    {
        // meant to be overwritten; 
    }
    public virtual void AddFlag()
    {
    }

    public virtual void ResetFlag()
    {
    }

    public virtual string GetDescription()
    {
        // meant to be overwritten
        return null;
    }

    public virtual bool ShouldBeRemoved()
    {
        return NumberOfTurns <= 0;
    }
}
