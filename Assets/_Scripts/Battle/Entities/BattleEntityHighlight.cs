using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class BattleEntityHighlight : MonoBehaviour
{
    [SerializeField] GameObject _highlightDiamondPrefab;
    BattleHighlightDiamond _highlightDiamond;

    BattleEntity _battleEntity;
    Disc _disc;

    public void Initialize(BattleEntity battleEntity)
    {
        _battleEntity = battleEntity;
        _battleEntity.OnDeath += (a, b, c) => DisableHighlightFully();

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

    void InitializeDiamond()
    {
        _highlightDiamond = Instantiate(_highlightDiamondPrefab, transform).GetComponent<BattleHighlightDiamond>();
        Bounds b = _battleEntity.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        float y = b.extents.y * 2;

        _highlightDiamond.Initialize(new Vector3(0, y, 0));
        _highlightDiamond.Disable();
        _highlightDiamond.Enable(Color.cyan);
    }

    public void Highlight(Color c)
    {
        if (_highlightDiamond == null) InitializeDiamond();

        _highlightDiamond.Enable(c);
    }

    public void ClearHighlight()
    {
        _highlightDiamond.Disable();
    }

    public void DisableHighlightFully()
    {
        _disc.gameObject.SetActive(false);
        _highlightDiamond.Disable();
    }
}
