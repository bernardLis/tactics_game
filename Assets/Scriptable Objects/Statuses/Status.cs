using UnityEngine;


public class Status : ScriptableObject
{
    public int numberOfTurns;
    public int value;
    public Sprite icon;
    public string displayOnCharacterText;
    public Color displayOnCharacterColor;

    protected GameObject characterGameObject;
    protected BattleCharacterController battleCharacterController;
    protected DamageUI damageUI;

    public bool isFirstTurn;
    protected GameObject attacker;

    public virtual void Initialize(GameObject _self, GameObject _attacker)
    {
        characterGameObject = _self;
        damageUI = _self.GetComponent<DamageUI>();
        battleCharacterController = BattleCharacterController.instance;
        isFirstTurn = true;

        if (_attacker != null)
            attacker = _attacker;
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
        // TODO: this is imperfect, coz if it was applied by character from the same team it won't trigger
        if (isFirstTurn)
        {
            HandleFirstTurn();
            isFirstTurn = false;
            return false;
        }

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

    public void SetIsFirstTurn(bool _is) { isFirstTurn = _is; }
}
