using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MapHero : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] SpriteRenderer _gfx;
    [SerializeField] SpriteRenderer _selection;

    [SerializeField] GameObject _canvasContainer;
    [SerializeField] TextMeshProUGUI _floatingText;

    public Character Character { get; private set; }

    public Vector3 _lastDestination;

    public FloatVariable RangeLeft { get; private set; }
    // public float RangeLeft { get; private set; }

    public void Initialize(Character c)
    {
        Character = c;

        _gfx.sprite = c.Portrait.Sprite;
        _selection.enabled = false;

        RangeLeft = ScriptableObject.CreateInstance<FloatVariable>();
        RangeLeft.SetValue(Character.Speed.GetValue());

        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void OnDayPassed(int day)
    {
        RangeLeft.SetValue(Character.Speed.GetValue());
    }

    public void SetLastDestination(Vector3 pos) { _lastDestination = pos; }
    public Vector3 GetLastDestination() { return _lastDestination; }

    public void UpdateRangeLeft(float distanceTraveled) { RangeLeft.ApplyChange(-distanceTraveled); }

    public void UpdateMapPosition() { Character.MapPosition = transform.position; }

    public void Select() { _selection.enabled = true; }

    public void Unselect() { _selection.enabled = false; }

    public void FloatText(string txt)
    {
        _canvasContainer.SetActive(true);

        _floatingText.color = Color.white;
        _floatingText.text = txt;

        _canvasContainer.transform.DOLocalMove(Vector3.up * 2f, 2f)
            .OnComplete(() =>
            {
                _canvasContainer.transform.localPosition = Vector3.zero;
                _canvasContainer.SetActive(false);
            });

        Color c = new Color(1f, 1f, 1f, 0f);
        _floatingText.DOColor(c, 2f);
    }
}
