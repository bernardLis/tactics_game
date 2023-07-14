using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BattleBase : Singleton<BattleBase>
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Base _base;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;

        _base = _gameManager.SelectedBattle.Base;

        transform.position = new Vector3(0, 1.89f, 0);
    }
}
