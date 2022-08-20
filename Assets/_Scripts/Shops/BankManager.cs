using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BankManager : MonoBehaviour
{
    GameManager _gameManager;
    RunManager _runManager;
    VillageManager _villageManager;

    VisualElement _root;
    VisualElement _bank;

    VisualElement _currentAccountContainer;
    VisualElement _savingsAccountContainer;
    VisualElement _totalInterestEarnedContainer;

    bool _isInitialized;

    void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        if (_isInitialized)
            return;
        _isInitialized = true;

        _gameManager = GameManager.Instance;
        _runManager = RunManager.Instance;
        _villageManager = VillageManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _bank = _root.Q<VisualElement>("bank");
        _currentAccountContainer = _root.Q<VisualElement>("currentAccountContainer");
        _savingsAccountContainer = _root.Q<VisualElement>("savingsAccountContainer");
        _totalInterestEarnedContainer = _root.Q<VisualElement>("totalInterestEarnedContainer");

        PopulateAccounts();

        Button addToSavingsButton = _root.Q<Button>("addToSavingsButton");
        Button withdrawFromSavingsButton = _root.Q<Button>("withdrawFromSavingsButton");
        addToSavingsButton.clickable.clicked += AddToSavings;
        withdrawFromSavingsButton.clickable.clicked += WithdrawFromSavings;

        Button buyGoldButton = _root.Q<Button>("buyGoldButton");
        Button buyObolsButton = _root.Q<Button>("buyObolsButton");
        buyGoldButton.clickable.clicked += BuyGold;
        buyObolsButton.clickable.clicked += BuyObols;
        buyGoldButton.RegisterCallback<MouseEnterEvent>((evt) => PlayClick()); // HERE:
        buyObolsButton.RegisterCallback<MouseEnterEvent>((evt) => PlayClick());

        Button bankBackButton = _root.Q<Button>("bankBackButton");
        bankBackButton.clickable.clicked += Back;

        _runManager.OnGoldChanged += HandleCurrencyChange;
        _runManager.OnSavingsAccountChanged += HandleCurrencyChange;
        _gameManager.OnObolsChanged += HandleCurrencyChange;
    }

    void PlayClick()
    {
        AudioManager.Instance.PlaySFX("uiClick", Vector3.zero);
    }

    void HandleCurrencyChange(int ignoredValue)
    {
        PopulateAccounts();
    }

    void PopulateAccounts()
    {
        _currentAccountContainer.Clear();
        _savingsAccountContainer.Clear();

        Label gold = new Label($"Gold: {_runManager.Gold}");
        Label savings = new Label($"Saved Gold: {_runManager.SavingsAccountGold}");

        Label obols = new Label($"Obols: {_gameManager.Obols}");

        _currentAccountContainer.Add(gold);
        _currentAccountContainer.Add(obols);

        _savingsAccountContainer.Add(savings);
    }

    void AddToSavings()
    {
        if (_runManager.Gold > 0)
        {
            _runManager.ChangeGoldValue(-1);
            _runManager.ChangeSavingsAccountValue(1);
            return;
        }
        _villageManager.DisplayText(_currentAccountContainer, "Insufficient funds", Color.red);
    }

    void WithdrawFromSavings()
    {
        if (_runManager.SavingsAccountGold > 0)
        {
            _runManager.ChangeGoldValue(1);
            _runManager.ChangeSavingsAccountValue(-1);
            return;
        }
        _villageManager.DisplayText(_savingsAccountContainer, "Insufficient funds", Color.red);
    }

    void BuyGold()
    {
        if (_gameManager.Obols > 0)
        {
            _runManager.ChangeGoldValue(10);
            _gameManager.ChangeObolValue(-1);
            return;
        }

        _villageManager.DisplayText(_root.Q<Button>("buyGoldButton"), "Insufficient funds", Color.red);
        PlayClick();
    }

    void BuyObols()
    {
        if (_runManager.Gold >= 10)
        {
            _runManager.ChangeGoldValue(-10);
            _gameManager.ChangeObolValue(1);
            return;
        }

        _villageManager.DisplayText(_root.Q<Button>("buyObolsButton"), "Insufficient funds", Color.red);
        PlayClick();
    }

    void Back()
    {
        VillageManager.Instance.ShowVillage();
        _gameManager.SaveJsonData();
    }
}
