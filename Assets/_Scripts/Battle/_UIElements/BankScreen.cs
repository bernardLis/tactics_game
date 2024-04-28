using Lis.Battle.Arena;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class BankScreen : RewardScreen
    {
        Bank _bank;

        VisualElement _activeInvestmentContainer;

        VisualElement _investmentContainer;

        public void InitializeBank(Bank bank)
        {
            _bank = bank;

            Title = "Bank";
            AddTitle();
            AddRerollButton();
            AddHeroElement();
            AddHeroGoldElement();

            _investmentContainer = new();
            _investmentContainer.style.flexDirection = FlexDirection.Row;
            Content.Add(_investmentContainer);

            AddAvailableInvestments();
            AddActiveInvestments();
        }

        void AddAvailableInvestments()
        {
            VisualElement container = new();
            container.Add(new Label("Available Investments"));
            _investmentContainer.Add(container);

            foreach (Investment i in _bank.AvailableInvestments)
            {
                InvestmentElement investmentElement = new(i);
                container.Add(investmentElement);
                i.OnStarted += () => _activeInvestmentContainer.Add(investmentElement);
            }
        }

        void AddActiveInvestments()
        {
            _activeInvestmentContainer = new();
            _activeInvestmentContainer.Add(new Label("Active Investments"));
            _investmentContainer.Add(_activeInvestmentContainer);

            foreach (Investment i in _bank.ActiveInvestments)
                _activeInvestmentContainer.Add(new InvestmentElement(i));
        }
    }
}