using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
public class VillageManager : Singleton<VillageManager>
{
    ShopManager _shopManager;
    BankManager _bankManager;

    VisualElement _root;
    VisualElement _mainScreen;
    VisualElement _backToJourneyContainer;
    VisualElement _shop;
    VisualElement _bank;

    protected override void Awake()
    {
        base.Awake();

        _shopManager = GetComponent<ShopManager>();
        _bankManager = GetComponent<BankManager>();

        _root = GetComponent<UIDocument>().rootVisualElement;
        _mainScreen = _root.Q<VisualElement>("mainScreen");

        VisualElement shopPicture = _root.Q<VisualElement>("shopPicture");
        _shop = _root.Q<VisualElement>("shop");

        VisualElement bankPicture = _root.Q<VisualElement>("bankPicture");
        _bank = _root.Q<VisualElement>("bank");

        _backToJourneyContainer = _root.Q<VisualElement>("backToJourneyContainer");
        Button b = _root.Q<Button>("backToJourney");
        b.clickable.clicked += BackToJourney;

        shopPicture.RegisterCallback<MouseDownEvent>(ShowShop);
        bankPicture.RegisterCallback<MouseDownEvent>(ShowBank);

    }

    void ShowShop(MouseDownEvent evt)
    {
        HideVillage();
        _shop.style.display = DisplayStyle.Flex;
        _shopManager.Initialize();
    }

    void ShowBank(MouseDownEvent evt)
    {
        HideVillage();
        _bank.style.display = DisplayStyle.Flex;
        _bankManager.Initialize();

    }

    void HideVillage()
    {
        _backToJourneyContainer.style.display = DisplayStyle.None;
        _mainScreen.style.display = DisplayStyle.None;
    }

    public void ShowVillage()
    {
        _shop.style.display = DisplayStyle.None;
        _bank.style.display = DisplayStyle.None;
        _backToJourneyContainer.style.display = DisplayStyle.Flex;
        _mainScreen.style.display = DisplayStyle.Flex;
    }

    void BackToJourney()
    {
        GameManager.Instance.LoadLevel(Scenes.Journey);
    }

    public async void DisplayText(VisualElement element, string text, Color color)
    {
        Label l = new Label(text);
        l.AddToClassList("textSecondary");
        l.style.color = color;
        l.style.position = Position.Absolute;
        l.style.left = element.worldBound.xMin - element.worldBound.width / 2;
        l.style.top = element.worldBound.yMax;

        _root.Add(l);
        float end = element.worldBound.yMin + 100;
        await DOTween.To(x => l.style.top = x, element.worldBound.yMax, end, 1).SetEase(Ease.OutSine).AsyncWaitForCompletion();
        await DOTween.To(x => l.style.opacity = x, 1, 0, 1).AsyncWaitForCompletion();
        _root.Remove(l);
    }

}
