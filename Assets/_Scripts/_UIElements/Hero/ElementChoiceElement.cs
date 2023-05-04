using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementChoiceElement : VisualElement
{
    GameManager _gameManager;

    public ElementChoiceElement()
    {
        GameManager _gameManager = GameManager.Instance;


        Label header = new("Choose your element:");
        Add(header);

        VisualElement elementContainer = new();
        elementContainer.style.flexDirection = FlexDirection.Row;
        Add(elementContainer);

        ElementalElement fire = new(_gameManager.HeroDatabase.GetElementByName(ElementName.Fire));
        ElementalElement earth = new(_gameManager.HeroDatabase.GetElementByName(ElementName.Earth));
        ElementalElement wind = new(_gameManager.HeroDatabase.GetElementByName(ElementName.Wind));
        ElementalElement water = new(_gameManager.HeroDatabase.GetElementByName(ElementName.Water));

        fire.RegisterCallback<MouseEnterEvent>(evt => fire.PlayEffect());
        earth.RegisterCallback<MouseEnterEvent>(evt => earth.PlayEffect());
        wind.RegisterCallback<MouseEnterEvent>(evt => wind.PlayEffect());
        water.RegisterCallback<MouseEnterEvent>(evt => water.PlayEffect());

        elementContainer.Add(fire);
        elementContainer.Add(earth);
        elementContainer.Add(wind);
        elementContainer.Add(water);
    }
}
