using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class MapCollectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] SpriteRenderer _gfx;

    Collectable _collectable;
    Canvas _canvas;
    TextMeshProUGUI _textMesh;

    public void Initialize(Collectable collectable)
    {
        _collectable = collectable;
        _gfx.sprite = collectable.Sprite;

        _canvas = GetComponentInChildren<Canvas>();
        _canvas.enabled = false;

        _textMesh = GetComponentInChildren<TextMeshProUGUI>();

        string str = $"{Helpers.ParseScriptableObjectCloneName(_collectable.name)}";
        if (collectable.Amount > 0)
            str = $"{collectable.Amount} x {Helpers.ParseScriptableObjectCloneName(_collectable.name)}";
        _textMesh.text = str;
    }

    public void OnPointerEnter(PointerEventData evt) { _canvas.enabled = true; }
    public void OnPointerExit(PointerEventData evt) { _canvas.enabled = false; }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<MapHero>(out MapHero hero))
            Collect(hero);
    }

    void Collect(MapHero hero)
    {
        GetComponent<BoxCollider2D>().enabled = false;

        _collectable.Collect(hero);

        transform.DOLocalJump(transform.position + Vector3.up, 3, 1, 0.5f);
        _gfx.DOColor(new Color(1f, 1f, 1f, 0f), 1f).OnComplete(() => gameObject.SetActive(false));
    }


}
