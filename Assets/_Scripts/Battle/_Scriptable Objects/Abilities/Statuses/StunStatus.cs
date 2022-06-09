using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Stun")]
public class StunStatus : Status
{
    public override void FirstTrigger()
    {
        // this is when you apply stun on your mate
        if (Attacker.CompareTag(_selfGameObject.tag) && !_characterSelection.HasFinishedTurn)
            _characterSelection.FinishCharacterTurn();

        // this is normal situation, when you apply stun on person from opposite team
        base.FirstTrigger();
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();
        _characterSelection.GrayOutCharacter();
        AddFlag();
    }

    public override void AddFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsStunned(true);
    }

    public override void ResetFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsStunned(false);
    }

    public override string GetDescription()
    {
        return "Stun for " + NumberOfTurns + " turn/s.";
    }
}
