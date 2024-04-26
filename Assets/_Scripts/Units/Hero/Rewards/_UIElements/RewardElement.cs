using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElement : ElementWithSound
    {
        readonly GameManager _gameManager;
        const string _ussCommonTextPrimaryBlack = "common__text-primary-black";
        const string _ussClassName = "reward-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussDisabled = _ussClassName + "disabled";

        public readonly Reward Reward;

        protected RewardElement(Reward reward)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.RewardElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            Reward = reward;
            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimaryBlack);

            RegisterCallback<ClickEvent>(OnClick);
            RegisterCallback<MouseOverEvent>(OnMouseOver);
        }


        void OnClick(ClickEvent evt)
        {
            if (evt.button != 0) return;

            _gameManager.GetComponent<AudioManager>().PlayUI("Bang");
            Reward.GetReward();
        }

        void OnMouseOver(MouseOverEvent evt)
        {
            BringToFront();
        }

        public void DisableCard()
        {
            SetEnabled(false);
            AddToClassList(_ussDisabled);
        }

        public void DisableClicks()
        {
            UnregisterCallback<ClickEvent>(OnClick);
        }

        public void IsShop()
        {
            UnregisterCallback<ClickEvent>(OnClick);
            UnregisterCallback<MouseOverEvent>(OnMouseOver);
        }
    }
}