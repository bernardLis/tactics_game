namespace Lis
{
    public class QuestDestroy : Quest
    {
        // TODO: more breakable types
        public override void StartQuest()
        {
            base.StartQuest();

            BattleManager.Instance.GetComponent<BattleVaseManager>().OnVaseBroken += CheckDestroyable;
            OnQuestCompleted += () =>
                BattleManager.Instance.GetComponent<BattleVaseManager>().OnVaseBroken -= CheckDestroyable;
        }

        void CheckDestroyable(BattleBreakableVase vase)
        {
            UpdateQuest();
        }
    }
}