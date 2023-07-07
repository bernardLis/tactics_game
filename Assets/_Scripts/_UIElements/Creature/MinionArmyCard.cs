using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinionArmyCard : VisualElement
{
    GameManager _gameManager;

    public Creature Creature;
    public int Count;

    public MinionArmyCard(Creature creature, int count)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);

        Creature = creature;
        Count = count;

        CreatureIcon icon = new(creature);
        Label countLabel = new Label($"Count: {count}");
        Add(icon);
        Add(countLabel);
    }
}
