using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class StatsScreen : FullScreenElement
    {
        const string _commonTextPrimary = "common__text-primary";
        const string _commonOddBackground = "common__odd-background";
        const string _commonEvenBackground = "common__even-background";

        const string _ussClassName = "stats-screen__";
        const string _ussStatContainer = _ussClassName + "stat-container";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussIconUndiscovered = _ussClassName + "icon-undiscovered";

        const string _ussMinionsKilledIcon = _ussClassName + "minions-killed-icon";
        const string _ussCreaturesKilledIcon = _ussClassName + "creatures-killed-icon";
        const string _ussTilesUnlockedIcon = _ussClassName + "tiles-unlocked-icon";

        const string _ussVasesBrokenIcon = _ussClassName + "vases-broken-icon";
        const string _ussCoinsCollectedIcon = _ussClassName + "coins-collected-icon";
        const string _ussHammersCollectedIcon = _ussClassName + "hammers-collected-icon";
        const string _ussHorseshoesCollectedIcon = _ussClassName + "horseshoes-collected-icon";
        const string _ussBagsCollectedIcon = _ussClassName + "bags-collected-icon";
        const string _ussSkullsCollectedIcon = _ussClassName + "skulls-collected-icon";
        const string _ussFriendBallsCollectedIcon = _ussClassName + "friend-balls-collected-icon";

        const string _ussCommonExpStonesCollectedIcon = _ussClassName + "common-exp-stones-collected-icon";
        const string _ussUncommonExpStonesCollectedIcon = _ussClassName + "uncommon-exp-stones-collected-icon";
        const string _ussRareExpStonesCollectedIcon = _ussClassName + "rare-exp-stones-collected-icon";
        const string _ussEpicExpStonesCollectedIcon = _ussClassName + "epic-exp-stones-collected-icon";

        const string _ussFriendBallsThrownIcon = _ussClassName + "friend-balls-thrown-icon";

        readonly Stats _stats;
        readonly ScrollView _scrollView;
        readonly IVisualElementScheduledItem _scrollScheduler;

        public StatsScreen(Stats stats)
        {
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.StatsScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_commonTextPrimary);

            _stats = stats;
            Label title = new("Stats");
            title.style.fontSize = 48;
            _content.Add(title);

            _scrollView = new();
            _content.Add(_scrollView);

            AddBasicStats();
            AddPickupStats();
            AddExpStoneStats();

            AddStat("Friend Balls Thrown: ", _stats.FriendBallsThrown, _ussFriendBallsThrownIcon);
            AddCreaturesCaptured();
            _scrollView.Add(new HorizontalSpacerElement());

            AddCollectedTablets();
            AddCollectedAdvancedTablets();
            _scrollView.Add(new HorizontalSpacerElement());

            AddAbilitiesUnlocked();
            _scrollView.Add(new HorizontalSpacerElement());
            AddBossesKilled();

            AddContinueButton();
        }

        void AddBasicStats()
        {
            AddStat("Minions Killed: ", _stats.MinionsKilled, _ussMinionsKilledIcon);
            AddStat("Creatures Killed: ", _stats.CreaturesKilled, _ussCreaturesKilledIcon);
            AddStat("Tiles Unlocked: ", _stats.TilesUnlocked, _ussTilesUnlockedIcon);
            _scrollView.Add(new HorizontalSpacerElement());
        }

        void AddPickupStats()
        {
            AddStat("Vases Broken: ", _stats.VasesBroken, _ussVasesBrokenIcon);
            AddStat("Coins Collected: ", _stats.CoinsCollected, _ussCoinsCollectedIcon);
            AddStat("Hammers Collected: ", _stats.HammersCollected, _ussHammersCollectedIcon);
            AddStat("Horseshoes Collected: ", _stats.HorseshoesCollected, _ussHorseshoesCollectedIcon);
            AddStat("Bags Collected: ", _stats.BagsCollected, _ussBagsCollectedIcon);
            AddStat("Skulls Collected: ", _stats.SkullsCollected, _ussSkullsCollectedIcon);
            AddStat("Friend Balls Collected: ", _stats.FriendBallsCollected, _ussFriendBallsCollectedIcon);
            _scrollView.Add(new HorizontalSpacerElement());
        }

        void AddExpStoneStats()
        {
            AddStat("Common Exp Stones Collected: ", _stats.CommonExpStonesCollected, _ussCommonExpStonesCollectedIcon);
            AddStat("Uncommon Exp Stones Collected: ", _stats.UncommonExpStonesCollected, _ussUncommonExpStonesCollectedIcon);
            AddStat("Rare Exp Stones Collected: ", _stats.RareExpStonesCollected, _ussRareExpStonesCollectedIcon);
            AddStat("Epic Exp Stones Collected: ", _stats.EpicExpStonesCollected, _ussEpicExpStonesCollectedIcon);
            _scrollView.Add(new HorizontalSpacerElement());
        }

        bool _isEven;
        void AddStat(string text, int value, string iconClass, Texture2D iconSprite = null)
        {
            VisualElement container = new();
            container.AddToClassList(_ussStatContainer);
            container.AddToClassList(_isEven ? _commonEvenBackground : _commonOddBackground);
            _isEven = !_isEven;

            VisualElement icon = new();
            icon.AddToClassList(_ussIcon);
            icon.AddToClassList(iconClass);
            if (iconSprite != null) icon.style.backgroundImage = new(iconSprite);
            if (value == 0) icon.AddToClassList(_ussIconUndiscovered);
            container.Add(icon);

            Label desc = new();
            string actualText = text + ": " + value;
            if (value == 0) actualText = "???";
            desc.text = actualText;
            container.Add(desc);

            _scrollView.Add(container);
        }

        void AddCreaturesCaptured()
        {
            foreach (Creature creature in _gameManager.EntityDatabase.AllCreatures)
            {
                int value = 0;
                if (_stats.CreaturesCaptured.Exists(d => d.Id == creature.Id))
                    value = _stats.CreaturesCaptured.Find(d => d.Id == creature.Id).Count;
                AddStat($"Captured {creature.name}", value, "", creature.Icon.texture);
            }
        }

        void AddCollectedTablets()
        {
            foreach (Tablet tablet in _gameManager.EntityDatabase.HeroTablets)
            {
                int value = 0;
                if (_stats.TabletsCollected.Exists(d => d.Id == tablet.Id))
                    value = _stats.TabletsCollected.Find(d => d.Id == tablet.Id).Count;
                AddStat($"Collected {tablet.name}", value, "", tablet.Icon.texture);
            }
        }

        void AddCollectedAdvancedTablets()
        {
            foreach (TabletAdvanced tablet in _gameManager.EntityDatabase.GetAllAdvancedTablets())
            {
                int value = 0;
                if (_stats.AdvancedTabletsCollected.Exists(d => d.Id == tablet.Id))
                    value = _stats.AdvancedTabletsCollected.Find(d => d.Id == tablet.Id).Count;
                AddStat($"Collected {tablet.name} (Advanced)", value, "", tablet.Icon.texture);
            }
        }

        void AddAbilitiesUnlocked()
        {
            foreach (Ability ability in _gameManager.EntityDatabase.GetAllAbilities())
            {
                int value = 0;
                if (_stats.AbilitiesUnlocked.Exists(d => d.Id == ability.Id))
                    value = _stats.AbilitiesUnlocked.Find(d => d.Id == ability.Id).Count;
                AddStat($"Unlocked {ability.name}", value, "", ability.Icon.texture);
            }
        }

        void AddBossesKilled()
        {
            foreach (Boss boss in _gameManager.EntityDatabase.GetAllBosses())
            {
                int value = 0;
                if (_stats.BossesKilled.Exists(d => d.Id == boss.Id))
                    value = _stats.BossesKilled.Find(d => d.Id == boss.Id).Count;
                AddStat($"Killed {boss.name}", value, "", boss.Icon.texture);
            }
        }
    }
}
