using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleLivesElement : ElementWithTooltip
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-lives__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";

    GameManager _gameManager;
    BattleSpire _battleSpire;

    Label _icon;
    Label _text;

    Tween _shakeTween;

    public BattleLivesElement()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleLivesStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleSpire = BattleSpire.Instance;
        if (_battleSpire == null)
        {
            Debug.LogError("No spire found.");
            return;
        }

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _icon = new();
        _icon.AddToClassList(_ussIcon);
        Add(_icon);

        _text = new();
        Add(_text);

        _battleSpire.Spire.StoreyLives.CurrentLives.OnValueChanged += UpdateText;
        _battleSpire.Spire.StoreyLives.MaxLivesTree.CurrentValue.OnValueChanged += UpdateText;

        UpdateText(default);
    }

    void UpdateText(int _)
    {
        _text.text = $"{_battleSpire.Spire.StoreyLives.CurrentLives.Value} / {_battleSpire.Spire.StoreyLives.MaxLivesTree.CurrentValue.Value}";

        if (_shakeTween.IsActive()) return;

        _shakeTween = DOTween.Shake(() => _icon.transform.position, x => _icon.transform.position = x,
             0.5f, 5f).SetUpdate(true);

    }

    protected override void DisplayTooltip()
    {
        Label tooltip = new("Lives");
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }
}
