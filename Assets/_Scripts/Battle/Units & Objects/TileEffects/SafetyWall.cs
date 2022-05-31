using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SafetyWall : TileEffect, ICreatable<Vector3, Ability>
{
    [SerializeField] Absorber _absorber;

    CharacterStats _shieldedCharacter;

    Vector3 _offsetVector = new Vector3(0.05f, 0.3f, 0f);

    public override async Task Initialize(Vector3 pos, Ability ability)
    {
        await base.Initialize(pos, ability);
        transform.position = transform.position + _offsetVector;

        _ability = ability;
        _numberOfTurnsLeft = ability.BasePower;
        _absorber = Instantiate(_absorber);

        _selfCollider.enabled = false;
        CheckCollision(ability, pos);
        _selfCollider.enabled = true;
    }

    public override string DisplayText()
    {
        return _absorber.Description;
    }

    protected override void CheckCollision(Ability ability, Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for shieldable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
                Shield(stats);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // shield someone as they enter
        if (other.TryGetComponent(out CharacterStats stats))
            Shield(stats);
    }

    protected override void OnTriggerExit2D(Collider2D other)
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

    protected override void DestroySelf()
    {
        if (_shieldedCharacter != null)
            RemoveShield(_shieldedCharacter);
        base.DestroySelf();
    }
}
