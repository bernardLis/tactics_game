using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingElement : VisualElement
{
    GameManager _gameManager;
    Building _building;
    public BuildingElement(Building building)
    {
        style.backgroundImage = new StyleBackground(building.OutlineSprite);
        style.width = 200;
        style.height = 200;
    }
}
