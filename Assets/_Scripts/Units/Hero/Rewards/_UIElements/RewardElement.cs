using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Rewards
{
    public class RewardElement : ElementWithSound
    {
        const string _ussCommonTextPrimaryBlack = "common__text-primary-black";
        const string _ussClassName = "reward-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussContent = _ussClassName + "content";
        const string _ussDisabled = _ussClassName + "disabled";
        readonly GameManager _gameManager;
        readonly AudioManager _audioManager;

        public readonly Reward Reward;
        Label _mysteryLabel;

        protected readonly VisualElement ContentContainer;

        protected RewardElement(Reward reward)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.RewardElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _audioManager = AudioManager.Instance;

            Reward = reward;
            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimaryBlack);

            ContentContainer = new();
            ContentContainer.AddToClassList(_ussContent);
            Add(ContentContainer);

            RegisterCallback<ClickEvent>(OnClick);
            RegisterCallback<MouseOverEvent>(OnMouseOver);
        }


        void OnClick(ClickEvent evt)
        {
            if (evt.button != 0) return;

            _audioManager.CreateSound().WithSound(_audioManager.GetSound("Bang")).Play();
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