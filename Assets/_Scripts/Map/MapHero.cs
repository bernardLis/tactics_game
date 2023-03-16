using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHero : MonoBehaviour
{
    [SerializeField] SpriteRenderer _gfx;
    [SerializeField] SpriteRenderer _selection;
    public Character Character { get; private set; }

    public void Initialize(Character c)
    {
        Character = c;
        _gfx.sprite = c.Portrait.Sprite;
        _selection.enabled = false;
    }

    public void Select()
    {
        _selection.enabled = true;
    }

    public void Unselect()
    {
        _selection.enabled = false;
    }
}
