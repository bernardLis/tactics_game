using System;
using System.Collections.Generic;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using UnityEngine;

namespace Lis.Core
{
    public class Stats : ScriptableObject
    {
        public int MinionsKilled;
        public int CreaturesKilled;

        public int TilesUnlocked;

        public int VasesBroken;
        public int CoinsCollected;
        public int HammersCollected;
        public int HorseshoesCollected;
        public int BagsCollected;
        public int SkullsCollected;
        public int FriendBallsCollected;

        public int CommonExpOrbsCollected;
        public int UncommonExpOrbsCollected;
        public int RareExpOrbsCollected;
        public int EpicExpOrbsCollected;

        public int FriendBallsThrown;
        public List<MyObjectData> CreaturesCaptured = new();

        public List<MyObjectData> TabletsCollected = new();
        public List<MyObjectData> AdvancedTabletsCollected = new();

        public List<MyObjectData> AbilitiesUnlocked = new();

        public List<MyObjectData> BossesKilled = new();

        public void Reset()
        {
            MinionsKilled = 0;
            CreaturesKilled = 0;

            TilesUnlocked = 0;
            VasesBroken = 0;
            CoinsCollected = 0;
            HammersCollected = 0;
            HorseshoesCollected = 0;
            BagsCollected = 0;
            SkullsCollected = 0;
            FriendBallsCollected = 0;

            CommonExpOrbsCollected = 0;
            UncommonExpOrbsCollected = 0;
            RareExpOrbsCollected = 0;
            EpicExpOrbsCollected = 0;

            FriendBallsThrown = 0;
            CreaturesCaptured.Clear();

            TabletsCollected.Clear();
            AdvancedTabletsCollected.Clear();

            AbilitiesUnlocked.Clear();

            BossesKilled.Clear();
        }

        public void AbilityUnlocked(Ability a)
        {
            foreach (MyObjectData data in AbilitiesUnlocked)
            {
                if (data.Id == a.Id)
                {
                    data.SetCount(data.Count + 1);
                    return;
                }
            }

            AbilitiesUnlocked.Add(new MyObjectData { Id = a.Id, Count = 1 });
        }

        public void CreatureCaptured(Creature c)
        {
            foreach (MyObjectData data in CreaturesCaptured)
            {
                if (data.Id == c.Id)
                {
                    data.SetCount(data.Count + 1);
                    return;
                }
            }

            CreaturesCaptured.Add(new MyObjectData { Id = c.Id, Count = 1 });
        }

        public void TabletCollected(Tablet t)
        {
            foreach (MyObjectData data in TabletsCollected)
            {
                if (data.Id == t.Id)
                {
                    data.SetCount(data.Count + 1);
                    return;
                }
            }

            TabletsCollected.Add(new MyObjectData { Id = t.Id, Count = 1 });
        }

        public void AdvancedTabletCollected(TabletAdvanced t)
        {
            foreach (MyObjectData data in AdvancedTabletsCollected)
            {
                if (data.Id == t.Id)
                {
                    data.SetCount(data.Count + 1);
                    return;
                }
            }

            AdvancedTabletsCollected.Add(new MyObjectData { Id = t.Id, Count = 1 });
        }

        public void BossKilled(Boss b)
        {
            foreach (MyObjectData data in BossesKilled)
            {
                if (data.Id == b.Id)
                {
                    data.SetCount(data.Count + 1);
                    return;
                }
            }

            BossesKilled.Add(new MyObjectData { Id = b.Id, Count = 1 });
        }

        public void AddStats(Stats newStats)
        {
            MinionsKilled += newStats.MinionsKilled;
            CreaturesKilled += newStats.CreaturesKilled;

            TilesUnlocked += newStats.TilesUnlocked;

            VasesBroken += newStats.VasesBroken;
            CoinsCollected += newStats.CoinsCollected;
            HammersCollected += newStats.HammersCollected;
            HorseshoesCollected += newStats.HorseshoesCollected;
            BagsCollected += newStats.BagsCollected;
            SkullsCollected += newStats.SkullsCollected;
            FriendBallsCollected += newStats.FriendBallsCollected;

            CommonExpOrbsCollected += newStats.CommonExpOrbsCollected;
            UncommonExpOrbsCollected += newStats.UncommonExpOrbsCollected;
            RareExpOrbsCollected += newStats.RareExpOrbsCollected;
            EpicExpOrbsCollected += newStats.EpicExpOrbsCollected;

            FriendBallsThrown += newStats.FriendBallsThrown;

            MergeTwoObjectLists(CreaturesCaptured, newStats.CreaturesCaptured);

            MergeTwoObjectLists(TabletsCollected, newStats.TabletsCollected);
            MergeTwoObjectLists(AdvancedTabletsCollected, newStats.AdvancedTabletsCollected);
            MergeTwoObjectLists(AbilitiesUnlocked, newStats.AbilitiesUnlocked);
            MergeTwoObjectLists(BossesKilled, newStats.BossesKilled);
        }

