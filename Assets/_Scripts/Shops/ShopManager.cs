using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopManager : Singleton<ShopManager>
{
    BlacksmithManager _blacksmithManager;

    VisualElement _root;
    VisualElement _shopsContainer;
    VisualElement _blacksmith;
    protected override void Awake()
    {
        base.Awake();

        _blacksmithManager = GetComponent<BlacksmithManager>();

        _root = GetComponent<UIDocument>().rootVisualElement;
        _shopsContainer = _root.Q<VisualElement>("shopsContainer");

        VisualElement blacksmithPicture = _root.Q<VisualElement>("blacksmithPicture");
        _blacksmith = _root.Q<VisualElement>("blacksmith");

        blacksmithPicture.RegisterCallback<MouseDownEvent>(ShowBlackSmith);
    }

    void ShowBlackSmith(MouseDownEvent evt)
    {
        HideShops();
        _blacksmith.style.display = DisplayStyle.Flex;
        _blacksmithManager.Initialize();
    }

    void HideShops()
    {
        _shopsContainer.style.display = DisplayStyle.None;
    }

    public void ShowShops()
    {
        _blacksmith.style.display = DisplayStyle.None;

        _shopsContainer.style.display = DisplayStyle.Flex;
    }

}
