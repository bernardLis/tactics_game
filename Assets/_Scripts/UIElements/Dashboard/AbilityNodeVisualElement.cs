using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class AbilityNodeVisualElement : VisualWithTooltip
{
    AbilityNode _abilityNode;

    VisualElement _icon;
    VisualElement _tooltipElement;

    VisualElement _unlockAnimationContainer;

    public AbilityNodeVisualElement(AbilityNode abilityNode)
    {
        _abilityNode = abilityNode;
        AddToClassList("abilityNodeContent");

        AddIcon();
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

    async void PlayUnlockAnimation()
    {

        _unlockAnimationContainer = new();
        _unlockAnimationContainer.style.position = Position.Absolute;
        _unlockAnimationContainer.style.width = Length.Percent(100);
        _unlockAnimationContainer.style.height = Length.Percent(100);
        Sprite[] animationSprites = GameManager.Instance.GameDatabase.AbilityUnlockAnimationSprites;
        _unlockAnimationContainer.Add(new AnimationVisualElement(animationSprites, 20, false));
        Add(_unlockAnimationContainer);

        await Task.Delay(animationSprites.Length * 20); // for animation to finish
        _icon.style.backgroundImage = new(_abilityNode.IconUnlocked);
        Remove(_unlockAnimationContainer);
    }

    void ShakeNode()
    {
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
