using System;
using Lis.Battle;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Core
{
    public class PurchaseButton : MyButton
    {
        const string _ussCommonPurchaseButton = "common__purchase-button";
        const string _ussCommonPurchased = "common__purchased";

        readonly GoldElement _costGoldElement;

        int _cost;
        bool _isInfinite;
        bool _isPurchaseBlocked;
        string _blockText;

        public event Action OnPurchased;
        public PurchaseButton(int cost, bool isInfinite = false, bool isPurchased = false,
            string buttonText = "", string className = _ussCommonPurchaseButton, Action callback = null)
            : base(buttonText, className, callback)
        {
            _cost = cost;
            _isInfinite = isInfinite;

            if (isPurchased)
            {
                SetEnabled(false);
                AddToClassList(_ussCommonPurchased);
                return;
            }

            _costGoldElement = new(cost);
            Add(_costGoldElement);

            if (_gameManager.Gold < cost)
                SetEnabled(false);

            _gameManager.OnGoldChanged += UpdateButton;
            clicked += Buy;
        }

        public void UpdateCost(int cost)
        {
            _cost = cost;
            _costGoldElement.ChangeAmount(cost);
            if (_gameManager.Gold < cost)
                SetEnabled(false);
        }

        public void NoMoreInfinity()
        {
            _isInfinite = false;
        }

        void Buy()
        {
            if (!CanBePurchased()) return;

            _gameManager.ChangeGoldValue(-_cost);

            if (_isInfinite) return;

            _gameManager.OnGoldChanged -= UpdateButton;
            AddToClassList(_ussCommonPurchased);
            SetEnabled(false);
            Remove(_costGoldElement);
            OnPurchased?.Invoke();
        }

        bool CanBePurchased()
        {
            if (_isPurchaseBlocked)
            {
                Helpers.DisplayTextOnElement(BattleManager.Instance.Root, this, _blockText, Color.red);
                return false;
            }

            if (_gameManager.Gold < _cost) return false;

            return true;
        }

        public void BlockPurchase(string blockText)
        {
            _blockText = blockText;
            _isPurchaseBlocked = true;
        }

        public void UnblockPurchase()
        {
            _isPurchaseBlocked = false;
        }

        void UpdateButton(int gold)
        {
            if (gold < _cost)
            {
                SetEnabled(false);
                return;
            }
            SetEnabled(true);
        }
    }
}
