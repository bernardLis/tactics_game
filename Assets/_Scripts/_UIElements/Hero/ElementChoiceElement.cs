using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementChoiceElement : VisualElement
{
    const string _ussClassName = "element-choice__";
    const string _ussMain = _ussClassName + "main";
    const string _ussElementIcon = _ussClassName + "element-icon";

    GameManager _gameManager;

    ElementalElement _fireElement;
    ElementalElement _earthElement;
    ElementalElement _windElement;
    ElementalElement _waterElement;

    public event Action<Element> OnElementChosen;
    public ElementChoiceElement()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ElementChoiceStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        Label header = new("Choose your element:");
        Add(header);

        VisualElement elementContainer = new();
        elementContainer.style.flexDirection = FlexDirection.Row;
        Add(elementContainer);

        Element fire = _gameManager.HeroDatabase.GetElementByName(ElementName.Fire);
        Element earth = _gameManager.HeroDatabase.GetElementByName(ElementName.Earth);
        Element wind = _gameManager.HeroDatabase.GetElementByName(ElementName.Wind);
        Element water = _gameManager.HeroDatabase.GetElementByName(ElementName.Water);

        _fireElement = new(fire, 200);
        _earthElement = new(earth, 200);
        _windElement = new(wind, 200);
        _waterElement = new(water, 200);

        _fireElement.AddToClassList(_ussElementIcon);
        _earthElement.AddToClassList(_ussElementIcon);
        _windElement.AddToClassList(_ussElementIcon);
        _waterElement.AddToClassList(_ussElementIcon);

        _fireElement.RegisterCallback<MouseEnterEvent>(evt => _fireElement.PlayEffect());
        _earthElement.RegisterCallback<MouseEnterEvent>(evt => _earthElement.PlayEffect());
        _windElement.RegisterCallback<MouseEnterEvent>(evt => _windElement.PlayEffect());
        _waterElement.RegisterCallback<MouseEnterEvent>(evt => _waterElement.PlayEffect());

        _fireElement.RegisterCallback<MouseUpEvent>(evt => ElementChosen(fire));
        _earthElement.RegisterCallback<MouseUpEvent>(evt => ElementChosen(earth));
        _windElement.RegisterCallback<MouseUpEvent>(evt => ElementChosen(wind));
        _waterElement.RegisterCallback<MouseUpEvent>(evt => ElementChosen(water));

        elementContainer.Add(_fireElement);
        elementContainer.Add(_earthElement);
        elementContainer.Add(_windElement);
        elementContainer.Add(_waterElement);
    }

    void ElementChosen(Element element)
    {
        _fireElement.SetEnabled(false);
        _earthElement.SetEnabled(false);
        _windElement.SetEnabled(false);
        _waterElement.SetEnabled(false);

        _fireElement.DisableEffect();
        _earthElement.DisableEffect();
        _windElement.DisableEffect();
        _waterElement.DisableEffect();

        if (element.ElementName == ElementName.Fire)
            _fireElement.SetEnabled(true);
        if (element.ElementName == ElementName.Earth)
            _earthElement.SetEnabled(true);
        if (element.ElementName == ElementName.Wind)
            _windElement.SetEnabled(true);
        if (element.ElementName == ElementName.Water)
            _waterElement.SetEnabled(true);

        OnElementChosen?.Invoke(element);
    }
}
