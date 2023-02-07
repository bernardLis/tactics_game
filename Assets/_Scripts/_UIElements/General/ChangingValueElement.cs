using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangingValueElement : ElementWithTooltip
{
    public int Amount;

    protected Label _text;
    protected int _currentlyDisplayedAmount;
    protected IVisualElementScheduledItem _amountChangeScheduler;

    public event Action OnAnimationFinished;

    public void ChangeAmount(int newValue)
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
