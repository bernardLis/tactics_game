using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Quest/Destroy")]
    public class QuestDestroy : Quest
    {
        // TODO: more breakable types
        public override void StartQuest()
        {
            base.StartQuest();
            BattleVaseManager bvm = BattleManager.Instance.GetComponent<BattleVaseManager>();

            bvm.OnVaseBroken += CheckDestroyable;
            OnQuestCompleted += () => bvm.OnVaseBroken -= CheckDestroyable;

            TotalAmount = 2; // HERE: testing
        }

        void CheckDestroyable(BattleBreakableVase vase)
        {
            UpdateQuest();
        }

        public override Sprite GetIcon()
        {
            return GameManager.Instance.GameDatabase.VaseIcon;
        }

        public override string GetDescription()
        {
            return $"Destroy {TotalAmount - CurrentAmount} Vases";
        }
    }
}