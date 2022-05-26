using UnityEngine;
using System.Threading.Tasks;

public class PushTriggerable : BaseTriggerable
{
    public async Task Push(Vector3 pos, Ability ability)
    {
        GameObject target;
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        _myStats.SetAttacker(true);

        // looking for a target
        target = GetTarget(pos);
        if (target == null)
            return;

        DisplayBattleLog(target, ability);

        // player can push characters/PushableObstacle
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        await target.GetComponent<IPushable<Vector3, GameObject, Ability>>()
                    .GetPushed(pushDir, gameObject, ability);
    }

    GameObject GetTarget(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for pushable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out IPushable<Vector3, GameObject, Ability> pushable))
                return c.gameObject;
        return null;

    }

}
