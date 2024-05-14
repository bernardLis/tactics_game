using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Fight;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerHeal : AttackController
    {
        FightManager _fightManager;

        [SerializeField] GameObject _healEffect;
        [SerializeField] GameObject _healedEffect;

        public override void Initialize(UnitController unitController, Attack attack)
        {
            base.Initialize(unitController, attack);
            _fightManager = FightManager.Instance;
        }

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();

            //if (UnitController.Team != 0) yield break;

            List<UnitController> copyOfAllies = new(_fightManager.GetAllies(UnitController));
            bool hasHealed = false;
            UnitController healedEntity = null;
            foreach (UnitController b in copyOfAllies)
            {
                if (b.HasFullHealth()) continue;
                if (b.IsDead) continue;
                if (Vector3.Distance(b.transform.position, transform.position) > Attack.Range) continue;
                hasHealed = true;
                healedEntity = b;
            }

            if (!hasHealed) healedEntity = UnitController;
            if (UnitController.HasFullHealth() && healedEntity == UnitController)
                yield break;

            Animator.SetTrigger(AnimSpecialAttack);
            if (Attack.Sound != null) AudioManager.PlaySfx(Attack.Sound, transform.position);
            _healEffect.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            if (healedEntity == null) yield break;

            if (_healedEffect != null)
            {
                _healedEffect.transform.parent = healedEntity.transform;
                _healedEffect.transform.localPosition = Vector3.up * 2;
                _healedEffect.transform.localRotation = Quaternion.Euler(new(180, 0, 0));
                _healedEffect.SetActive(true);
            }

            healedEntity.GetHealed(Mathf.FloorToInt(Attack.GetDamage()));

            Invoke(nameof(CleanUp), 3f);
        }

        void CleanUp()
        {
            _healEffect.SetActive(false);

            if (_healedEffect == null) return;
            _healedEffect.transform.parent = transform;
            _healedEffect.SetActive(false);
        }
    }
}