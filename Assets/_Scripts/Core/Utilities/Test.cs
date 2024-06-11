using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core.Utilities
{
    public class Test : MonoBehaviour
    {
        Button _rankElementTests;
        VisualElement _root;

        void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _rankElementTests = _root.Q<Button>("rankElementTests");
            _rankElementTests.clickable.clicked += AddRankElementTest;
        }

        void AddRankElementTest()
        {
            VisualElement container = new();
            StarRankElement starRankElement = new(0);
            container.Add(starRankElement);
            MyButton addRank = new("add rank", null, () => starRankElement.SetRank(starRankElement.Rank + 1));
            MyButton subtractRank =
                new("subtract rank ", null, () => starRankElement.SetRank(starRankElement.Rank - 1));
            container.Add(addRank);
            container.Add(subtractRank);

            _root.Add(container);
        }
    }
}