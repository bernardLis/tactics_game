using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Hero.Tablets;
using UnityEngine;

namespace Lis.Battle
{
    public class StatsTracker : MonoBehaviour
    {
        BattleManager _battleManager;
        FightManager _fightManager;
        HeroManager _heroManager;
        Stats _stats;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _fightManager = _battleManager.GetComponent<FightManager>();
            _heroManager = _battleManager.GetComponent<HeroManager>();
            _stats = _battleManager.Battle.Stats;
            _stats.Reset();

            TrackKills();
            TracksVases();
            TracksPickups();
            TrackAbilitiesUnlocked();
            TrackTabletsCollected();
            TrackAdvancedTabletsCollected();
        }

        void TrackKills()
        {
            _fightManager.OnOpponentUnitDeath += (be) =>
            {
                if (be is CreatureController) _stats.CreaturesKilled++;
                else if (be is BossController) _stats.BossKilled(be.Unit as Boss);
            };
        }

        void TracksVases()
        {
            GetComponent<BreakableVaseManager>().OnVaseBroken += _ => _stats.VasesBroken++;
        }


        void TracksPickups()
        {
            GetComponent<PickupManager>().OnPickupCollected += pickup =>
            {
                switch (pickup)
                {
                    case Coin _:
                        _stats.CoinsCollected++;
                        break;
                    case Hammer _:
                        _stats.HammersCollected++;
                        break;
                    case Horseshoe _:
                        _stats.HorseshoesCollected++;
                        break;
                    case Bag _:
                        _stats.BagsCollected++;
                        break;
                    case Skull _:
                        _stats.SkullsCollected++;
                        break;
                    case FriendBall _:
                        _stats.FriendBallsCollected++;
                        break;
                    case ExperienceStone stone:
                        AssignOrb(stone);
                        break;
                }
            };
        }

        void AssignOrb(ExperienceStone stone)
        {
            // meh solution
            if (stone.name == "Common Experience Stone")
                _stats.CommonExpStonesCollected++;
            else if (stone.name == "Uncommon Experience Stone")
                _stats.UncommonExpStonesCollected++;
            else if (stone.name == "Rare Experience Stone")
                _stats.RareExpStonesCollected++;
            else if (stone.name == "Epic Experience Stone")
                _stats.EpicExpStonesCollected++;
        }

        void TrackAbilitiesUnlocked()
        {
            _heroManager.Hero.OnAbilityAdded += ability => _stats.AbilityUnlocked(ability);
        }

        void TrackTabletsCollected()
        {
            foreach (Tablet t in _heroManager.Hero.Tablets)
            {
                t.OnLevelUp += tablet => _stats.TabletCollected(tablet);
            }
        }

        void TrackAdvancedTabletsCollected()
        {
            _heroManager.Hero.OnTabletAdvancedAdded += t => _stats.AdvancedTabletCollected(t);
        }
    }
}