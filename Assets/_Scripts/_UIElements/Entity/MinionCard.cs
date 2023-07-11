using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinionCard : VisualElement
{
    const string _ussClassName = "minion-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLeftContainer = _ussClassName + "left-container";
    const string _ussName = _ussClassName + "name";

    protected GameManager _gameManager;

    protected Minion _minion;

    protected VisualElement _leftContainer;
    protected VisualElement _middleContainer;
    protected VisualElement _rightContainer;

    protected ElementalElement _elementalElement;
    public EntityIcon EntityIcon;
    protected Label _nameLabel;
    protected Label _levelLabel;
    protected Label _healthLabel;
    protected Label _armor;
    protected Label _speed;

    public MinionCard(Minion minion)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.MinionCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _minion = minion;

        AddToClassList(_ussMain);

        _leftContainer = new();
        _leftContainer.AddToClassList(_ussLeftContainer);
        _middleContainer = new();
        _rightContainer = new();
        Add(_leftContainer);
        Add(_middleContainer);
        Add(_rightContainer);

        _elementalElement = new ElementalElement(_minion.Element);
        EntityIcon = new(_minion, true);
        _nameLabel = new Label();
        _nameLabel.AddToClassList(_ussName);

        _levelLabel = new Label($"Level: {_minion.Level}");

        _healthLabel = new Label($"Health:");
        _armor = new Label($"Armor:");
        _speed = new Label($"Speed:");

        _leftContainer.Add(_nameLabel);
        _leftContainer.Add(EntityIcon);
        _leftContainer.Add(_elementalElement);

        _middleContainer.Add(_levelLabel);
        _middleContainer.Add(_healthLabel);
        _middleContainer.Add(_armor);
        _middleContainer.Add(_speed);


        SetValues(_minion);
    }

    public void SetValues(Minion minion)
    {
        _minion = minion;

        EntityIcon.SetEntity(minion);
        _nameLabel.text = $"{minion.Name}";
        _healthLabel.text = $"Health: {minion.GetHealth()}";
        _armor.text = $"Armor: {minion.Armor}";
        _speed.text = $"Speed: {minion.Speed}";
    }

}
