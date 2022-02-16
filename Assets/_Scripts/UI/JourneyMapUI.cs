using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class JourneyMapUI : MonoBehaviour
{
    UIDocument UIDocument;
    Label currencyAmount;
    VisualElement nodeInfo;
    Label nodeType;
    Label nodeObols;

    [Header("Unity Setup")]
    public GameObject obolObject;



    void Awake()
    {
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        currencyAmount = root.Q<Label>("currencyAmount");

        nodeInfo = root.Q<VisualElement>("nodeInfo");
        nodeType = root.Q<Label>("nodeType");
        nodeObols = root.Q<Label>("nodeObols");
    }

    void Start()
    {
        currencyAmount.text = GetComponent<JourneyMapManager>().obols.ToString();
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
                current++;
            else
                current--;

            SpawnObol();
            yield return new WaitForSeconds(0.1f);
            currencyAmount.text = current.ToString();

        }
    }

    void SpawnObol()
    {

        Vector3 pos = JourneyMapManager.instance.currentNode.gameObject.transform.position;
        GameObject g = Instantiate(obolObject, pos, Quaternion.identity);
        Vector3 toPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
        g.transform.DOMove(toPos, 1f);
        Destroy(g, 1.1f);
    }

    public void ShowNodeInfo(JourneyNode _node)
    {
        nodeInfo.style.visibility = Visibility.Visible;
        nodeType.text = _node.nodeType.ToString();
        nodeObols.text = _node.nodeObols.ToString();
    }

    public void HideNodeInfo()
    {
        nodeInfo.style.visibility = Visibility.Hidden;
    }

}
