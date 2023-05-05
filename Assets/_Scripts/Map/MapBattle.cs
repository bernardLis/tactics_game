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
        _gameManager.SelectedBattle = Battle;
        _gameManager.LoadScene(Scenes.Battle);
    }

    public string GetTooltipText() { return "Battle"; }

    public void DisplayBattleTooltip()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        _battleTooltipElement = new(Battle, pos);
        _root.Add(_battleTooltipElement);
    }

    public void HideBattleTooltip()
    {
        if (_battleTooltipElement == null) return;
        if (_battleTooltipElement.parent != _root) return;
        _root.Remove(_battleTooltipElement);
    }
}
