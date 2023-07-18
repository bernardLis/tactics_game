using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class StoreyTroopsElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "storey-troops__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";
    const string _ussUpgradesTitle = _ussClassName + "upgrades-title";
    const string _ussUpgradesContainer = _ussClassName + "upgrades-container";

    GameManager _gameManager;

    VisualElement _content;


    StoreyTroops _storeyTroops;

    public event Action OnClosed;
    public StoreyTroopsElement(StoreyTroops storeyTroops)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyTroopsStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _storeyTroops = storeyTroops;

        _content = new();
        _content.AddToClassList(_ussContent);
        Add(_content);

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }



    void Close()
    {
        DOTween.To(x => style.opacity = x, style.opacity.value, 0, 0.5f).SetUpdate(true);
        DOTween.To(x => _content.style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() =>
                {
                    OnClosed?.Invoke();
                    RemoveFromHierarchy();
                });
    }

}
