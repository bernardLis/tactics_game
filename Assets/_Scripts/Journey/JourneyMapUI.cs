using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class JourneyMapUI : MonoBehaviour
{
    GameManager _gameManager;
    JourneyMapManager _journeyMapManager;

    VisualElement _root;
    VisualElement _viewTroopsContainer;
    VisualElement _nodeInfo;

    MyButton _viewTroopsButton;

    [Header("Unity Setup")]
    [SerializeField] GameObject obolObject;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _journeyMapManager = JourneyMapManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _viewTroopsContainer = _root.Q<VisualElement>("viewTroopsContainer");
        _nodeInfo = _root.Q<VisualElement>("nodeInfo");

        _viewTroopsButton = new("View Troops", "menuButton", ViewTroopsClick);
        _viewTroopsContainer.Add(_viewTroopsButton);
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

}
