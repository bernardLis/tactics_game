using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCard : VisualElement
{
    const string _ussClassName = "reward-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussDisabled = _ussClassName + "disabled";

    public Reward Reward;

    public RewardCard(Reward reward)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Reward = reward;

        AddToClassList(_ussMain);

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0) return;

        Reward.GetReward();
    }

    public void DisableCard() { AddToClassList(_ussDisabled); }

    public void DisableClicks() { UnregisterCallback<PointerUpEvent>(OnPointerUp); }

}
