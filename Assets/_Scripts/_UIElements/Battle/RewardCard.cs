using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCard : VisualElement
{
    const string _ussClassName = "reward-card__";
    const string _ussMain = _ussClassName + "main";

    protected Reward _reward;

    public RewardCard(Reward reward)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _reward = reward;

        AddToClassList(_ussMain);

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0)
            return;

        _reward.GetReward();
    }
}
