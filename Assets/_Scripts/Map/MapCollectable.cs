using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MapCollectable : MonoBehaviour, ITooltipDisplayable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] SpriteRenderer _gfx;

    Collectable _collectable;

    string _tooltipText;

    public void Initialize(Collectable collectable)
    {
        _collectable = collectable;
        _gfx.sprite = collectable.Sprite;

        _tooltipText = $"{Helpers.ParseScriptableObjectCloneName(_collectable.name)}";
        if (collectable.Amount > 0)
            _tooltipText = $"{collectable.Amount} x {Helpers.ParseScriptableObjectCloneName(_collectable.name)}";
    }

    public void OnPointerEnter(PointerEventData evt) { }

    public void OnPointerExit(PointerEventData evt) { }

    public void Collect(MapHero hero)
    {
        GetComponent<BoxCollider2D>().enabled = false;

        _collectable.Collect(hero);

        transform.DOLocalJump(transform.position + Vector3.up, 3, 1, 0.5f);
        _gfx.DOColor(new Color(1f, 1f, 1f, 0f), 1f).OnComplete(() => gameObject.SetActive(false));

        Bounds b = new(transform.position, Vector3.one * 2);
        AstarPath.active.UpdateGraphs(b);

        hero.FloatText($"+ {_tooltipText}");
    }

    public string GetTooltipText() { return _tooltipText; }

}
