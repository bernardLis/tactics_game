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
        // looking for a target
        Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);
        if (col == null)
            return;
        target = col.gameObject;

        // looking for pushable target
        var pushableObject = target.GetComponent<IPushable<Vector3, GameObject, Ability>>();
        if (pushableObject == null)
            return;

        DisplayBattleLog(target, ability);

        _myStats.SetAttacker(true);
        // player can push characters/PushableObstacle
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        await target.GetComponent<IPushable<Vector3, GameObject, Ability>>()
                    .GetPushed(pushDir, gameObject, ability);
    }
}
