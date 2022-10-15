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
        _backToJourneyContainer.Add(new MyButton("Back To Journey", "menuButton", BackToJourney));

        shopPicture.RegisterCallback<MouseDownEvent>(ShowShop);
        bankPicture.RegisterCallback<MouseDownEvent>(ShowBank);

    }

    void ShowShop(MouseDownEvent evt)
    {
        HideVillage();
        _shop.style.display = DisplayStyle.Flex;
       // _shopManager.Initialize();
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
        RunManager.Instance.VisitedJourneyNodes.Add(RunManager.Instance.CurrentNode.Serialize());
        GameManager.Instance.LoadLevel(Scenes.Journey);
    }
}
