using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SafetyWall : MonoBehaviour, ICreatable<Vector3, Ability>, IUITextDisplayable
{
    BoxCollider2D _selfCollider;

    int _numberOfTurnsLeft;

    [SerializeField] Absorber _absorber;

    Ability _ability;

    CharacterStats _shieldedCharacter;

    Vector3 offsetVector = new Vector3(0.05f, 0.3f, 0f);

    public async Task Initialize(Vector3 pos, Ability ability)
    {
        transform.position = transform.position + offsetVector;

        _ability = ability;
        _numberOfTurnsLeft = ability.BasePower;
        _absorber = Instantiate(_absorber);

        _selfCollider = GetComponent<BoxCollider2D>();
        _selfCollider.enabled = false;
        CheckCollision(ability, pos);
        _selfCollider.enabled = true;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        await Task.Yield();
    }

    public string DisplayText()
    {
        return _absorber.Description;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (_ability.CharacterGameObject.CompareTag(Tags.Player) && state == BattleState.PlayerTurn)
            DecrementTurnsLeft();

        if (_ability.CharacterGameObject.CompareTag(Tags.Enemy) && state == BattleState.EnemyTurn)
            DecrementTurnsLeft();
    }

    void CheckCollision(Ability ability, Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for shieldable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
                Shield(stats);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // shield someone as they enter
        if (other.TryGetComponent(out CharacterStats stats))
            Shield(stats);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // remove shield
        if (other.TryGetComponent(out CharacterStats stats))
            RemoveShield(stats);
    }

    void Shield(CharacterStats stats)
    {
        stats.AddAbsorber(_absorber);
        _shieldedCharacter = stats;
    }

    void RemoveShield(CharacterStats stats)
    {
        stats.RemoveAbsorber(_absorber);
        _shieldedCharacter = null;
    }

    void DecrementTurnsLeft()
    {
        _numberOfTurnsLeft -= 1;
        if (_numberOfTurnsLeft <= 0)
            DestroySelf();
    }

    void DestroySelf()
    {
        if (_shieldedCharacter != null)
            RemoveShield(_shieldedCharacter);

        Destroy(gameObject);
    }
}
