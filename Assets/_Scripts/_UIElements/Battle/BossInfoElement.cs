using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossInfoElement : EntityInfoElement
{
    BattleBoss _battleBoss;

    ResourceBarElement _stunBar;
    public BossInfoElement(BattleBoss bs) : base(bs)
    {
        _battleBoss = bs;

        style.minWidth = 600;

        string text = "Stun";
        if (_gameManager.GlobalUpgradeBoard.GetUpgradeByName("Boss Stun").CurrentLevel == -1)
            text = "Buy Upgrade To Stun Boss";


        Color c = _gameManager.GameDatabase.GetColorByName("Stun").Color;
        _stunBar = new(c, text, bs.CurrentDamageToBreakCorruption,
                                bs.TotalDamageToBreakCorruption);

        _stunBar.HideText();
        _stunBar.style.backgroundImage = null;
        _stunBar.style.minWidth = 300;
        _stunBar.style.height = 20;
        _stunBar.style.opacity = 0.8f;

        // _stunBar.ResourceBar.style.height = Length.Percent(100);
        // _stunBar.ResourceBar.style.width = Length.Percent(100);
        // _stunBar.ResourceBar.style.marginLeft = Length.Percent(0);
        // _stunBar.ResourceBar.style.marginRight = Length.Percent(0);

        // _stunBar.MissingBar.style.height = Length.Percent(100);

        Add(_stunBar);

        bs.OnCorruptionStarted += OnCorruptionStarted;
        bs.OnCorruptionBroken += OnCorruptionBroken;
        bs.OnStunFinished += OnStunFinished;
    }

    void OnCorruptionBroken()
    {
        _stunBar.UpdateTrackedVariables(_battleBoss.CurrentStunDuration,
                                        _battleBoss.TotalStunDuration);
    }

    void OnCorruptionStarted()
    {
        _stunBar.UpdateTrackedVariables(_battleBoss.CurrentDamageToBreakCorruption,
                                        _battleBoss.TotalDamageToBreakCorruption);
    }

    void OnStunFinished()
    {
        _stunBar.UpdateTrackedVariables(_battleBoss.CurrentDamageToBreakCorruption,
                                        _battleBoss.TotalDamageToBreakCorruption);
    }
}
