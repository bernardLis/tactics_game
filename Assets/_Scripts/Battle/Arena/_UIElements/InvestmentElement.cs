using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class InvestmentElement : VisualElement
    {
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "investment-element__";
        const string _ussMain = _ussClassName + "main";


        VisualElement _leftContainer;
        VisualElement _rightContainer;

        readonly Investment _investment;

        VisualElement _firstLineContainer;

        MyButton _collectButton;
        MyButton _investButton;

        public InvestmentElement(Investment investment)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.InvestmentElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);

            _leftContainer = new();
            _rightContainer = new();
            Add(_leftContainer);
            Add(_rightContainer);

            _investment = investment;
            AddFirstLine();
            AddSecondLine();

            if (_investment.IsActive) HandleActiveInvestment();
            else HandleAvailableInvestment();
        }

        void AddFirstLine()
        {
            if (_investment.IsActive) return;

            _firstLineContainer = new();
            _firstLineContainer.style.flexDirection = FlexDirection.Row;
            _firstLineContainer.style.justifyContent = Justify.Center;
            _firstLineContainer.style.alignItems = Align.Center;
            _firstLineContainer.style.fontSize = 24;
            _leftContainer.Add(_firstLineContainer);

            _firstLineContainer.Add(new Label($"Invest "));
            _firstLineContainer.Add(new GoldElement(_investment.GoldToInvest));
            _firstLineContainer.Add(new Label($"for {_investment.FightCount}"));
            _firstLineContainer.Add(new FightIcon());
        }

        void AddSecondLine()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;
            _leftContainer.Add(container);

            container.Add(new Label($"Collect "));
            container.Add(new GoldElement(_investment.GoldToReturn));
            container.Add(new Label($"(Interest: {Mathf.RoundToInt(_investment.Interest * 100)}%)"));
        }

        void HandleActiveInvestment()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;
            _leftContainer.Add(container);
            container.Add(new FightIcon());
            container.Add(new Label($" remaining: {_investment.FightsRemaining}."));

            _collectButton = new("Collect", _ussCommonButton, Collect);
            _collectButton.style.width = 140;
            _collectButton.style.minHeight = 60;

            _rightContainer.Add(_collectButton);

            if (_investment.FightsRemaining <= 0) return;
            _collectButton.SetEnabled(false);
        }

        void Collect()
        {
            _investment.Collect();
            _collectButton.SetEnabled(false);
            _collectButton.SetText("Collected!");
        }

        void HandleAvailableInvestment()
        {
            _investButton = new("Invest", _ussCommonButton, Invest);
            _investButton.style.width = 140;
            _investButton.style.minHeight = 60;
            _rightContainer.Add(_investButton);
        }

        void Invest()
        {
            if (GameManager.Instance.Gold < _investment.GoldToInvest)
            {
                Helpers.DisplayTextOnElement(BattleManager.Instance.Root, _investButton,
                    "Not enough gold!", Color.red);
                return;
            }

            _investment.StartInvestment();
            _firstLineContainer.RemoveFromHierarchy();
            _investButton.RemoveFromHierarchy();
            HandleActiveInvestment();
        }
    }
}