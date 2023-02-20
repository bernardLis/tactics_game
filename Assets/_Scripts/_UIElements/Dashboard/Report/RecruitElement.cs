using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecruitElement : VisualElement
{
    const string _ussClassName = "recruit-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussWageContainer = _ussClassName + "wage-container";

    GameManager _gameManager;
    Recruit _recruit;

    public RecruitElement(Recruit recruit)
    {
        _gameManager = GameManager.Instance;

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RecruitElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _recruit = recruit;

        AddToClassList(_ussMain);

        Add(new CharacterCardMini(recruit.Character));
        VisualElement wageContainer = new();
        wageContainer.AddToClassList(_ussWageContainer);
        wageContainer.Add(new Label("Expected weekly wage:"));
        wageContainer.Add(new GoldElement(recruit.Character.WeeklyWage.Value));
        Add(wageContainer);
    }
}
