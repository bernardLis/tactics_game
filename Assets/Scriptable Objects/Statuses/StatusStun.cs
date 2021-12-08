using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/Stun")]
public class StatusStun : Status
{
    CharacterSelection characterSelection;
    public override void Initialize(GameObject _self)
    {
        base.Initialize(_self);

        characterSelection = _self.GetComponent<CharacterSelection>();
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();
        characterSelection.FinishCharacterTurn();        
    }

    public override string GetDescription()
    {
        return "Stuns target for " + numberOfTurns + " turn/s.";
    }
}
