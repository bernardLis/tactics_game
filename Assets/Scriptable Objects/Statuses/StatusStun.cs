using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/Stun")]
public class StatusStun : Status
{
    CharacterSelection characterSelection;
    CharacterStats characterStats;


    public override void Initialize(GameObject _self, GameObject _attacker)
    {
        base.Initialize(_self, _attacker);

        characterSelection = _self.GetComponent<CharacterSelection>();
        characterStats = _self.GetComponent<CharacterStats>();
    }


    protected override void HandleFirstTurn()
    {
        Debug.Log("handle first turn in stun");
        Debug.Log("attacker.tag " + attacker.tag);
        Debug.Log("characterGameObject.tag " + characterGameObject.tag);

        // if you are stunned by your boy (ally) and you have not finished the turn
        if (attacker.CompareTag(characterGameObject.tag) && !characterSelection.hasFinishedTurn)
            characterSelection.FinishCharacterTurn();
    }


    public override void TriggerStatus()
    {
        base.TriggerStatus();
        characterStats.SetIsStunned(true);
    }

    public override void ResetFlag()
    {
        if (ShouldResetFlag())
            characterStats.SetIsStunned(false);
    }

    public override string GetDescription()
    {
        return "Stuns target for " + numberOfTurns + " turn/s.";
    }
}
