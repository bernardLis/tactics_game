using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class BattleEntityHighlight : MonoBehaviour
{
    BattleEntity _battleEntity;
    Disc _disc;

    public void Initialize(BattleEntity battleEntity)
    {
        _battleEntity = battleEntity;
        _battleEntity.OnDeath += (a, b, c) => DisableHighlight();

        InitializeDisc();
    }

    void InitializeDisc()
    {
        Color c = _battleEntity.GetHighlightColor();
        c.a = 0.25f;

        GameObject g = new("Highlight Disc");
        _disc = g.AddComponent<Disc>();
        _disc.Color = c;
        g.transform.SetParent(transform);
        g.transform.localPosition = -Vector3.up * 0.9f; // TODO: how do i know where is floor relative to entity?
        g.transform.localEulerAngles = Vector3.right * 90;
    }

    public void DisableHighlight()
    {
        _disc.gameObject.SetActive(false);
    }
}
