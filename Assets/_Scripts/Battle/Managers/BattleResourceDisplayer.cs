


using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class BattleResourceDisplayer : MonoBehaviour
    {
        GameManager _gameManager;
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;

        VisualElement _root;
        VisualElement _resourcePanel;
        TroopsCountElement _troopsCounter;

        GoldElement _goldElement;

        LineTimerElement _nextTileTimer;

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
            _resourcePanel.style.opacity = 0f;
            _resourcePanel.style.display = DisplayStyle.Flex;
            DOTween.To(x => _resourcePanel.style.opacity = x, 0, 1, 0.5f).SetDelay(3f);

            UpgradeBoard globalUpgradeBoard = _gameManager.UpgradeBoard;
            if (globalUpgradeBoard.GetUpgradeByName("Troops Count").CurrentLevel != -1)
                AddTroopsCountElement();
            if (globalUpgradeBoard.GetUpgradeByName("Gold Count").CurrentLevel != -1)
                AddGoldElement();
            if (globalUpgradeBoard.GetUpgradeByName("Next Tile Timer").CurrentLevel != -1)
                AddNextTileTimer();
        }

        void AddGoldElement()
        {
            _goldElement = new(_gameManager.Gold);
            _gameManager.OnGoldChanged += OnGoldChanged;
            _resourcePanel.Add(_goldElement);
        }

        void AddTroopsCountElement()
        {
            _troopsCounter = new("");
            _resourcePanel.Add(_troopsCounter);

            _battleManager.OnPlayerCreatureAdded += (c) => UpdateTroopsCountElement();
            _battleManager.OnPlayerEntityDeath += (c) => UpdateTroopsCountElement();

            UpdateTroopsCountElement();
        }

        void UpdateTroopsCountElement()
        {
            int count = Mathf.Clamp(_battleManager.PlayerCreatures.Count - 1, 0, 9999);
            _troopsCounter.UpdateCountContainer($"{count}", Color.white);
        }

        void OnGoldChanged(int newValue)
        {
            int change = newValue - _goldElement.Amount;
            Helpers.DisplayTextOnElement(_root, _goldElement, "" + change, Color.yellow);
            _goldElement.ChangeAmount(newValue);
        }

        void AddNextTileTimer()
        {
            _nextTileTimer = new(55, 60, true, "Next Tile Unlocked In");
            _nextTileTimer.HideLabel();
            _resourcePanel.Add(_nextTileTimer);
        }
    }
}
