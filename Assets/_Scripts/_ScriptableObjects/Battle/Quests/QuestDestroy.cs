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

            BattleManager.Instance.GetComponent<BattleVaseManager>().OnVaseBroken += CheckDestroyable;
            OnQuestCompleted += () =>
                BattleManager.Instance.GetComponent<BattleVaseManager>().OnVaseBroken -= CheckDestroyable;
        }

        void CheckDestroyable(BattleBreakableVase vase)
        {
            UpdateQuest();
        }

        public override Sprite GetIcon()
        {
            return GameManager.Instance.GameDatabase.VaseIcon;
        }
    }
}