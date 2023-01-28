using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class AbilityNodeElement : ElementWithTooltip
{
    GameManager _gameManager;

    public AbilityNode AbilityNode;

    VisualElement _icon;
    VisualElement _tooltipElement;

    SpiceElement _costElement;

    VisualElement _overlay;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "ability-node__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussOverlay = _ussClassName + "overlay";
    const string _ussPermaLockOverlay = _ussClassName + "perma-lock-overlay";

    public AbilityNodeElement(AbilityNode abilityNode)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityNodeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AbilityNode = abilityNode;
        abilityNode.OnCooldownChanged += UpdateCooldownOverlay;
        AddToClassList(_ussMain);

        AddIcon();
        AddTooltip();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
        AddCooldownOverlay();

        if (abilityNode.IsPermaLocked)
            AddPermaLockOverlay();
    }


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
        {
            EffectHolder instance = ScriptableObject.Instantiate(AbilityNode.UnlockEffect);
            await instance.PlayEffectAwaitable(worldPos, new Vector3(0.5f, 1f, 1f));
        }

        _icon.style.backgroundImage = new StyleBackground(AbilityNode.IconUnlocked);
    }

    public void AddCostElement(SpiceElement el) { _costElement = el; }

    async void RemoveCostElement()
    {
        await _costElement.AwaitableChangeAmount(0);
        _costElement.style.display = DisplayStyle.None;
    }

    public void ShakeNode() { DOTween.Shake(() => transform.position, x => transform.position = x, 1f, 6f); }

    void AddIcon()
    {
        _icon = new();
        _icon.AddToClassList(_ussIcon);
        StyleBackground s = AbilityNode.IsUnlocked ? new(AbilityNode.IconUnlocked) : new(AbilityNode.IconLocked);
        _icon.style.backgroundImage = s;
        Add(_icon);
    }

    void AddTooltip()
    {
        _tooltipElement = new();
        _tooltipElement.AddToClassList(_ussCommonTextPrimary);
        Label description = new(AbilityNode.Description);
        _tooltipElement.Add(description);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipElement);
        base.DisplayTooltip();
    }

    void AddCooldownOverlay()
    {
        _overlay = new();
        _overlay.AddToClassList(_ussOverlay);
        Add(_overlay);

        UpdateCooldownOverlay();
    }

    void UpdateCooldownOverlay()
    {
        _overlay.style.display = DisplayStyle.None;

        if (!AbilityNode.IsOnCooldown)
            return;

        _overlay.style.display = DisplayStyle.Flex;
        _overlay.Clear();
        Label l = new($"{AbilityNode.DaysOnCooldownRemaining}");
        l.AddToClassList(_ussCommonTextPrimary);
        _overlay.Add(l);
    }

    void AddPermaLockOverlay()
    {
        VisualElement permaLockOverlay = new();
        permaLockOverlay.AddToClassList(_ussPermaLockOverlay);
        Add(permaLockOverlay);
    }

}
