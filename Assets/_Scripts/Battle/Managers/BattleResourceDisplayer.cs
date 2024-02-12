using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class BattleResourceDisplayer : MonoBehaviour
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        
        GameManager _gameManager;
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;

        VisualElement _root;
        VisualElement _resourcePanel;
        TroopsCountElement _troopsCounter;

        GoldElement _goldElement;

        Hero _hero;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

            _root = _battleManager.Root;
            _resourcePanel = _root.Q<VisualElement>("resourcePanel");

            _battleManager.OnBattleInitialized += ResolveResourcePanel;
        }

        void ResolveResourcePanel()
        {
            _hero = _battleManager.Hero;

            _resourcePanel.style.opacity = 0f;
            _resourcePanel.style.display = DisplayStyle.Flex;
            DOTween.To(x => _resourcePanel.style.opacity = x, 0, 1, 0.5f).SetDelay(3f);

            UpgradeBoard globalUpgradeBoard = _gameManager.UpgradeBoard;
            AddFriendBallCountElement();
            AddTroopsCountElement();
            if (globalUpgradeBoard.GetUpgradeByName("Gold Count").CurrentLevel != -1)
                AddGoldElement();
        }

        void AddGoldElement()
        {
            _goldElement = new(_gameManager.Gold);
            _gameManager.OnGoldChanged += OnGoldChanged;
            _resourcePanel.Add(_goldElement);
        }

        void AddFriendBallCountElement()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;

            Label icon = new();
            icon.style.backgroundImage = new(_gameManager.GameDatabase.FriendBallIcon);
            icon.style.width = 25;
            icon.style.height = 25;

            Label friendBallCountLabel = new($"{_hero.NumberOfFriendBalls}");
            friendBallCountLabel.AddToClassList(_ussCommonTextPrimary);
            _hero.OnFriendBallCountChanged +=
                () => friendBallCountLabel.text = $"{_hero.NumberOfFriendBalls}";

            container.Add(icon);
            container.Add(friendBallCountLabel);
            _resourcePanel.Add(container);
        }

        void AddTroopsCountElement()
        {
            _troopsCounter = new("");
            _resourcePanel.Add(_troopsCounter);
            _hero.OnTroopMemberAdded += (_) => UpdateTroopsCountElement();
            _hero.TroopsLimit.OnValueChanged += (_) => UpdateTroopsCountElement();

            UpdateTroopsCountElement();
        }

        void UpdateTroopsCountElement()
        {
            _troopsCounter.UpdateCountContainer($"{_hero.Troops.Count} / {_hero.TroopsLimit.Value}",
                Color.white);
        }

        void OnGoldChanged(int newValue)
        {
            int change = newValue - _goldElement.Amount;
            Helpers.DisplayTextOnElement(_root, _goldElement, "" + change, Color.yellow);
            _goldElement.ChangeAmount(newValue);
        }
    }
}