using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FogOfWarSquare : MonoBehaviour
{
    public bool IsExplored { get; private set; }
    public bool IsVisible { get; private set; }

    [SerializeField] Sprite[] _sprites;
    SpriteRenderer _sr;

    public event Action<bool> OnVisibilityChanged;
    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        _sr.sprite = _sprites[Random.Range(0, _sprites.Length)];
    }

    public void ResetVisibility()
    {
        IsVisible = false;
        if (IsExplored) SetExplored();
        OnVisibilityChanged?.Invoke(IsVisible);
    }

    public void SetExplored()
    {
        IsExplored = true;
        _sr.enabled = true;
        _sr.color = new(0, 0, 0, 0.25f);
    }

    public void SetVisible()
    {
        IsExplored = true;
        IsVisible = true;

        _sr.enabled = false;
        OnVisibilityChanged?.Invoke(IsVisible);
    }
}
