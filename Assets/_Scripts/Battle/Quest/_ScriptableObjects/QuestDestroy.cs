using Lis.Battle.Pickup;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Quest
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Quest/Destroy")]
    public class QuestDestroy : Quest
    {
        // TODO: more breakable types
        public override void StartQuest()
        {
            base.StartQuest();
            BreakableVaseManager bvm = BattleManager.Instance.GetComponent<BreakableVaseManager>();

            bvm.OnVaseBroken += CheckDestroyable;
            OnQuestCompleted += () => bvm.OnVaseBroken -= CheckDestroyable;

            TotalAmount = 2; // HERE: testing
        }

        void CheckDestroyable(BreakableVaseController vaseController)
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