using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class ControllerHeal : Controller
    {
        [SerializeField] GameObject _healEffect;
        [SerializeField] GameObject _healedEffect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            if (CreatureController.Team != 0) yield break;

            List<UnitController> copyOfAllies = new(BattleManager.GetAllies(CreatureController));
            bool hasHealed = false;
            UnitController healedEntity = null;
            foreach (UnitController b in copyOfAllies)
            {
                if (b.HasFullHealth()) continue;
                if (b.IsDead) continue;
                if (Vector3.Distance(b.transform.position, transform.position) > 20) continue;
                hasHealed = true;
                healedEntity = b;
            }

            if (!hasHealed) healedEntity = CreatureController;
            if (CreatureController.HasFullHealth() && healedEntity == CreatureController)
            {
                yield return base.ExecuteAbilityCoroutine();
                yield break;
            }

            Animator.SetTrigger(AnimAbility);
            AudioManager.PlaySFX(Ability.Sound, transform.position);
            _healEffect.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            if (healedEntity == null)
            {
                yield return base.ExecuteAbilityCoroutine();
                yield break;
            }

            _healedEffect.transform.parent = healedEntity.transform;
            _healedEffect.transform.localPosition = Vector3.up * 2;
            _healedEffect.transform.localRotation = Quaternion.Euler(new(180, 0, 0));
            _healedEffect.SetActive(true);
            healedEntity.GetHealed(5); // TODO: hardcoded value

            Invoke(nameof(CleanUp), 3f);
            yield return base.ExecuteAbilityCoroutine();
        }

        void CleanUp()
        {
            _healEffect.SetActive(false);
            _healedEffect.SetActive(false);
        }
    }
}