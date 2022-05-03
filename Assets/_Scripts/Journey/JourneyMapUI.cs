using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class JourneyMapUI : MonoBehaviour
{
    GameManager _gameManager;
    JourneyMapManager _journeyMapManager;

    VisualElement _root;
    Label _playerName;
    Label _currencyAmount;
    VisualElement _nodeInfo;
    Label _nodeType;
    Label _nodeObols;
    FullScreenVisual _viewTroopsContainer;
    Button _viewTroopsButton;

    [Header("Unity Setup")]
    [SerializeField] GameObject obolObject;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _journeyMapManager = JourneyMapManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _playerName = _root.Q<Label>("playerName");
        _currencyAmount = _root.Q<Label>("currencyAmount");

        _nodeInfo = _root.Q<VisualElement>("nodeInfo");
        _nodeType = _root.Q<Label>("nodeType");
        _nodeObols = _root.Q<Label>("nodeObols");

        _viewTroopsButton = _root.Q<Button>("viewTroops");
        _viewTroopsButton.clickable.clicked += ViewTroopsClick;
    }

    void Start()
    {
        _playerName.text = "Welcome King " + _gameManager.PlayerName;
        _currencyAmount.text = _gameManager.Obols.ToString();
    }

    public void ChangeObols(int start, int end)
    {
        StartCoroutine(ChangeObolsCoroutine(start, end));
    }

    IEnumerator ChangeObolsCoroutine(int start, int end)
    {
        int current = start;
        int amount = Mathf.Abs(end - start);
        // TODO: there must be a better way
        for (int i = 0; i < amount; i++)
        {
            if (end > start)
            {
                current++;
                SpawnObol(1);
            }
            else
            {
                if (current - 1 < 0)
                    yield break;

                current--;

                SpawnObol(-1);
            }

            yield return new WaitForSeconds(0.1f);
            _currencyAmount.text = current.ToString();
        }
    }

    void SpawnObol(int dir)
    {
        Vector3 spawnPos = _journeyMapManager.CurrentNode.GameObject.transform.position;
        Vector3 travelToPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
        if (dir == -1)
        {
            // if we are subtracting obols, they should be flying the other way
            spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
            travelToPos = _journeyMapManager.CurrentNode.GameObject.transform.position;
        }

        GameObject g = Instantiate(obolObject, spawnPos, Quaternion.identity);
        g.transform.localScale = Vector3.one * 10;
        g.transform.DOMove(travelToPos, 1f);
        Destroy(g, 1.1f);
    }

    public void ShowNodeInfo(JourneyNode node)
    {
        _nodeInfo.style.visibility = Visibility.Visible;
        _nodeType.text = node.NodeType.ToString();
    }

    public void HideNodeInfo()
    {
        _nodeInfo.style.visibility = Visibility.Hidden;
    }

    void ViewTroopsClick()
    {
        _viewTroopsContainer = new FullScreenVisual();
        _viewTroopsContainer.Initialize(_root);

        foreach (Character character in _gameManager.PlayerTroops)
        {
            CharacterCardVisual card = new CharacterCardVisual(character);
            _viewTroopsContainer.Add(card);
        }
    }

}
