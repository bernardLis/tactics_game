using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCastle : MonoBehaviour, ITooltipDisplayable
{
    GameManager _gameManager;
    Castle _castle;

    void Start()
    {
        _gameManager = GameManager.Instance;
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

        // TODO: here cache;
        CastleElement el = new(DashboardManager.Instance.Root, _castle, h);
        DraggableArmies a = DashboardManager.Instance.GetComponent<DraggableArmies>();
        Debug.Log($"a {a}");
        el.OnSetUpFinished += a.Initialize;
        el.OnHide += a.Reset;
        a.Initialize();
    }

    public string GetTooltipText() { return _castle.name; }
}
