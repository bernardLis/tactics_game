using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class JourneyMapUI : MonoBehaviour
{
    JourneyManager _journeyManager;
    JourneyMapManager _journeyMapManager;

    Label _currencyAmount;
    VisualElement _nodeInfo;
    Label _nodeType;
    Label _nodeObols;

    [Header("Unity Setup")]
    [SerializeField] GameObject obolObject;



    void Awake()
    {
        _journeyManager = JourneyManager.Instance;
        _journeyMapManager = JourneyMapManager.Instance;

        var root = GetComponent<UIDocument>().rootVisualElement;
        _currencyAmount = root.Q<Label>("currencyAmount");

        _nodeInfo = root.Q<VisualElement>("nodeInfo");
        _nodeType = root.Q<Label>("nodeType");
        _nodeObols = root.Q<Label>("nodeObols");
    }

    void Start()
    {
        _currencyAmount.text = _journeyManager.Obols.ToString();
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

}
