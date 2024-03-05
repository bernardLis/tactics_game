using Lis.Battle.Pickup;
using Lis.Battle.Tiles;
using Lis.Units.Creature;
using Lis.Units.Hero;
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

            TrackKills();
            TracksVases();
            TrackTiles();
            TracksPickups();
            TrackFriendBalls();
        }

        void TrackKills()
        {
            _battleManager.OnOpponentEntityDeath += (be) =>
            {
                if (be is CreatureController) _stats.CreaturesKilled++;
                else if (be is MinionController) _stats.MinionsKilled++;
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
                    case ExperienceStone _:
                        _stats.ExpOrbsCollected++;
                        break;
                }
            };
        }

        void TrackFriendBalls()
        {
            _battleManager.HeroController.GetComponent<CreatureCatcher>().OnBallThrown +=
                () => _stats.FriendBallsThrown++;
        }
    }
}