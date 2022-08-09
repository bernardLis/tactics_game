using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class GlobalUpgradeShopManager : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;
    VisualElement _menuContainer;
    VisualElement _shopContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;
        _menuContainer = _root.Q<VisualElement>("menuContainer");
        _shopContainer = _root.Q<VisualElement>("shopContainer");

        PopulateShop();
        AddBackButton();
    }

    void PopulateShop()
    {
        _shopContainer.Clear();
        foreach (GlobalUpgrade upgrade in _gameManager.AllGlobalUpgrades)
        {
            bool isPurchased = _gameManager.IsGlobalUpgradePurchased(upgrade);
            GlobalUpgradeVisual visual = new(upgrade, isPurchased);
            visual.RegisterCallback<PointerDownEvent>(BuyUpgrade);

            _shopContainer.Add(visual);
        }
    }

    void BuyUpgrade(PointerDownEvent evt)
    {
        GlobalUpgradeVisual visual = (GlobalUpgradeVisual)evt.currentTarget;
        if (_gameManager.Obols < visual.Upgrade.Price)
        {
            DisplayText(visual, "Insufficient funds", Color.red);
            return;
        }

        _gameManager.PurchaseGlobalUpgrade(visual.Upgrade);
        visual.PurchaseUpgrade();
    }

    void AddBackButton()
    {
        Button b = new Button();
        b.text = "Back";
        b.AddToClassList("menuButton");
        b.clickable.clicked += BackToMenu;
        _shopContainer.Add(b);
    }

    void BackToMenu()
    {
        _menuContainer.style.display = DisplayStyle.Flex;
        _shopContainer.style.display = DisplayStyle.None;
    }

    async void DisplayText(VisualElement element, string text, Color color)
    {
        Label l = new Label(text);
        l.AddToClassList("textSecondary");
        l.style.color = color;
        l.style.position = Position.Absolute;
        l.style.left = element.worldBound.xMin + element.worldBound.width / 2;
        l.style.top = element.worldBound.yMax;

        _root.Add(l);
        float end = element.worldBound.yMin + 100;
        await DOTween.To(x => l.style.top = x, element.worldBound.yMax, end, 1).SetEase(Ease.OutSine).AsyncWaitForCompletion();
        await DOTween.To(x => l.style.opacity = x, 1, 0, 1).AsyncWaitForCompletion();
        _root.Remove(l);
    }

}
