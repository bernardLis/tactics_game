using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class HeroCardMini : ElementWithTooltip
{
    const string _ussCommonTextSecondary = "common__text-secondary";
    const string _ussCommonTransitionBasic = "common__transition-basic";

    const string _ussClassName = "hero-card-mini__";
    const string _ussMain = _ussClassName + "main";
    const string _ussShadow = _ussClassName + "shadow";

    const string _ussTimerMain = _ussClassName + "timer-element-overlay-main";
    const string _ussTimerOverlayMask = _ussClassName + "timer-element-overlay-mask";
    const string _ussTimerLabelWrapper = _ussClassName + "timer-element-label-wrapper";

    GameManager _gameManager;
    public Hero Hero;

    HeroPortraitElement _portrait;
    VisualElement _shadow;

    public bool IsLocked;

    public event Action<HeroCardMini> OnLocked;
    public event Action<HeroCardMini> OnUnlocked;
    public HeroCardMini(Hero hero)
    {
        _gameManager = GameManager.Instance;
        var common = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardMiniStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;

        AddToClassList(_ussMain);

        _shadow = new();
        _shadow.AddToClassList(_ussShadow);
        _shadow.style.display = DisplayStyle.None;
        Add(_shadow);

        _portrait = new HeroPortraitElement(hero);
        Add(_portrait);


        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public void SmallCard()
    {
        style.minHeight = 100;
        style.minWidth = 100; 
        _portrait.SmallPortrait();
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0) return;
        evt.StopPropagation();

        HeroCardFull heroCardFull = new(Hero, BattleManager.Instance.Root);
    }

    protected override void DisplayTooltip()
    {
        HeroCardStats tooltip = new HeroCardStats(Hero);
        _tooltip = new(this, tooltip, true);
        base.DisplayTooltip();
    }

    public void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
    }

    public void Unlock()
    {
        IsLocked = false;
        OnUnlocked?.Invoke(this);
    }
}
