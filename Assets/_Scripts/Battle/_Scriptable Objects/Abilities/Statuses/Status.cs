using UnityEngine;


public class Status : BaseScriptableObject
{
    public int NumberOfTurns;
    public int Value;
    public Sprite Icon;

    public string DisplayOnCharacterText;
    public Color DisplayOnCharacterColor;

    protected GameObject _characterGameObject;
    protected CharacterSelection _characterSelection;
    protected CharacterStats _characterStats;

    protected BattleCharacterController _battleCharacterController;
    protected DamageUI _damageUI;

    protected GameObject _attacker;

    public virtual void Initialize(GameObject self, GameObject attacker)
    {
        _characterGameObject = self;
        _characterSelection = self.GetComponent<CharacterSelection>();
        _characterStats = self.GetComponent<CharacterStats>();

        _damageUI = self.GetComponent<DamageUI>();
        _battleCharacterController = BattleCharacterController.instance;

        if (attacker != null)
            this._attacker = attacker;
    }

    public virtual void FirstTrigger()
    {
        // "normal" status application and triggering is when you apply status on character from opposite team, then it works fine! 
        // but when you apply it on person from your team, it is a bit weird. 
        TriggerStatus();
    }

    public virtual void TriggerStatus()
    {
        _damageUI.DisplayOnCharacter(DisplayOnCharacterText, 24, DisplayOnCharacterColor);
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

    public virtual void ResetFlag()
    {
        // meant to be overwritten; 
    }

    public virtual string GetDescription()
    {
        // meant to be overwritten
        return null;
    }

    public virtual bool ShouldBeRemoved()
    {
        Debug.Log("number of turns in should be removed " + NumberOfTurns);
        return NumberOfTurns <= 0;
    }
}
