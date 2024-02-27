using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbilityHeal : BattleCreatureAbility
    {
        [SerializeField] GameObject _healEffect;
        [SerializeField] GameObject _healedEffect;

        protected override IEnumerator ExecuteAbilityCoroutine()
        {
            List<BattleEntity> copyOfAllies = new(BattleManager.Instance.GetAllies(BattleCreature));
            bool hasHealed = false;
            BattleEntity healedEntity = null;
            foreach (BattleEntity b in copyOfAllies)
            {
                if (b.HasFullHealth()) continue;
                if (b.IsDead) continue;
                hasHealed = true;
                healedEntity = b;
            }

            if (!hasHealed) healedEntity = BattleCreature;
            if (BattleCreature.HasFullHealth() && healedEntity == BattleCreature) yield break;

            _healEffect.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            if (healedEntity == null) yield break;
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