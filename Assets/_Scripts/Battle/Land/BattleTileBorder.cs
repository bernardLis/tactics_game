using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleTileBorder : MonoBehaviour
{
    BattleFightManager _battleFightManager;

    BattleTileBorderColorChanger _borderColorChanger;
    Color defaultColor = new(0.4f, 1f, 0.6f, 0.2f);
    Color fightColor = new(1f, 0.22f, 0f, 0.2f);

    bool _isGameBorder;

    public void EnableBorder(Color color, bool isGameBorder = false)
    {
        _isGameBorder = isGameBorder;
        if (_isGameBorder)
        {
            color = Color.magenta;
            defaultColor = Color.magenta;
            fightColor = Color.magenta;
        }

        float y = transform.localScale.y;
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        transform.DOScaleY(y, 0.5f)
            .SetEase(Ease.InOutSine);

        _battleFightManager = BattleFightManager.Instance;
        _battleFightManager.OnFightStarted += OnFightStarted;
        _battleFightManager.OnFightEnded += OnFightEnded;

        _borderColorChanger = GetComponentInChildren<BattleTileBorderColorChanger>();

        gameObject.SetActive(true);
        if (color == default) color = defaultColor;
        _borderColorChanger.ChangeColor(color);
    }

    void OnFightStarted()
    {
        if (_borderColorChanger != null)
            _borderColorChanger.ChangeColor(fightColor); // magic color
    }

    void OnFightEnded()
    {
        if (_borderColorChanger != null)
            _borderColorChanger.ChangeColor(defaultColor);
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void OnDisabled()
    {
        Unsubscribe();
    }

    void Unsubscribe()
    {
        if (_battleFightManager != null)
        {
            _battleFightManager.OnFightStarted -= OnFightStarted;
            _battleFightManager.OnFightEnded -= OnFightEnded;
        }
    }

    public void DestroySelf()
    {
        if (this == null) return;
        Unsubscribe();

        transform.DOScaleY(0, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => Destroy(gameObject));
    }

}
