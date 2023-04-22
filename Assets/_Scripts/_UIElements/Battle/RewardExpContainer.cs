using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardExpContainer : VisualElement
{
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;

    public event Action OnContinue;
    public RewardExpContainer()
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        HeroCardQuest card = new HeroCardQuest(_gameManager.PlayerHero);
        _gameManager.PlayerHero.GetExp(100);
        Add(card);

        // TODO: normally, if the hero is not leveled up, we should wait a bit and show the rewards
        card.OnLeveledUp += AddContinueButton;

    }

    void AddContinueButton()
    {
        MyButton continueButton = new("Continue", _ussCommonMenuButton, () => OnContinue?.Invoke());
        Add(continueButton);
    }

}
