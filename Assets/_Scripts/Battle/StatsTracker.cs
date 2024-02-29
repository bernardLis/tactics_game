using Lis.Battle.Land;
using Lis.Battle.Pickup;
using Lis.Units.Creature;
using Lis.Units.Hero;
using Lis.Units.Minion;
using UnityEngine;

namespace Lis.Battle
{
    public class StatsTracker : MonoBehaviour
    {
        public Stats Stats;

        public void Initialize()
        {
            Stats.Initialize();
            TrackKills();
            TracksVases();
            TrackTiles();
            TracksPickups();
            TrackFriendBalls();
        }

        void TrackKills()
        {
            BattleManager.Instance.OnOpponentEntityDeath += (be) =>
            {
                if (be is CreatureController) Stats.CreaturesKilled++;
                else if (be is MinionController) Stats.MinionsKilled++;
            };
        }

        void TracksVases()
        {
            GetComponent<BreakableVaseManager>().OnVaseBroken += _ => Stats.VasesBroken++;
        }

        void TrackTiles()
        {
            GetComponent<AreaManager>().OnTileUnlocked += (_) => Stats.TilesUnlocked++;
        }

        void TracksPickups()
        {
            GetComponent<PickupManager>().OnPickupCollected += pickup =>
            {
                switch (pickup)
                {
                    case Coin _:
                        Stats.CoinsCollected++;
                        break;
                    case Hammer _:
                        Stats.HammersCollected++;
                        break;
                    case Horseshoe _:
                        Stats.HorseshoesCollected++;
                        break;
                    case Bag _:
                        Stats.BagsCollected++;
                        break;
                    case Skull _:
                        Stats.SkullsCollected++;
                        break;
                    case FriendBall _:
                        Stats.FriendBallsCollected++;
                        break;
                    case ExperienceStone _:
                        Stats.ExpOrbsCollected++;
                        break;
                }
            };
        }

        void TrackFriendBalls()
        {
            BattleManager.Instance.HeroController.GetComponent<CreatureCatcher>().OnBallThrown +=
                () => Stats.FriendBallsThrown++;
        }
    }
}