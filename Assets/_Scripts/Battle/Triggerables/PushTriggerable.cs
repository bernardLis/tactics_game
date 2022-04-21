using UnityEngine;
using System.Threading.Tasks;

public class PushTriggerable : BaseTriggerable
{
    public async Task Push(GameObject target, Ability ability)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        _myStats.SetAttacker(true);

        if (target == null)
            return;

        // player can push characters/PushableObstacle
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        await target.GetComponent<IPushable<Vector3, GameObject, Ability>>()
                    .GetPushed(pushDir, gameObject, ability);
    }
}
