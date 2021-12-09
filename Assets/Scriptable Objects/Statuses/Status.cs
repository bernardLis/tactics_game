using UnityEngine;


public class Status : ScriptableObject
{
    public int numberOfTurns;
    public int value;
    public Sprite icon;
    public string displayOnCharacterText;
    public Color displayOnCharacterColor;

    GameObject characterGameObject;
    protected BattleCharacterController battleCharacterController;
    protected DamageUI damageUI;

    public virtual void Initialize(GameObject _self)
    {
        characterGameObject = _self;
        damageUI = _self.GetComponent<DamageUI>();
        battleCharacterController = BattleCharacterController.instance;
    }

    public virtual void TriggerStatus()
    {
        damageUI.DisplayOnCharacter(displayOnCharacterText, 24, displayOnCharacterColor);
    }

    public virtual string GetDescription()
    {
        // meant to be overwritten
        return null;
    }
}
