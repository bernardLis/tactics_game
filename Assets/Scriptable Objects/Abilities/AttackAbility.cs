using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Attack Ability")]
public class AttackAbility : Ability
{
    AttackTriggerable attackTriggerable;

    public bool isRetaliation { get; private set; }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        attackTriggerable = obj.GetComponent<AttackTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject _target)
    {
        // check if target is valid
        var attackableObject = _target.GetComponent<IAttackable<GameObject>>();
        if (attackableObject == null)
            return false;

        // interact
        if (!await attackTriggerable.Attack(_target, value, manaCost, aProjectile, isRetaliation))
            return false;

        SetIsRetaliation(false);

        // sound
        audioSource.clip = aSound;
        audioSource.Play();

        return true;
    }

    public void SetIsRetaliation(bool _is) { isRetaliation = _is; }

}
