using UnityEngine;


public class Status : BaseScriptableObject
{
    public int numberOfTurns;
    public int value;
    public Sprite icon;
    public string displayOnCharacterText;
    public Color displayOnCharacterColor;

    protected GameObject characterGameObject;
    protected BattleCharacterController battleCharacterController;
    protected DamageUI damageUI;

    protected GameObject attacker;

    public virtual void Initialize(GameObject _self, GameObject _attacker)
    {
        characterGameObject = _self;
        damageUI = _self.GetComponent<DamageUI>();
        battleCharacterController = BattleCharacterController.instance;

        if (_attacker != null)
            attacker = _attacker;
    }

    public virtual void FirstTrigger()
    {
        // "normal" status application and triggering is when you apply status on character from opposite team, then it works fine! 
        // but when you apply it on person from your team, it is a bit weird. 
        TriggerStatus();
    }

    public virtual void TriggerStatus()
    {
        damageUI.DisplayOnCharacter(displayOnCharacterText, 24, displayOnCharacterColor);
    }

    public virtual void ResolveTurnEnd()
    {
        numberOfTurns--;
    }

    public virtual bool ShouldTrigger()
    {
        if (numberOfTurns > 0)
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
        Debug.Log("number of turns in should be removed " + numberOfTurns);
        return numberOfTurns <= 0;
    }
}
