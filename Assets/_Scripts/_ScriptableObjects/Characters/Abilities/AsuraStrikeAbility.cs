using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Ability/Asura Strike")]
public class AsuraStrikeAbility : AttackAbility
{
    public async override Task AbilityLogic(Vector3 pos)
    {
        await base.AbilityLogic(pos);
        // lose all mana, lose all health-1, get teleported to the tile behind pos
        _stats.UseMana(_stats.CurrentMana);
        await _stats.TakeDamageFinal(_stats.CurrentHealth - 1);
    }

}
