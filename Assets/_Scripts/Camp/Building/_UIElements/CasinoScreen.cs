using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class CasinoScreen : RewardScreen
    {
        const string _ussColorPicker = USSClassName + "color-picker";
        const string _ussColorSelected = USSClassName + "color-selected";
        const string _ussBetAmountContainer = USSClassName + "bet-amount-container";


        Casino _casino;

        bool _isGreenSelected;

        SliderInt _betAmountSlider;
        int _betAmount;

        MyButton _spinButton;

        public void InitializeCasino(Casino casino)
        {
            _casino = casino;
            _isGreenSelected = true;
            SetTitle("Casino");

            AddColorSelectors();
            AddBetAmountSelector();

            AddSpinButton();
            AddContinueButton();
        }


        void AddColorSelectors()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            Content.Add(container);

            Button greenButton = new();
            Button orangeButton = new();
            container.Add(greenButton);
            container.Add(orangeButton);

            greenButton.AddToClassList(_ussColorPicker);
            greenButton.AddToClassList(_ussColorSelected);
            greenButton.style.backgroundColor = GameManager.GameDatabase.GetColorByName("Common").Primary;
            greenButton.text = "Green";
            greenButton.clickable.clicked += () =>
            {
                _isGreenSelected = true;
                orangeButton.RemoveFromClassList(_ussColorSelected);
                greenButton.AddToClassList(_ussColorSelected);
            };

            orangeButton.AddToClassList(_ussColorPicker);
            orangeButton.text = "Orange";
            orangeButton.style.backgroundColor = GameManager.GameDatabase.GetColorByName("Epic").Primary;
            orangeButton.clickable.clicked += () =>
            {
                _isGreenSelected = false;
                greenButton.RemoveFromClassList(_ussColorSelected);
                orangeButton.AddToClassList(_ussColorSelected);
            };
        }

        void AddBetAmountSelector()
        {
            VisualElement container = new();
            container.AddToClassList(_ussBetAmountContainer);
            Content.Add(container);

            _betAmountSlider = new(0, GameManager.Gold);
            _betAmountSlider.style.flexGrow = 1;
            container.Add(_betAmountSlider);

            Label betAmountLabel = new("Bet Amount: 0");
            container.Add(betAmountLabel);

            _betAmountSlider.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                betAmountLabel.text = $"Bet Amount: {evt.newValue}";
            });
        }

        void AddSpinButton()
        {
            _spinButton = new("Spin", USSCommonButton, Spin);
            Content.Add(_spinButton);
        }

        void Spin()
        {
            _casino.PlaceBet(_betAmountSlider.value, _isGreenSelected);
            Hide();
        }
    }
}