using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Weapon")]
public class Weapon : Equipment
{
    public WeaponType WeaponType;
    public Ability BasicAttack;

    public override void Initialize(GameObject equipmentHandler)
    {
        equipmentHandler.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;

        // if it is a bow, activate arrow gameobject
        if (WeaponType == WeaponType.Ranged)
            equipmentHandler.transform.Find("Arrow").gameObject.SetActive(true);
        else
            equipmentHandler.transform.Find("Arrow").gameObject.SetActive(false);
    }

}
