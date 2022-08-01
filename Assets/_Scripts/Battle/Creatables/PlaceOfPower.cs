using UnityEngine;
using UnityEngine.UIElements;

public class PlaceOfPower : PlaceOf
{
    [SerializeField] Ability _abilityToAdd;

    protected override void SetUpVisuals()
    {
        _sr.sprite = _abilityToAdd.Icon;
        _ps.textureSheetAnimation.SetSprite(0, _abilityToAdd.Icon);
    }

    protected override void EnterPlaceOf(CharacterStats stats)
    {
        base.EnterPlaceOf(stats);
        stats.ReplaceAbility(_abilityToAdd);
    }

    protected override void ExitPlaceOf(CharacterStats stats)
    {
        base.ExitPlaceOf(stats);
        stats.RevertAbilityReplace();
    }

    public override VisualElement DisplayText()
    {
        return new Label("Place of power. Replaces your first ability as long as you stand on it.");
    }


}
