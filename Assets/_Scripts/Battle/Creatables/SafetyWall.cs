using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class SafetyWall : Creatable, ICreatable<Vector3, Ability, string>
{
    CharacterStats _shieldedCharacter;

    Vector3 _offsetVector = new Vector3(0.05f, 0.3f, 0f);

    public override async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        await base.Initialize(pos, ability, tag);
        transform.position = transform.position + _offsetVector;

        _numberOfTurnsLeft = ability.BasePower;

        CheckCollision(ability, pos);
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

    async void Shield(CharacterStats stats)
    {
        _shieldedCharacter = stats;
        await stats.AddStatus(Status, null);
    }

    void RemoveShield(CharacterStats stats)
    {
        _shieldedCharacter = null;
        stats.RemoveStatus(Status);
    }

    public override async Task DestroySelf(bool playEffects = true, bool scanAstar = true)
    {
        if (_shieldedCharacter != null)
            RemoveShield(_shieldedCharacter);
        await base.DestroySelf();
    }

    public override VisualElement DisplayText()
    {
        return new Label("Blocks damage.");
    }

}
