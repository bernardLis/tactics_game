using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Fight
{
    public class FightOptionElement : VisualElement
    {
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "fight-option__";
        const string _ussMain = _ussClassName + "main";

        readonly MyButton _chooseButton;

        readonly GameManager _gameManager;

        readonly FightOption _option;

        public FightOptionElement(FightOption option)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.FightOptionElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _option = option;

            Label label = new("Fight Option");
            Add(label);

            GoldElement goldElement = new(option.GoldReward);
            Add(goldElement);

            ScrollView scrollView = new();
            Add(scrollView);
            List<VisualElement> aggregated = new(AggregateEnemies(option.Army));
            foreach (VisualElement el in aggregated)
                scrollView.Add(el);

            _chooseButton = new("Choose", _ussCommonButton, ChooseOption);
            Add(_chooseButton);
        }

        List<VisualElement> AggregateEnemies(List<Unit> army)
        {
            Dictionary<string, int> unitCountDict = new();

            foreach (Unit e in army)
                if (!unitCountDict.TryAdd(e.Id, 1))
                    unitCountDict[e.Id]++;

            List<VisualElement> result = new();
            foreach (var item in unitCountDict)
            {
                VisualElement container = new();
                container.style.flexDirection = FlexDirection.Row;
                result.Add(container);

                Label count = new(item.Value + "x");
                container.Add(count);

                Unit unit = _gameManager.UnitDatabase.GetUnitById(item.Key);
                Unit instance = Object.Instantiate(unit);
                instance.InitializeBattle(1);
                UnitIcon icon = new(instance);
                container.Add(icon);
            }

            return result;
        }

        void ChooseOption()
        {
            _option.Choose();
        }

        public void DisableSelf()
        {
            _chooseButton.SetEnabled(false);
            if (_option.IsChosen) return;
            SetEnabled(false);
        }
    }
}