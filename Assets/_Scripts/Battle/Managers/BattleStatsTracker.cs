using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleStatsTracker : MonoBehaviour
    {
        [SerializeField] BattleStats _stats;

        public void Initialize()
        {
            _stats.Initialize();
            TrackKills();
            TracksVases();
            TrackTiles();
            TracksPickups();
        }

        void TrackKills()
        {
            BattleManager.Instance.OnOpponentEntityDeath += (be) =>
            {
                if (be is BattleCreature) _stats.CreaturesKilled++;
                else if (be is BattleMinion) _stats.MinionsKilled++;
            };
        }

        void TracksVases()
        {
            GetComponent<BattleVaseManager>().OnVaseBroken += _ => _stats.VasesBroken++;
        }

        void TrackTiles()
        {
            GetComponent<BattleAreaManager>().OnTileUnlocked += (_) => _stats.TilesUnlocked++;
        }

        void TracksPickups()
        {
            GetComponent<BattlePickupManager>().OnPickupCollected += pickup =>
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
                    case ExperienceOrb _:
                        _stats.ExpOrbsCollected++;
                        break;
                }
            };
        }
    }
}