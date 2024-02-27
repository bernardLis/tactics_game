using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleSunBlossom : BattleCreatureMelee
    {
        [SerializeField] GameObject _healEffect;
        [SerializeField] GameObject _healedEffect;

        // //TODO: I'd prefer if it used its ability whenever it is off cooldown, it is not shielded and ability is available
        // protected override IEnumerator Attack()
        // {
        //     yield return ManageCreatureAbility();
        //     yield return base.Attack();
        // }
        //
        // protected override IEnumerator PathToOpponent()
        // {
        //     yield return ManageCreatureAbility();
        //     yield return base.PathToOpponent();
        // }
        //
        // protected override IEnumerator CreatureAbility()
        // {
        //     yield return base.CreatureAbility();
        //     Debug.Log("SunBlossom ability");
        //
        //     List<BattleEntity> copyOfAllies = new(BattleManager.Instance.GetAllies(this));
        //     bool hasHealed = false;
        //     BattleEntity healedEntity = null;
        //     foreach (BattleEntity b in copyOfAllies)
        //     {
        //         if (b.HasFullHealth()) continue;
        //         if (b.IsDead) continue;
        //         hasHealed = true;
        //         healedEntity = b;
        //     }
        //
        //     if (!hasHealed) healedEntity = this;
        //     if (Entity.CurrentHealth.Value == Entity.MaxHealth.GetValue() && healedEntity == this) yield break;
        //
        //     _healEffect.SetActive(true);
        //     yield return new WaitForSeconds(0.5f);
        //     if (healedEntity == null) yield break;
        //     _healedEffect.transform.parent = healedEntity.transform;
        //     _healedEffect.transform.localPosition = Vector3.up * 2;
        //     _healedEffect.transform.localRotation = Quaternion.Euler(new(180, 0, 0));
        //     _healedEffect.SetActive(true);
        //     healedEntity.GetHealed(5); // TODO: hardcoded value
        //
        //     StartCoroutine(ResetEffects());
        // }
        //
        // IEnumerator ResetEffects()
        // {
        //     yield return new WaitForSeconds(3f);
        //     _healEffect.SetActive(false);
        //     _healedEffect.SetActive(false);
        // }
    }
}