public class PlayerStats : CharacterStats
{

    protected override void Awake()
    {
        base.Awake();
        FindObjectOfType<TurnManager>().EnemyTurnEndEvent += OnEnemyTurnEnd;
    }

    public override void Die()
    {
        base.Die();
    }

    void OnEnemyTurnEnd()
    {
        GainMana(10);

        // TODO: modifiers should last number of turns and I should be checking each stat for modifier and how many turns are left;
        foreach(Stat stat in stats)
        {
            if (stat.modifiers.Count == 0)
                return;

            foreach(int modifier in stat.modifiers)
            {
                stat.RemoveModifier(modifier);
            }
        }
    }

}
