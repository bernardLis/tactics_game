using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class PlaceOfRestoration : PlaceOf
{
    protected override void Initialize()
    {
        _sr.sprite = Status.Icon;
        _ps.textureSheetAnimation.SetSprite(0, Status.Icon);
    }

    protected override void EnterPlaceOf(CharacterStats stats)
    {
        base.EnterPlaceOf(stats);
        stats.AddStatus(Status, null, false).GetAwaiter();
    }

    protected override void ExitPlaceOf(CharacterStats stats)
    {
        base.ExitPlaceOf(stats);
        stats.RemoveStatus(Status);
    }

    public override VisualElement DisplayText()
    {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        Label txt = new Label("Place of restoration. As long as you stand on it you get: ");
        txt.style.whiteSpace = WhiteSpace.Normal;
        ModifierVisual mod = new(Status);
        container.Add(txt);
        container.Add(mod);

        return container;
    }


}
