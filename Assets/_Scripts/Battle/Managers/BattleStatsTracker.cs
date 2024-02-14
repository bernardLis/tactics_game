using UnityEngine;

namespace Lis
{
    public class BattleStatsTracker : MonoBehaviour
    {
        public BattleStats Stats;

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
                if (be is BattleCreature) Stats.CreaturesKilled++;
                else if (be is BattleMinion) Stats.MinionsKilled++;
            };
        }

        void TracksVases()
        {
            GetComponent<BattleVaseManager>().OnVaseBroken += _ => Stats.VasesBroken++;
        }

        void TrackTiles()
        {
            GetComponent<BattleAreaManager>().OnTileUnlocked += (_) => Stats.TilesUnlocked++;
        }

        void TracksPickups()
        {
            GetComponent<BattlePickupManager>().OnPickupCollected += pickup =>
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
                    case ExperienceOrb _:
                        Stats.ExpOrbsCollected++;
                        break;
                }
            };
        }

        void TrackFriendBalls()
        {
            BattleManager.Instance.BattleHero.GetComponent<BattleCreatureCatcher>().OnBallThrown +=
                () => Stats.FriendBallsThrown++;
        }
    }
}