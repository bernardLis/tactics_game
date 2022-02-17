using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class JourneyMapUI : MonoBehaviour
{
    JourneyManager journeyManager;
    JourneyMapManager journeyMapManager;
    UIDocument UIDocument;
    Label currencyAmount;
    VisualElement nodeInfo;
    Label nodeType;
    Label nodeObols;

    [Header("Unity Setup")]
    public GameObject obolObject;



    void Awake()
    {
        journeyManager = JourneyManager.instance; 
        journeyMapManager = JourneyMapManager.instance;
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        currencyAmount = root.Q<Label>("currencyAmount");

        nodeInfo = root.Q<VisualElement>("nodeInfo");
        nodeType = root.Q<Label>("nodeType");
        nodeObols = root.Q<Label>("nodeObols");
    }

    void Start()
    {
        currencyAmount.text = journeyManager.obols.ToString();
    }

    public void ChangeObols(int _start, int _end)
    {
        StartCoroutine(ChangeObolsCoroutine(_start, _end));
    }

    IEnumerator ChangeObolsCoroutine(int _start, int _end)
    {
        int current = _start;
        int amount = Mathf.Abs(_end - _start);
        // TODO: there must be a better way
        for (int i = 0; i < amount; i++)
        {
            if (_end > _start)
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
            currencyAmount.text = current.ToString();
        }
    }

    void SpawnObol(int _dir)
    {
        Vector3 spawnPos = journeyMapManager.currentNode.gameObject.transform.position;
        Vector3 travelToPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
        if (_dir == -1)
        {
            // if we are subtracting obols, they should be flying the other way
            spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
            travelToPos = journeyMapManager.currentNode.gameObject.transform.position;
        }

        GameObject g = Instantiate(obolObject, spawnPos, Quaternion.identity);
        g.transform.DOMove(travelToPos, 1f);
        Destroy(g, 1.1f);
    }

    public void ShowNodeInfo(JourneyNode _node)
    {
        nodeInfo.style.visibility = Visibility.Visible;
        nodeType.text = _node.nodeType.ToString();
    }

    public void HideNodeInfo()
    {
        nodeInfo.style.visibility = Visibility.Hidden;
    }

}
