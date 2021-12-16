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
    public override void FirstTrigger()
    {
        Debug.Log("in first trigger");
        // this is when you apply stun on your mate
        if (attacker.CompareTag(characterGameObject.tag) && !characterSelection.hasFinishedTurn)
            characterSelection.FinishCharacterTurn();

        // this is normal situation, when you apply stun on person from opposite team
        base.FirstTrigger();
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();
        characterSelection.GrayOutCharacter();
        characterStats.SetIsStunned(true);
    }

    public override void ResetFlag()
    {
        characterStats.SetIsStunned(false);
    }

    public override string GetDescription()
    {
        return "Stuns target for " + numberOfTurns + " turn/s.";
    }
}
