using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SafetyWall : MonoBehaviour, ICreatable<Vector3, Ability>
{
    BoxCollider2D _selfCollider;

    int _numberOfTurnsLeft;

    Ability _ability;

    CharacterStats _shieldedCharacter;

    public async Task Initialize(Vector3 pos, Ability ability)
    {
        _ability = ability;
        _numberOfTurnsLeft = ability.BasePower;

        _selfCollider = GetComponent<BoxCollider2D>();
        _selfCollider.enabled = false;
        Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);
        CheckCollision(ability, col);
        _selfCollider.enabled = true;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        await Task.Yield();
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (_ability.CharacterGameObject.CompareTag(Tags.Player) && state == BattleState.PlayerTurn)
            DecrementTurnsLeft();

        if (_ability.CharacterGameObject.CompareTag(Tags.Enemy) && state == BattleState.EnemyTurn)
            DecrementTurnsLeft();
    }

    void CheckCollision(Ability ability, Collider2D col)
    {
        if (col == null)
            return;
        Debug.Log($"check collisioon {col.gameObject.name}");

        if (col.TryGetComponent(out CharacterStats stats))
            Shield(stats);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"on trigger enter {other.gameObject.name}");
        // shield someone as they enter

        if (other.TryGetComponent(out CharacterStats stats))
            Shield(stats);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"on trigger exit {other.gameObject.name}");
        // remove shield

        if (other.TryGetComponent(out CharacterStats stats))
            RemoveShield(stats);

    }

    void Shield(CharacterStats stats)
    {
        stats.StrengthShield = true;
        _shieldedCharacter = stats;
    }

    void RemoveShield(CharacterStats stats)
    {
        stats.StrengthShield = false;
        _shieldedCharacter = null;
    }

    void DecrementTurnsLeft()
    {
        Debug.Log($"turns lefT:  {_numberOfTurnsLeft - 1}");
        // if someone is shielded, unshield them
        _numberOfTurnsLeft -= 1;
        if (_numberOfTurnsLeft <= 0)
            DestroySelf();
    }

    void DestroySelf()
    {
        if (_shieldedCharacter != null)
        {
            _shieldedCharacter.StrengthShield = false;
            _shieldedCharacter = null;
        }

        Destroy(gameObject);
    }
}
