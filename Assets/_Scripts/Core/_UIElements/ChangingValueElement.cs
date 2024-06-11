using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ChangingValueElement : ElementWithTooltip
    {
        protected IVisualElementScheduledItem _amountChangeScheduler;
        protected int _currentlyDisplayedAmount;

        protected Label _text;
        public int Amount;

        public event Action OnAnimationFinished;

        public void Initialize(int amount, int fontSize)
        {
            Amount = amount;
            _currentlyDisplayedAmount = amount;
            _text = new(amount.ToString());
            _text.style.fontSize = fontSize;
            Add(_text);
        }

        public virtual void ChangeAmount(int newValue)
        {
            if (newValue == Amount)
                return;

            _currentlyDisplayedAmount = Amount;
            Amount = newValue;

            if (_amountChangeScheduler != null)
                _amountChangeScheduler.Pause();

            _amountChangeScheduler = schedule.Execute(NumberAnimation).Every(5);
        }

        protected virtual void NumberAnimation()
        {
            if (_currentlyDisplayedAmount == Amount)
                FinishAnimation();

            int currentDiff = Mathf.Abs(_currentlyDisplayedAmount - Amount);
            int multiplier = 1 + Mathf.FloorToInt(currentDiff / 100);

            if (_currentlyDisplayedAmount < Amount)
                _currentlyDisplayedAmount += 1 * multiplier;
            if (_currentlyDisplayedAmount > Amount)
                _currentlyDisplayedAmount -= 1 * multiplier;

            _text.text = _currentlyDisplayedAmount.ToString();
        }

        protected virtual void FinishAnimation()
        {
            if (_amountChangeScheduler != null)
                _amountChangeScheduler.Pause();

            OnAnimationFinished?.Invoke();
        }
    }
}