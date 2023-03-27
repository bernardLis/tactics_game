using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCastle : MonoBehaviour, ITooltipDisplayable
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    DraggableArmies _draggableArmies;

    Castle _castle;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _dashboardManager = DashboardManager.Instance;
        _draggableArmies = _dashboardManager.GetComponent<DraggableArmies>();

    }

    public void Initialize(Castle castle)
    {
        castle.Initialize(); // TODO: differentiate between owned / not owned castles
        _castle = castle;
        GetComponentInChildren<SpriteRenderer>().sprite = castle.Sprite;
    }

    public void VisitCastle(MapHero h)
    {
        Debug.Log($"{h.Character.CharacterName} is visiting {_castle.name}");

        _gameManager.ToggleTimer(false);

        CastleElement el = new(_dashboardManager.Root, _castle, h);
        el.OnHide += _draggableArmies.Reset;
        _draggableArmies.Initialize();
    }

    public string GetTooltipText() { return _castle.name; }
}
