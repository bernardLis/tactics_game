using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core.Utilities
{
    public class Test : MonoBehaviour
    {
        private Button _rankElementTests;
        private VisualElement _root;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _rankElementTests = _root.Q<Button>("rankElementTests");
            _rankElementTests.clickable.clicked += AddRankElementTest;
        }

        private void AddRankElementTest()
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