using UnityEngine;
using UnityEngine.UIElements;

public class PlaceOfPower : PlaceOf
{
    [SerializeField] Ability _abilityToAdd;

    protected override void Initialize()
    {
        if (_abilityToAdd == null)
            _abilityToAdd = GameManager.Instance.GameDatabase.GetRandomAbility();

        _sr.sprite = _abilityToAdd.Icon;
        _ps.textureSheetAnimation.SetSprite(0, _abilityToAdd.Icon);
    }

    protected override void EnterPlaceOf(CharacterStats stats)
    {
        base.EnterPlaceOf(stats);
        if (!stats.CompareTag(Tags.Player))
            return;
        stats.ReplaceAbility(_abilityToAdd);
    }

    protected override void ExitPlaceOf(CharacterStats stats)
    {
        base.ExitPlaceOf(stats);
        if (!stats.CompareTag(Tags.Player))
            return;
        stats.RevertAbilityReplace();
    }

    public override VisualElement DisplayText()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        container.AddToClassList("textPrimary");

        Label txt = new Label("Place of power. Replaces your first ability as long as you stand on it.");
        txt.style.whiteSpace = WhiteSpace.Normal;
        container.Add(txt);

        return container;
    }


}
