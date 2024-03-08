using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class GameStatsElement : VisualElement
    {
        const string _commonTextPrimary = "common__text-primary";

        const string _ussClassName = "game-stats__";
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

        const string _ussCommonExpOrbsCollectedIcon = _ussClassName + "common-exp-orbs-collected-icon";
        const string _ussUncommonExpOrbsCollectedIcon = _ussClassName + "uncommon-exp-orbs-collected-icon";
        const string _ussRareExpOrbsCollectedIcon = _ussClassName + "rare-exp-orbs-collected-icon";
        const string _ussEpicExpOrbsCollectedIcon = _ussClassName + "epic-exp-orbs-collected-icon";

        const string _ussFriendBallsThrownIcon = _ussClassName + "friend-balls-thrown-icon";

        GameManager _gameManager;
        Stats _stats;

        ScrollView _scrollView;

        IVisualElementScheduledItem _scrollScheduler;

        public GameStatsElement(Stats stats)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.GameStatsStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_commonTextPrimary);

            _stats = stats;

            _scrollView = new();
            Add(_scrollView);

            AddStat("Minions Killed: ", _stats.MinionsKilled, _ussMinionsKilledIcon);
            AddStat("Creatures Killed: ", _stats.CreaturesKilled, _ussCreaturesKilledIcon);
            AddStat("Tiles Unlocked: ", _stats.TilesUnlocked, _ussTilesUnlockedIcon);

            AddStat("Vases Broken: ", _stats.VasesBroken, _ussVasesBrokenIcon);
            AddStat("Coins Collected: ", _stats.CoinsCollected, _ussCoinsCollectedIcon);
            AddStat("Hammers Collected: ", _stats.HammersCollected, _ussHammersCollectedIcon);
            AddStat("Horseshoes Collected: ", _stats.HorseshoesCollected, _ussHorseshoesCollectedIcon);
            AddStat("Bags Collected: ", _stats.BagsCollected, _ussBagsCollectedIcon);
            AddStat("Skulls Collected: ", _stats.SkullsCollected, _ussSkullsCollectedIcon);
            AddStat("Friend Balls Collected: ", _stats.FriendBallsCollected, _ussFriendBallsCollectedIcon);

            AddStat("Common Exp Orbs Collected: ", _stats.CommonExpOrbsCollected, _ussCommonExpOrbsCollectedIcon);
            AddStat("Uncommon Exp Orbs Collected: ", _stats.UncommonExpOrbsCollected, _ussUncommonExpOrbsCollectedIcon);
            AddStat("Rare Exp Orbs Collected: ", _stats.RareExpOrbsCollected, _ussRareExpOrbsCollectedIcon);
            AddStat("Epic Exp Orbs Collected: ", _stats.EpicExpOrbsCollected, _ussEpicExpOrbsCollectedIcon);

            AddStat("Friend Balls Thrown: ", _stats.FriendBallsThrown, _ussFriendBallsThrownIcon);

            AddCreaturesCaptured();
            AddCollectedTablets();
            AddCollectedAdvancedTablets();
            AddAbilitiesUnlocked();
            AddBossesKilled();

            _scrollScheduler = schedule.Execute(ScrollDown).Every(1500);
            RegisterCallback<MouseEnterEvent>(StopScrolling);
            RegisterCallback<MouseLeaveEvent>(ResumeScrolling);
        }

        int _currentChild;

        void ScrollDown()
        {
            _scrollView.ScrollTo(_scrollView[_currentChild % _scrollView.childCount]);
            _currentChild++;
        }

        void StopScrolling(MouseEnterEvent e)
        {
            _scrollScheduler.Pause();
        }

        void ResumeScrolling(MouseLeaveEvent e)
        {
            _scrollScheduler.Resume();
        }


        void AddStat(string text, int value, string iconClass, Texture2D iconSprite = null)
        {
            VisualElement container = new();
            container.AddToClassList(_ussStatContainer);

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

/*
public List<MyObjectData> TabletsCollected = new();
public List<MyObjectData> AdvancedTabletsCollected = new();

public List<MyObjectData> AbilitiesUnlocked = new();

public List<MyObjectData> BossesKilled = new();
*/