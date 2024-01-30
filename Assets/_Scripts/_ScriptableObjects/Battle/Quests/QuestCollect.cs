using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Quest/Collect")]
    public class QuestCollect : Quest
    {
        public QuestablePickup QuestablePickup;
        public Pickup PickupToCollect;

        public override void CreateRandom(int level, List<Quest> previousQuests)
        {
            GameManager gameManager = GameManager.Instance;
            base.CreateRandom(level, previousQuests);

            List<QuestablePickup> questablePickups = new(gameManager.GameDatabase.QuestablePickups.ToList());
            foreach (Quest q in previousQuests)
                if (q is QuestCollect questCollect)
                    questablePickups.Remove(questCollect.QuestablePickup);

            // TODO: something more sophisticated, like a minion that is not in the previous x quests
            QuestablePickup = questablePickups.Count > 0
                ? questablePickups[Random.Range(0, questablePickups.Count)]
                : gameManager.GameDatabase.GetRandomQuestablePickup();

            PickupToCollect = QuestablePickup.Pickup;
            TotalAmount = Random.Range(QuestablePickup.AmountRange.x, QuestablePickup.AmountRange.y);
        }

        public override void StartQuest()
        {
            base.StartQuest();

            BattleManager.Instance.GetComponent<BattlePickupManager>().OnPickupCollected += CheckPickup;
            OnQuestCompleted += () =>
                BattleManager.Instance.GetComponent<BattlePickupManager>().OnPickupCollected -= CheckPickup;
        }

        void CheckPickup(Pickup pickup)
        {
            if (pickup == PickupToCollect)
                UpdateQuest();
        }

        public override Sprite GetIcon()
        {
            return PickupToCollect.Icon;
        }
    }
}