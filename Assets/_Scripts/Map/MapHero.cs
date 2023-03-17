using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHero : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] SpriteRenderer _gfx;
    [SerializeField] SpriteRenderer _selection;
    public Character Character { get; private set; }

    public float RangeLeft { get; private set; }

    public void Initialize(Character c)
    {
        Character = c;

        _gfx.sprite = c.Portrait.Sprite;
        _selection.enabled = false;

        RangeLeft = Character.Speed.GetValue();

        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void OnDayPassed(int day) { RangeLeft = Character.Speed.GetValue(); }

    public void UpdateRangeLeft(float distanceTraveled) { RangeLeft -= distanceTraveled; }

    public void UpdateMapPosition() { Character.MapPosition = transform.position; }

    public void Select() { _selection.enabled = true; }

    public void Unselect() { _selection.enabled = false; }
}
