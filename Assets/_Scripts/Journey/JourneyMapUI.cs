using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class JourneyMapUI : MonoBehaviour
{
    GameManager _gameManager;
    RunManager _runManager;
    JourneyMapManager _journeyMapManager;

    VisualElement _root;
    VisualElement _currencyContainer;
    VisualElement _viewTroopsContainer;
    VisualElement _nodeInfo;

    MyButton _viewTroopsButton;

    [Header("Unity Setup")]
    [SerializeField] GameObject obolObject;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _runManager = RunManager.Instance;
        _journeyMapManager = JourneyMapManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _currencyContainer = _root.Q<VisualElement>("currencyContainer");
        _viewTroopsContainer = _root.Q<VisualElement>("viewTroopsContainer");
        _nodeInfo = _root.Q<VisualElement>("nodeInfo");

        _viewTroopsButton = new("View Troops", "menuButton", ViewTroopsClick);
        _viewTroopsContainer.Add(_viewTroopsButton);

        PopulateCurrencyContainer();
        _gameManager.OnObolsChanged += HandleCurrencyChanges;
        _runManager.OnGoldChanged += HandleCurrencyChanges;
        _runManager.OnSavingsAccountChanged += HandleCurrencyChanges;
    }

    public void ShowNodeInfo(JourneyNode node)
    {
        _nodeInfo.Clear();
        _nodeInfo.Add(new JourneyNodeInfoVisual(node));
    }

    public void HideNodeInfo()
    {
        _nodeInfo.Clear();
    }

    void ViewTroopsClick()
    {
        _gameManager.GetComponent<GameUIManager>().ShowTroopsScreenNoContext();
    }

    void HandleCurrencyChanges(int i)
    {
        PopulateCurrencyContainer();
    }

    void PopulateCurrencyContainer()
    {
        _currencyContainer.Clear();

        Label obols = new Label($"Obols: {_gameManager.Obols}");
        Label gold = new Label($"Gold: {_runManager.Gold}");
        Label savings = new Label($"Savings: {_runManager.SavingsAccountGold}");

        _currencyContainer.Add(obols);
        _currencyContainer.Add(gold);
        _currencyContainer.Add(savings);
    }

}
