


using UnityEngine.UIElements;

namespace Lis
{
    public class RewardElement : ElementWithSound
    {
        readonly GameManager _gameManager;
        const string _ussClassName = "reward-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussDisabled = _ussClassName + "disabled";

        public Reward Reward;

        public RewardElement(Reward reward)
        {
            _gameManager = GameManager.Instance;
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            Reward = reward;
            AddToClassList(_ussMain);

            RegisterCallback<ClickEvent>(OnClick);
        }

        void OnClick(ClickEvent evt)
        {
            if (evt.button != 0) return;

            _gameManager.GetComponent<AudioManager>().PlayUI("Bang");
            Reward.GetReward();
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

    }
}
