using UnityEngine;
using System.Threading.Tasks;

public class PushTriggerable : MonoBehaviour
{
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public async Task<bool> Push(GameObject target, int manaCost)
    {
        // face the target character
        await characterRendererManager.SpellcastAnimation(); // add animation for pushing

        // player can push characters/stones
        // TODO: pushing characters with lerp breaks the A*
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        
        myStats.UseMana(manaCost);

        target.GetComponent<IPushable<Vector3>>().GetPushed(pushDir);
        // TODO: There is a better way to wait for target to get pushed
        await Task.Delay(500);

        return true;
    }
}
