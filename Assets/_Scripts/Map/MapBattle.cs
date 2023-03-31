using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapBattle : MonoBehaviour, ITooltipDisplayable
{
    GameManager _gameManager;

    public Battle Battle { get; private set; }

    VisualElement _root;
    BattleTooltipElement _battleTooltipElement;

    public void Initialize(Battle b)
    {
        _gameManager = GameManager.Instance;
        _root = DashboardManager.Instance.Root;
        Battle = b;
    }
    public void TakeBattle(MapHero h)
    {
        Battle.Character = h.Character;
        _gameManager.LoadBattle(Battle);
    }

    public string GetTooltipText() { return "Battle"; }

    public void DisplayBattleTooltip()
    {
        _battleTooltipElement = new(Battle);
        _root.Add(_battleTooltipElement);
    }

    public void HideBattleTooltip()
    {
        _root.Remove(_battleTooltipElement);
    }
}
