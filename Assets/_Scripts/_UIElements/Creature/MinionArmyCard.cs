using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinionArmyCard : VisualElement
{
    GameManager _gameManager;

    public Minion Minion;
    public int Count;

    public MinionArmyCard(Minion minion, int count)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);

        Minion = minion;
        Count = count;
        
        // HERE: minions should be addable to army with count not level


        //CreatureIcon icon = new(creature);
        Label countLabel = new Label($"Count: {count}");
        //   Add(icon);
        Add(countLabel);
    }
}
