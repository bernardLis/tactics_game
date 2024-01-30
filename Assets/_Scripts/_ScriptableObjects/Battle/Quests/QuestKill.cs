using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Quest/Kill")]
    public class QuestKill : Quest
    {
        public Minion MinionToKill;

        public override void CreateRandom(int level, List<Quest> previousQuests)
        {
            base.CreateRandom(level, previousQuests);

            GameManager gameManager = GameManager.Instance;
            List<Minion> minions = gameManager.EntityDatabase.GetAllMinions();
            foreach (Quest q in previousQuests)
                if (q is QuestKill questKill)
                    minions.Remove(questKill.MinionToKill);

            // TODO: something more sophisticated, like a minion that is not in the previous x quests
            MinionToKill = minions.Count > 0
                ? minions[Random.Range(0, minions.Count)]
                : gameManager.EntityDatabase.GetRandomMinion();
        }

        public override void StartQuest()
        {
            base.StartQuest();

            BattleManager.Instance.OnOpponentEntityDeath += CheckMinionDeath;
            OnQuestCompleted += () => BattleManager.Instance.OnOpponentEntityDeath -= CheckMinionDeath;
        }

        void CheckMinionDeath(BattleEntity entity)
        {
            // TODO: this is incorrect, but for now I am going for it, just the elements
            if (entity.Entity.Element == MinionToKill.Element)
                UpdateQuest();
        }

        public override Sprite GetIcon()
        {
            return MinionToKill.Icon;
        }
    }
}