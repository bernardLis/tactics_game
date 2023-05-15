using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class RewardCard : VisualElement
{
    GameManager _gameManager;
    const string _ussClassName = "reward-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussDisabled = _ussClassName + "disabled";

    public Reward Reward;

    public RewardCard(Reward reward)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardCardStyles);
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

    public void DisableCard()
    {
        SetEnabled(false);
        AddToClassList(_ussDisabled);
    }

    public void DisableClicks() { UnregisterCallback<PointerUpEvent>(OnPointerUp); }

}
