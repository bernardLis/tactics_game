using UnityEngine;
using System.Threading.Tasks;

public class PushTriggerable : BaseTriggerable
{
    public async Task<bool> Push(GameObject target, Ability ability)
    {
        // face the target character
        await _characterRendererManager.SpellcastAnimation(); // add animation for pushing

        // player can push characters/PushableObstacle
        // TODO: pushing characters with lerp breaks the A*
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        
        _myStats.UseMana(ability.ManaCost);

        target.GetComponent<IPushable<Vector3, Ability>>().GetPushed(pushDir, ability);
        // TODO: There is a better way to wait for target to get pushed
        await Task.Delay(500);

        return true;
    }
}
