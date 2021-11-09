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
    }

}
