
namespace Lis
{
    public class QuestKill : Quest
    {
        public Minion MinionToKill;

        public override void CreateRandom(int level)
        {
            base.CreateRandom(level);

            MinionToKill = GameManager.Instance.EntityDatabase.GetRandomMinion();
        }

        public override void StartQuest()
        {
            base.StartQuest();

            BattleManager.Instance.OnOpponentEntityDeath += CheckMinionDeath;
            OnQuestCompleted += () => BattleManager.Instance.OnOpponentEntityDeath -= CheckMinionDeath;
        }

        void CheckMinionDeath(BattleEntity entity)
        {
            if (entity.Entity == MinionToKill)
                UpdateQuest();
        }
    }
}