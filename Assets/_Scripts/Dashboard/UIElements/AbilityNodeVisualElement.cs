using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class AbilityNodeVisualElement : VisualWithTooltip
{
    public AbilityNode AbilityNode;

    VisualElement _icon;
    VisualElement _tooltipElement;

    SpiceElement _costElement;

    public AbilityNodeVisualElement(AbilityNode abilityNode)
    {
        AbilityNode = abilityNode;
        AddToClassList("abilityNodeContent");

        AddIcon();
        AddTooltip();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public void AddCostElement(SpiceElement el) { _costElement = el; }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (AbilityNode.Unlock())
            UnlockNode();
        else
            ShakeNode();
    }

    void UnlockNode() { PlayUnlockAnimation(); }

    async void PlayUnlockAnimation()
    {
        RemoveCostElement();

        Vector3 pos = this.worldTransform.GetPosition();
        pos.x = pos.x + this.resolvedStyle.width / 2;
        pos.y = Camera.main.pixelHeight - pos.y - this.resolvedStyle.height; // inverted, plus play on bottom of element
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        worldPos.z = 0;

        if (AbilityNode.UnlockEffect != null)
            await AbilityNode.UnlockEffect.PlayEffectAwaitable(worldPos, new Vector3(0.5f, 1f, 1f));

        _icon.style.backgroundImage = new StyleBackground(AbilityNode.IconUnlocked);
    }

    async void RemoveCostElement()
    {
        await _costElement.AwaitableChangeAmount(0);
        _costElement.style.display = DisplayStyle.None;
    }

    void ShakeNode()
    {
        DOTween.Shake(() => transform.position, x => transform.position = x, 1f, 6f);
    }

    void AddIcon()
    {
        _icon = new();
        _icon.AddToClassList("abilityNodeIcon");
        StyleBackground s = AbilityNode.IsUnlocked ? new(AbilityNode.IconUnlocked) : new(AbilityNode.IconLocked);
        _icon.style.backgroundImage = s;
        Add(_icon);
    }

    void AddTooltip()
    {
        _tooltipElement = new();
        _tooltipElement.AddToClassList("textPrimary");
        Label description = new(AbilityNode.Description);
        _tooltipElement.Add(description);
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();

        _tooltip = new(this, _tooltipElement);

        base.DisplayTooltip();
    }
}