        void MergeTwoObjectLists(List<MyObjectData> originalList, List<MyObjectData> newLis)
        {
            foreach (MyObjectData b in newLis)
            {
                bool found = false;
                foreach (MyObjectData a in originalList)
                {
                    if (a.Id == b.Id)
                    {
                        a.SetCount(a.Count + b.Count);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    originalList.Add(b);
            }
        }

        /* SERIALIZATION */
        public GameStats SerializeSelf()
        {
            return new()
            {
                MinionsKilled = MinionsKilled,
                CreaturesKilled = CreaturesKilled,

                TilesUnlocked = TilesUnlocked,

                VasesBroken = VasesBroken,
                CoinsCollected = CoinsCollected,
                HammersCollected = HammersCollected,
                HorseshoesCollected = HorseshoesCollected,
                BagsCollected = BagsCollected,
                SkullsCollected = SkullsCollected,
                FriendBallsCollected = FriendBallsCollected,

                CommonExpOrbsCollected = CommonExpOrbsCollected,
                UncommonExpOrbsCollected = UncommonExpOrbsCollected,
                RareExpOrbsCollected = RareExpOrbsCollected,
                EpicExpOrbsCollected = EpicExpOrbsCollected,

                FriendBallsThrown = FriendBallsThrown,
                CreaturesCaptured = CreaturesCaptured,

                TabletsCollected = TabletsCollected,
                AdvancedTabletsCollected = AdvancedTabletsCollected,

                AbilitiesUnlocked = AbilitiesUnlocked,

                BossesKilled = BossesKilled
            };
        }

        public void LoadFromData(GameStats data)
        {
            MinionsKilled = data.MinionsKilled;
            CreaturesKilled = data.CreaturesKilled;

            TilesUnlocked = data.TilesUnlocked;

            VasesBroken = data.VasesBroken;
            CoinsCollected = data.CoinsCollected;
            HammersCollected = data.HammersCollected;
            HorseshoesCollected = data.HorseshoesCollected;
            BagsCollected = data.BagsCollected;
            SkullsCollected = data.SkullsCollected;
            FriendBallsCollected = data.FriendBallsCollected;

            CommonExpOrbsCollected = data.CommonExpOrbsCollected;
            UncommonExpOrbsCollected = data.UncommonExpOrbsCollected;
            RareExpOrbsCollected = data.RareExpOrbsCollected;
            EpicExpOrbsCollected = data.EpicExpOrbsCollected;

            FriendBallsThrown = data.FriendBallsThrown;
            CreaturesCaptured = data.CreaturesCaptured;

            TabletsCollected = data.TabletsCollected;
            AdvancedTabletsCollected = data.AdvancedTabletsCollected;

            AbilitiesUnlocked = data.AbilitiesUnlocked;

            BossesKilled = data.BossesKilled;
        }
    }

    [Serializable]
    public struct GameStats
    {
        public int MinionsKilled;
        public int CreaturesKilled;

        public int TilesUnlocked;

        public int VasesBroken;
        public int CoinsCollected;
        public int HammersCollected;
        public int HorseshoesCollected;
        public int BagsCollected;
        public int SkullsCollected;
        public int FriendBallsCollected;

        public int CommonExpOrbsCollected;
        public int UncommonExpOrbsCollected;
        public int RareExpOrbsCollected;
        public int EpicExpOrbsCollected;

        public int FriendBallsThrown;

        public List<MyObjectData> CreaturesCaptured;
        public List<MyObjectData> TabletsCollected;
        public List<MyObjectData> AdvancedTabletsCollected;
        public List<MyObjectData> AbilitiesUnlocked;
        public List<MyObjectData> BossesKilled;
    }

    [Serializable]
    public struct MyObjectData
    {
        public string Id;
        public int Count;

        public void SetCount(int c)
        {
            Count = c;
        }
    }
}