using Lis.Battle.Arena;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class InvestmentElement : VisualElement
    {
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "investment-element__";
        const string _ussMain = _ussClassName + "main";


        readonly Investment _investment;

        MyButton _collectButton;
        MyButton _investButton;

        public InvestmentElement(Investment investment)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.InvestmentElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);

            _investment = investment;

            Add(new Label($"Invest {investment.GoldToInvest} for {investment.FightCount} fights."));
            Add(new Label($"Collect {investment.GoldToReturn} gold."));
            Add(new Label($"Interest: {Mathf.RoundToInt(investment.Interest * 100)}%"));

            if (_investment.IsActive) HandleActiveInvestment();
            else HandleAvailableInvestment();
        }

        void HandleActiveInvestment()
        {
            Add(new Label($"Fights remaining: {_investment.FightsRemaining}."));

            _collectButton = new("Collect", _ussCommonButton, Collect);
            _collectButton.style.width = 140;
            _collectButton.style.minHeight = 60;

            Add(_collectButton);

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
            Add(_investButton);
        }

        void Invest()
        {
            _investment.StartInvestment();
            _investButton.RemoveFromHierarchy();
            HandleActiveInvestment();
        }
    }
}