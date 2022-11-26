using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class AbilityNodeVisualElement : VisualWithTooltip
{
    AbilityNode _abilityNode;

    VisualElement _icon;
    VisualElement _tooltipElement;

    Sprite[] _unlockAnimationSprites;
    VisualElement _unlockAnimationContainer;
    IVisualElementScheduledItem _unlockAnimation;
    int _unlockSpriteIndex = 0;

    public AbilityNodeVisualElement(AbilityNode abilityNode)
    {
        _abilityNode = abilityNode;
        AddToClassList("abilityNodeContent");

        AddIcon();
        AddCost();
        AddTooltip();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (_abilityNode.Unlock())
            UnlockNode();
        else
            ShakeNode();
    }

    void UnlockNode()
    {
        PlayUnlockAnimation();
    }

    void PlayUnlockAnimation()
    {
        _unlockAnimationSprites = GameManager.Instance.GameDatabase.AbilityUnlockAnimationSprites;

        _unlockAnimationContainer = new();
        _unlockAnimationContainer.style.position = Position.Absolute;
        _unlockAnimationContainer.style.width = Length.Percent(100);
        _unlockAnimationContainer.style.height = Length.Percent(100);
        Add(_unlockAnimationContainer);

        _unlockAnimation = _unlockAnimationContainer.schedule.Execute(UnlockAnimation).Every(20);
    }

    void UnlockAnimation()
    {
        _unlockAnimationContainer.style.backgroundImage = new StyleBackground(_unlockAnimationSprites[_unlockSpriteIndex]);
        _unlockSpriteIndex++;
        if (_unlockSpriteIndex == _unlockAnimationSprites.Length)
        {
            _unlockAnimation.Pause();
            Remove(_unlockAnimationContainer);

            _icon.style.backgroundImage = new(_abilityNode.IconUnlocked);
        }
    }

    void ShakeNode()
    {
        Debug.Log($"shaking node, can't be bought");
        DOTween.Shake(() => transform.position, x => transform.position = x, 1f, 6f);
    }

    void AddIcon()
    {
        _icon = new();
        _icon.AddToClassList("abilityNodeIcon");
        StyleBackground s = _abilityNode.IsUnlocked ? new(_abilityNode.IconUnlocked) : new(_abilityNode.IconLocked);
        _icon.style.backgroundImage = s;
        Add(_icon);
    }

    void AddCost()
    {
        VisualElement cost = new();
        cost.style.flexDirection = FlexDirection.Row;
        cost.Add(new SpiceElement(_abilityNode.AbilityNodeUnlockCost.YellowSpiceCost, SpiceColor.Yellow));
        cost.Add(new SpiceElement(_abilityNode.AbilityNodeUnlockCost.BlueSpiceCost, SpiceColor.Blue));
        cost.Add(new SpiceElement(_abilityNode.AbilityNodeUnlockCost.RedSpiceCost, SpiceColor.Red));
        Add(cost);
    }

    void AddTooltip()
    {
        _tooltipElement = new();
        _tooltipElement.AddToClassList("textPrimary");
        Label description = new(_abilityNode.Description);
        _tooltipElement.Add(description);
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();

        _tooltip = new(this, _tooltipElement);

        base.DisplayTooltip();
    }
}
