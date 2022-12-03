using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Ability/Statuses/Stun")]
public class StunStatus : Status
{
    public override async Task FirstTrigger()
    {
        // this is when you apply stun on your mate
        if (Attacker.CompareTag(_characterGameObject.tag) && !_characterSelection.HasFinishedTurn)
            _characterSelection.FinishCharacterTurn();

        // this is normal situation, when you apply stun on person from opposite team
        await base.FirstTrigger();
    }

    public async override Task TriggerStatus()
    {
        await base.TriggerStatus();
        _characterSelection.SetCharacterColor(Helpers.GetColor("gray"));
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
