using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/Stun")]
public class StatusStun : Status
{
    CharacterSelection characterSelection;
    CharacterStats characterStats;

    public override void Initialize(GameObject _self)
    {
        base.Initialize(_self);

        characterSelection = _self.GetComponent<CharacterSelection>();
        characterStats = _self.GetComponent<CharacterStats>();
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();

        characterSelection.FinishCharacterTurn();
        characterStats.SetIsStunned(true);
    }

    public override string GetDescription()
    {
        return "Stuns target for " + numberOfTurns + " turn/s.";
    }
}
