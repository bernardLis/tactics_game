using UnityEngine;
using System.Threading.Tasks;

public class PushTriggerable : BaseTriggerable
{
    public async Task<bool> Push(GameObject target, Ability ability)
    {
        // face the target character
        await _characterRendererManager.SpellcastAnimation(); // add animation for pushing

        // player can push characters/PushableObstacle
        Vector3 pushDir = (target.transform.position - transform.position).normalized;

        _myStats.UseMana(ability.ManaCost);

        await target.GetComponent<IPushable<Vector3, GameObject, Ability>>().GetPushed(pushDir, gameObject, ability);

        return true;
    }
}
