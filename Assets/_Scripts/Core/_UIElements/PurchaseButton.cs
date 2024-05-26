using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class PurchaseButton : MyButton
    {
        protected readonly GoldElement GoldElement;

        public PurchaseButton(string text, string ussClassName, Action onClick, int price) : base(text, ussClassName,
            onClick)
        {
            style.width = 100;
            style.minHeight = 60;
            style.paddingLeft = 12;
            style.alignItems = Align.Center;

            GoldElement = new(price);
            Add(GoldElement);
        }

        public void ChangePrice(int amount)
        {
            GoldElement.ChangeAmount(amount);
        }

        public void RemovePrice()
        {
            GoldElement.RemoveFromHierarchy();
        }
    }
}