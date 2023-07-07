using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureCardExp : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";

    const string _ussClassName = "creature-card-exp__";
    const string _ussMain = _ussClassName + "main";
    const string _ussMiddlePanel = _ussClassName + "middle-panel";
    const string _ussLevelUpButton = _ussClassName + "level-up-button";
    const string _ussMainFolded = _ussClassName + "main-folded";

    GameManager _gameManager;

    public Creature Creature;

    VisualElement _leftPanel;
    VisualElement _middlePanel;

    Label _name;
    public CreatureIcon CreatureIcon;

    IntVariable _currentSpice;
    IntVariable _spiceToNextLevel;
    ResourceBarElement _levelBar;
    Label _levelLabel;
    MyButton _levelUpButton;
    SpiceElement _buttonSpice;

    public CreatureCardExp(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureCardExpStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Creature = creature;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        AddLeftPanel();
        AddMiddlePanel();
    }

    void AddLeftPanel()
    {
        _leftPanel = new();
        Add(_leftPanel);

        CreatureIcon = new(Creature);
        _leftPanel.Add(CreatureIcon);
    }

    void AddMiddlePanel()
    {
        _middlePanel = new();
        _middlePanel.AddToClassList(_ussMiddlePanel);
        Add(_middlePanel);

        _name = new(Creature.Name);
        _middlePanel.Add(_name);

        _currentSpice = ScriptableObject.CreateInstance<IntVariable>();
        _currentSpice.SetValue(0);

        _spiceToNextLevel = ScriptableObject.CreateInstance<IntVariable>();
        _spiceToNextLevel.SetValue(Creature.NextLevelSpiceRequired());

        _levelBar = new(Color.gray,
                $"Chance to evolve on level up: {Creature.ChanceToEvolve(Creature.Level + 1)}",
                _currentSpice, _spiceToNextLevel);
        _middlePanel.Add(_levelBar);

        _levelLabel = new Label($"Level {Creature.Level}");
        _levelLabel.AddToClassList(_ussCommonTextPrimary);
        _levelLabel.style.position = Position.Absolute;
        _levelBar.Add(_levelLabel);

        _levelUpButton = new("", _ussLevelUpButton, LevelUp);
        _buttonSpice = new(_spiceToNextLevel.Value);
        _levelUpButton.Add(_buttonSpice);
        _middlePanel.Add(_levelUpButton);
    }

    void LevelUp()
    {
        if (_gameManager.Spice < _spiceToNextLevel.Value)
        {
            Helpers.DisplayTextOnElement(Helpers.GetRoot(this), _levelUpButton, "Not enough spice", Color.red);
            return;
        }

        _levelUpButton.SetEnabled(false);
        _gameManager.ChangeSpiceValue(-_spiceToNextLevel.Value);
        _currentSpice.ApplyChange(_spiceToNextLevel.Value);

        Creature.LevelUp();
        _levelLabel.text = $"Level {Creature.Level}";

        _levelBar.OnAnimationFinished += () =>
        {
            _levelBar.ChangeValueNoAnimation(0);
            _spiceToNextLevel.SetValue(Creature.NextLevelSpiceRequired());

            _buttonSpice.ChangeAmount(_spiceToNextLevel.Value);
            _levelUpButton.SetEnabled(true);
        };
    }

    public void FoldSelf()
    {
        CreatureIcon.SmallIcon();
        AddToClassList(_ussMainFolded);
        DOTween.To(x => _name.style.opacity = x, 1, 0, 0.5f)
            .OnComplete(() => _name.style.display = DisplayStyle.None);
        DOTween.To(x => _middlePanel.style.opacity = x, 1, 0, 0.5f)
            .OnComplete(() => _middlePanel.style.display = DisplayStyle.None);
    }
}
