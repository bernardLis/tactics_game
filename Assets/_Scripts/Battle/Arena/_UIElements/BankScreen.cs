using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class BankScreen : RewardScreen
    {
        const string _ussInvestmentContainer = USSClassName + "investment-container";
        const string _ussInvestmentTitle = USSClassName + "investment-title";

        Bank _bank;

        VisualElement _availableInvestmentContainer;
        VisualElement _activeInvestmentContainer;

        VisualElement _investmentContainer;

        public void InitializeBank(Bank bank)
        {
            _bank = bank;

            _investmentContainer = new();
            _investmentContainer.style.width = Length.Percent(100);
            _investmentContainer.style.flexDirection = FlexDirection.Row;
            Content.Add(_investmentContainer);

            _availableInvestmentContainer = new();
            _availableInvestmentContainer.AddToClassList(_ussInvestmentContainer);
            _investmentContainer.Add(_availableInvestmentContainer);

            _activeInvestmentContainer = new();
            _activeInvestmentContainer.AddToClassList(_ussInvestmentContainer);
            _investmentContainer.Add(_activeInvestmentContainer);

            AddAvailableInvestments();
            AddActiveInvestments();
            SetTitle("Bank");

            AddContinueButton();
        }

        void AddAvailableInvestments()
        {
            Label title = new("Available Investments");
            title.AddToClassList(_ussInvestmentTitle);
            _availableInvestmentContainer.Add(title);

            foreach (Investment i in _bank.AvailableInvestments)
            {
                InvestmentElement investmentElement = new(i);
                _availableInvestmentContainer.Add(investmentElement);
                i.OnStarted += () => _activeInvestmentContainer.Add(investmentElement);
            }
        }

        void AddActiveInvestments()
        {
            Label title = new("Active Investments");
            title.AddToClassList(_ussInvestmentTitle);
            _activeInvestmentContainer.Add(title);

            foreach (Investment i in _bank.ActiveInvestments)
            {
                InvestmentElement investmentElement = new(i);
                _activeInvestmentContainer.Add(investmentElement);
                i.OnCollected += () => _activeInvestmentContainer.Remove(investmentElement);
            }
        }
    }
}