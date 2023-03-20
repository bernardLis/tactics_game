using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBattle : MonoBehaviour, ITooltipDisplayable
{
    GameManager _gameManager;

    public Battle Battle { get; private set; }

    public void Initialize(Battle b)
    {
        _gameManager = GameManager.Instance;
        Battle = b;
    }
    public void TakeBattle(MapHero h)
    {
        Battle.Character = h.Character;
        _gameManager.LoadBattle(Battle);
    }
    public string GetTooltipText() { return "Battle"; }
}
