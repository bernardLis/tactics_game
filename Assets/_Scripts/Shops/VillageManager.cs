using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VillageManager : Singleton<VillageManager>
{
    ShopManager _shopManager;

    VisualElement _root;
    VisualElement _mainScreen;
    VisualElement _backToJourneyContainer;
    VisualElement _shop;
    protected override void Awake()
    {
        base.Awake();

        _shopManager = GetComponent<ShopManager>();

        _root = GetComponent<UIDocument>().rootVisualElement;
        _mainScreen = _root.Q<VisualElement>("mainScreen");

        VisualElement shopPicture = _root.Q<VisualElement>("shopPicture");
        _shop = _root.Q<VisualElement>("shop");

        _backToJourneyContainer = _root.Q<VisualElement>("backToJourneyContainer");
        Button b = _root.Q<Button>("backToJourney");
        b.clickable.clicked += BackToJourney;

        shopPicture.RegisterCallback<MouseDownEvent>(ShowShop);
    }

    void ShowShop(MouseDownEvent evt)
    {
        HideVillage();
        _shop.style.display = DisplayStyle.Flex;
        _shopManager.Initialize();
    }

    void HideVillage()
    {
        _backToJourneyContainer.style.display = DisplayStyle.None;
        _mainScreen.style.display = DisplayStyle.None;
    }

    public void ShowVillage()
    {
        _shop.style.display = DisplayStyle.None;
        _backToJourneyContainer.style.display = DisplayStyle.Flex;
        _mainScreen.style.display = DisplayStyle.Flex;
    }

    void BackToJourney()
    {
        GameManager.Instance.LoadLevel(Scenes.Journey);
    }

}
