using System.Collections.Generic;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Units;
using Lis.Units.Creature;
using Lis.Units.Hero.Ability;
using Lis.Units.Minion;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class StatsElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "stats-battle-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussPanel = _ussClassName + "panel";

        const string _ussCreatureContainer = _ussClassName + "creature-container";
        const string _ussCreatureLabel = _ussClassName + "creature-label";
        const string _ussCreatureIcon = _ussClassName + "creature-icon";

        readonly BattleManager _battleManager;

        readonly VisualElement _leftPanel;
        readonly VisualElement _middlePanel;
        readonly VisualElement _rightPanel;

        readonly Stats _stats;

        public StatsElement()
        {
            GameManager gameManager = GameManager.Instance;
            StyleSheet ss = gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.StatsBattleElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _battleManager = BattleManager.Instance;
            if (_battleManager == null)
            {
                Debug.LogError("BattleManager is null");
                return;
            }

            _stats = _battleManager.Battle.Stats;

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);

            _leftPanel = new();
            _middlePanel = new();
            _rightPanel = new();
            _leftPanel.AddToClassList(_ussPanel);
            _middlePanel.AddToClassList(_ussPanel);
            _rightPanel.AddToClassList(_ussPanel);
            Add(_leftPanel);
            Add(_middlePanel);
            Add(_rightPanel);

            PopulateLeftPanel();
            PopulateMiddlePanel();
            PopulateRightPanel();
        }

        void PopulateLeftPanel()
        {
            AddAbilityStats();
        }

        void PopulateMiddlePanel()
        {
            _middlePanel.Add(new PickupStatsElement(_stats));
            AddTotalGold();
            AddTimeSurvived();
            AddTilesUnlocked();
        }

        void PopulateRightPanel()
        {
            AddFriendBallsThrown();
            AddMinionsKilled();
            AddCreatureKills();
        }

        void AddAbilityStats()
        {
            foreach (Ability a in _battleManager.Hero.GetAllAbilities())
            {
                _leftPanel.Add(new AbilityStatsElement(a));
            }
        }

        void AddTotalGold()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;

            Label text = new("Gold collected: ");
            container.Add(text);

            GoldElement el = new(_battleManager.GoldCollected);
            container.Add(el);
            _middlePanel.Add(container);
        }

        void AddTimeSurvived()
        {
            VisualElement container = new();
            _middlePanel.Add(container);

            int minutes = Mathf.FloorToInt(_battleManager.GetTime() / 60f);
            int seconds = Mathf.FloorToInt(_battleManager.GetTime() - minutes * 60);

            string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);

            Label text = new($"Time survived: {timerText}");
            container.Add(text);
        }

        void AddTilesUnlocked()
        {
            VisualElement container = new();
            _middlePanel.Add(container);

            Label text = new($"Tiles unlocked: {_stats.TilesUnlocked}");
            container.Add(text);
        }

        void AddFriendBallsThrown()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            _rightPanel.Add(container);

            Label text = new($"Friend balls thrown: {_stats.FriendBallsThrown}");
            container.Add(text);
        }

        void AddMinionsKilled()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            _rightPanel.Add(container);

            Label text = new($"Minions defeated: ");
            container.Add(text);

            int minionKillCount = 0;
            foreach (Unit e in _battleManager.KilledOpponentEntities)
                if (e is Minion)
                    minionKillCount++;

            ChangingValueElement minionCount = new();
            minionCount.Initialize(minionKillCount, 18);
            container.Add(minionCount);
        }

        void AddCreatureKills()
        {
            VisualElement container = new();
            container.AddToClassList(_ussCreatureContainer);
            _rightPanel.Add(container);

            Label text = new("Creatures defeated:");
            text.AddToClassList(_ussCreatureLabel);
            container.Add(text);

            VisualElement iconContainer = new();
            iconContainer.style.flexWrap = Wrap.Wrap;
            iconContainer.style.flexDirection = FlexDirection.Row;
            container.Add(iconContainer);

            Dictionary<Sprite, int> creatureKillCount = new();

            foreach (Unit e in _battleManager.KilledOpponentEntities)
            {
                if (e is not Creature) continue;
                if (creatureKillCount.ContainsKey(e.Icon))
                    creatureKillCount[e.Icon]++;
                else
                    creatureKillCount.Add(e.Icon, 1);
            }

            foreach (KeyValuePair<Sprite, int> entry in creatureKillCount)
            {
                VisualElement c = new();
                c.style.flexDirection = FlexDirection.Row;
                iconContainer.Add(c);
                Label txt = new($"{entry.Value} x ");
                c.Add(txt);

                VisualElement icon = new();
                icon.AddToClassList(_ussCreatureIcon);
                icon.style.backgroundImage = new StyleBackground(entry.Key);
                c.Add(icon);
            }
        }
    }
}