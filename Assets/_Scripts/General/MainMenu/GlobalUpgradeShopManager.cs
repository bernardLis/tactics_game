using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalUpgradeShopManager : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;
    VisualElement _shopContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;
        _shopContainer = _root.Q<VisualElement>("shopContainer");

        PopulateShop();
    }

    void PopulateShop()
    {
        _shopContainer.Clear();
        foreach (GlobalUpgrade upgrade in _gameManager.AllGlobalUpgrades)
        {


        }
    }

}
