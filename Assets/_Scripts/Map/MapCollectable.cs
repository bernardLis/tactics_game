using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapCollectable : MonoBehaviour
{
    [SerializeField] SpriteRenderer _gfx;

    Collectable _collectable;

    public void Initialize(Collectable collectable)
    {
        _collectable = collectable;
        _gfx.sprite = collectable.Sprite;
    }

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
