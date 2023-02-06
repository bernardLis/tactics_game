using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    VisualElement _root;
    Button _rankElementTests;

    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _rankElementTests = _root.Q<Button>("rankElementTests");
        _rankElementTests.clickable.clicked += AddRankElementTest;
    }

    void AddRankElementTest()
    {
        VisualElement container = new();
        StarRankElement starRankElement = new StarRankElement(0, 1);
        container.Add(starRankElement);
        MyButton addRank = new("add rank", null, () => starRankElement.SetRank(starRankElement.Rank + 1));
        MyButton subtractRank = new("subtract rank ", null, () => starRankElement.SetRank(starRankElement.Rank - 1));
        container.Add(addRank);
        container.Add(subtractRank);

        _root.Add(container);
    }

}
