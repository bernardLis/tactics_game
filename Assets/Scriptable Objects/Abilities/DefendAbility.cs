using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Defend Ability")]
public class DefendAbility : Ability
{
    DefendTriggerable defendTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        defendTriggerable = obj.GetComponent<DefendTriggerable>();
    }

    public async override Task HighlightTargetable()
    {
        // there is no highlight targetable in defend ability
        await highlighter.ClearHighlightedTiles();

        // TODO: this is a hacky way it works for now but I don't know if I want to live this way.
        battleCharacterController.SetSelectedAbility(this);
        battleCharacterController.Select(null);

        battleCharacterController.characterState = CharacterState.SelectingFaceDir;
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject _target)
    {
        if (!await defendTriggerable.Defend(value, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }


}
