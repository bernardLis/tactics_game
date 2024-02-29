using System.Collections.Generic;
using System.Linq;
using Lis.Battle.Pickup;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Quest
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Quest/Collect")]
    public class QuestCollect : Quest
    {
        public QuestablePickup QuestablePickup;
        public Pickup.Pickup PickupToCollect;

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

            BattleManager.Instance.GetComponent<PickupManager>().OnPickupCollected += CheckPickup;
            OnQuestCompleted += () =>
                BattleManager.Instance.GetComponent<PickupManager>().OnPickupCollected -= CheckPickup;
        }

        void CheckPickup(Pickup.Pickup pickup)
        {
            if (pickup.Id == PickupToCollect.Id)
                UpdateQuest();
        }

        public override Sprite GetIcon()
        {
            return PickupToCollect.Icon;
        }

        public override string GetDescription()
        {
            return $"Collect {TotalAmount - CurrentAmount} {PickupToCollect.name}";
        }
    }
}