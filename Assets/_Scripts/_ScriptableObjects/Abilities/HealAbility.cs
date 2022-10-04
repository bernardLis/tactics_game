using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Heal Ability")]
public class HealAbility : Ability
{
    HealTriggerable _healTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _healTriggerable = obj.GetComponent<HealTriggerable>();
    }

    public override bool CanBeUsed() { return ManaCheck() && TargetCheck(); }

    public override bool IsTargetViable(GameObject target)
    {
        // you can heal enemies but they are not returned here
        if (target.TryGetComponent(out IHealable<GameObject, Ability> healable)
            && target.CompareTag(CharacterGameObject.tag))
            return true;
        return false;
    }

    public async override Task AbilityLogic(Vector3 pos)
    {
        // heal target if successful play sound and retrun true;
        await _healTriggerable.Heal(pos, this, CharacterGameObject);
    }

    public override int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender, bool isRetaliation = false)
    {
        // return positive value coz it is heal // TODO: this is not a perfect way to do it...
        return -1 * base.CalculateInteractionResult(attacker, defender);
    }

}
