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

            // iterate from the back to remove safely.
            for(int i = stat.modifiers.Count; i <=0; i--)
                stat.RemoveModifier(stat.modifiers[i]);
        }
    }

}
