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
    VisualElement _accountButtonsContainer;
    VisualElement _savingsAccountContainer;
    VisualElement _totalInterestEarnedContainer;

    VisualElement _exchangeButtonsContainer;

    VisualElement _bankBackButtonContainer;

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
        _accountButtonsContainer = _root.Q<VisualElement>("accountButtonsContainer");
        _savingsAccountContainer = _root.Q<VisualElement>("savingsAccountContainer");
        _totalInterestEarnedContainer = _root.Q<VisualElement>("totalInterestEarnedContainer");

        _exchangeButtonsContainer = _root.Q<VisualElement>("exchangeButtonsContainer");

        _bankBackButtonContainer = _root.Q<VisualElement>("bankBackButtonContainer");

        PopulateAccounts();

        MyButton addToSavingsButton = new("Deposit", "menuButton", AddToSavings);
        MyButton withdrawFromSavingsButton = new("Withdraw", "menuButton", WithdrawFromSavings);
        addToSavingsButton.style.width = Length.Percent(90);
        withdrawFromSavingsButton.style.width = Length.Percent(90);
        _accountButtonsContainer.Add(addToSavingsButton);
        _accountButtonsContainer.Add(withdrawFromSavingsButton);

        MyButton buyGoldButton = new("Buy Gold", "menuButton", BuyGold);
        MyButton buyObolsButton = new("Buy Obols", "menuButton", BuyObols);
        _exchangeButtonsContainer.Add(buyGoldButton);
        _exchangeButtonsContainer.Add(buyObolsButton);

        _bankBackButtonContainer.Add(new MyButton("Back", "menuButton", Back));

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
        _totalInterestEarnedContainer.Clear();

        Label gold = new Label($"Gold: {_runManager.Gold}");
        Label obols = new Label($"Obols: {_gameManager.Obols}");
        Label savings = new Label($"Saved Gold: {_runManager.SavingsAccountGold}");
        Label interests = new Label($"Gold earned through interests: {_runManager.InterestEarned}");

        _currentAccountContainer.Add(gold);
        _currentAccountContainer.Add(obols);
        _savingsAccountContainer.Add(savings);
        _totalInterestEarnedContainer.Add(interests);
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
