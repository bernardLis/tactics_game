using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class PurchaseButton : MyButton
    {
        readonly GoldElement _goldElement;

        public PurchaseButton(string text, string ussClassName, Action onClick, int price) : base(text, ussClassName,
            onClick)
        {
            style.width = 100;
            style.minHeight = 60;
            style.paddingLeft = 12;
            style.alignItems = Align.Center;

            _goldElement = new(price);
            Add(_goldElement);
        }

        public void ChangePrice(int amount)
        {
            _goldElement.ChangeAmount(amount);
        }

        public void RemovePrice()
        {
            _goldElement.RemoveFromHierarchy();
        }
    }
}