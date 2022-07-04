public enum QuestState { INACTIVE, ACTIVE, COMPLETED, FAILED }

public abstract class Quest : BaseScriptableObject
{
	public string Title;
	public string Description;
	public QuestState State;
	public QuestGoal[] Goals;
	public Item Reward;

	public abstract void Trigger();
	public abstract void Complete();
	public abstract void Fail();

}
