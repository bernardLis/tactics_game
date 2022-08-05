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

        _viewTroopsButton = _root.Q<Button>("viewTroops");
        _viewTroopsButton.clickable.clicked += ViewTroopsClick;
    }

    void Start()
    {
        _playerName.text = "Welcome King " + _gameManager.PlayerName;
        _currencyAmount.text = _gameManager.Gold.ToString();
    }

    public void ChangeGold(int start, int end)
    {
        StartCoroutine(ChangeGoldCoroutine(start, end));
    }

    IEnumerator ChangeGoldCoroutine(int start, int end)
    {
        int current = start;
        int amount = Mathf.Abs(end - start);
        // TODO: there must be a better way
        for (int i = 0; i < amount; i++)
        {
            if (end > start)
            {
                current++;
                SpawnGold(1);
            }
            else
            {
                if (current - 1 < 0)
                    yield break;

                current--;

                SpawnGold(-1);
            }

            yield return new WaitForSeconds(0.1f);
            _currencyAmount.text = current.ToString();
        }
    }

    void SpawnGold(int dir)
    {
        Vector3 spawnPos = _journeyMapManager.CurrentNode.GameObject.transform.position;
        Vector3 travelToPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
        if (dir == -1)
        {
            // if we are subtracting gold, they should be flying the other way
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
