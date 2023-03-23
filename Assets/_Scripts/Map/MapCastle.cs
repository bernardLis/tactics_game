using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCastle : MonoBehaviour, ITooltipDisplayable
{
    Castle _castle;

    void Start()
    {

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
        CastleElement el = new(DashboardManager.Instance.Root, _castle);
    }

    public string GetTooltipText() { return _castle.name; }
}
