using Lis.Battle.Pickup;
using Lis.Battle.Tiles;
using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Hero;
using Lis.Units.Hero.Tablets;
using Lis.Units.Minion;
using UnityEngine;

namespace Lis.Battle
{
    public class StatsTracker : MonoBehaviour
    {
        BattleManager _battleManager;
        Stats _stats;

        public void Initialize()
        {
            _battleManager = BattleManager.Instance;
            _stats = _battleManager.Battle.Stats;
            _stats.Reset();

            TrackKills();
            TracksVases();
            TrackTiles();
            TracksPickups();
            TrackFriendBalls();
            TrackCreaturesCaptured();
            TrackAbilitiesUnlocked();
            TrackTabletsCollected();
            TrackAdvancedTabletsCollected();
        }

        void TrackKills()
        {
            _battleManager.OnOpponentEntityDeath += (be) =>
            {
                if (be is CreatureController) _stats.CreaturesKilled++;
                else if (be is MinionController) _stats.MinionsKilled++;
                else if (be is BossController) _stats.BossKilled(be.Unit as Boss);
            };
        }

        void TracksVases()
        {
            GetComponent<BreakableVaseManager>().OnVaseBroken += _ => _stats.VasesBroken++;
        }

        void TrackTiles()
        {
            GetComponent<AreaManager>().OnTileUnlocked += (_) => _stats.TilesUnlocked++;
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

        void TrackFriendBalls()
        {
            _battleManager.HeroController.GetComponentInChildren<CreatureCatcher>().OnBallThrown +=
                () => _stats.FriendBallsThrown++;
        }

        void TrackCreaturesCaptured()
        {
            _battleManager.Hero.OnTroopMemberAdded += creature => _stats.CreatureCaptured(creature);
        }

        void TrackAbilitiesUnlocked()
        {
            _battleManager.Hero.OnAbilityAdded += ability => _stats.AbilityUnlocked(ability);
        }

        void TrackTabletsCollected()
        {
            foreach (Tablet t in _battleManager.Hero.Tablets)
            {
                t.OnLevelUp += tablet => _stats.TabletCollected(tablet);
            }
        }

        void TrackAdvancedTabletsCollected()
        {
            _battleManager.Hero.OnTabletAdvancedAdded += t => _stats.AdvancedTabletCollected(t);
        }
    }
}