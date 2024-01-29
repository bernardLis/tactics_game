using UnityEngine;

namespace Lis
{
    public class QuestCollect : Quest
    {
        public Pickup PickupToCollect;

        public override void CreateRandom(int level)
        {
            base.CreateRandom(level);
            QuestablePickup qp = GameManager.Instance.GameDatabase.GetRandomQuestablePickup();
            PickupToCollect = qp.Pickup;
            TotalAmount = Random.Range(qp.AmountRange.x, qp.AmountRange.y);
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
    }
}