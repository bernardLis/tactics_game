using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CastleElement : FullScreenElement
{
    GameManager _gameManager;
    Castle _castle;
    public CastleElement(VisualElement root, Castle castle)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CastleElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root, false);

        _castle = castle;

        AddBuildings();
        AddBackButton();
    }


    void AddBuildings()
    {
        foreach (Building b in _castle.Buildings)
        {
            BuildingElement e = new(b);
            Add(e);
        }
    }
}
